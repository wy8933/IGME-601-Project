using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugConsole : MonoBehaviour
{
    [Header("Console")]
    [SerializeField] private bool _showConsole = true;
    private string _input = "";
    private Vector2 _scroll;

    private GUIStyle _logStyle;

    [Serializable]
    public class CommandMeta { public string Id; public string Description; public string Format; public BaseEventSO<CommandRequested> commandEvent;}

    [SerializeField]
    private List<CommandMeta> _commands = new()
    {
        new CommandMeta{ Id="help", Description="List commands", Format="help" },
        new CommandMeta{ Id="test_command", Description="Run test", Format="test_command" },
    };

    [SerializeField] private ConsoleLogEventSO _logSource;

    void Awake()
    {
        EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = "Console ready. ` to toggle. Type 'help'.", Type = LogType.Log });

        // Allows the console to print the unity's logs
        Application.logMessageReceived += OnUnityLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= OnUnityLog;
    }

    /// <summary>
    /// Input System handler to toggle the visibility of the console.
    /// </summary>
    /// <param name="_">Unused input value payload.</param>
    public void OnToggleDebug(InputValue _) => _showConsole = !_showConsole;

    /// <summary>
    /// Input System handler for submit and return. If console is visible, submits current input line.
    /// </summary>
    /// <param name="_">Unused input value payload.</param>
    public void OnReturn(InputValue _)
    {
        if (_showConsole) 
        {
            Submit(_input); _input = ""; 
        }
    }

    /// <summary>
    /// Polls keyboard for toggling and submitting when the console is visible.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) _showConsole = !_showConsole;
        if (_showConsole && Keyboard.current.enterKey.wasPressedThisFrame) { Submit(_input); _input = ""; }
    }

    /// <summary>
    /// Renders the console UI, including input field and scrollable log area.
    /// </summary>
    void OnGUI()
    {
        if (!_showConsole) return;
        var lines = (_logSource != null) ? _logSource.Lines : Array.Empty<string>();

        _logStyle = new GUIStyle(GUI.skin.label)
        {
            wordWrap = true,
            richText = true,
            fontSize = 12
        };

        const float inputH = 35f;
        GUI.Box(new Rect(0, 0, Screen.width, inputH), "");
        _input = GUI.TextField(new Rect(10, 5, Screen.width - 15, inputH - 10), _input);

        float logTop = inputH + 5f;
        GUI.Box(new Rect(0, logTop - 5f, Screen.width, Screen.height - logTop), "");

        float outerW = Screen.width - 15f;
        float contentW = outerW - 15f;

        float y = 0f;
        for (int i = 0; i < lines.Count; i++)
        {
            float h = _logStyle.CalcHeight(new GUIContent(lines[i]), contentW);
            y += h + 5f;
        }
        float contentH = Mathf.Max(0f, y);


        _scroll = GUI.BeginScrollView(
            new Rect(10, logTop, outerW, Screen.height - logTop - 10),
            _scroll,
            new Rect(0, 0, contentW, contentH));

        float drawY = 0f;
        for (int i = 0; i < lines.Count; i++)
        {
            var gc = new GUIContent(lines[i]);
            float h = _logStyle.CalcHeight(gc, contentW);
            GUI.Label(new Rect(5, drawY, contentW - 10, h), gc, _logStyle);
            drawY += h + 5f;
        }

        GUI.EndScrollView();
    }

    /// <summary>
    /// Parses and executes a console command line. Supports "help" or dispatches to a configured command event.
    /// </summary>
    /// <param name="line">The raw input line to parse and execute.</param>
    void Submit(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        var tokens = Tokenize(line.Trim());
        var id = tokens.Count > 0 ? tokens[0].ToLowerInvariant() : string.Empty;
        var args = tokens.Count > 1 ? tokens.GetRange(1, tokens.Count - 1).ToArray() : Array.Empty<string>();

        if (id == "help")
        {
            foreach (var c in _commands)
            {
                var fmt = string.IsNullOrEmpty(c.Format) ? c.Id : c.Format;
                EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = $"{c.Id} — {c.Description}\n  {fmt}", Type = LogType.Log });
            }
            return;
        }

        var cmd = _commands.Find(c => string.Equals(c.Id, id, StringComparison.OrdinalIgnoreCase));
        if (cmd == null)
        {
            EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = $"Unknown command: '{id}'. Type 'help' to list commands.", Type = LogType.Warning });
            return;
        }

        if (cmd.commandEvent == null)
        {
            EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = $"No handler asset assigned for '{cmd.Id}'. Publishing anyway.", Type = LogType.Warning });
        }

        // it needs to be called under scope
        cmd.commandEvent.PublishToScopeKey(
            id,
            new CommandRequested { Id = cmd.Id, Args = args }
        );
    }

    /// <summary>
    /// Splits a raw command line into whitespace-delimited tokens.
    /// </summary>
    /// <param name="line">The raw command line.</param>
    /// <returns>List of tokens in order.</returns>
    static List<string> Tokenize(string line)
    {
        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return new List<string>(parts);
    }

    /// <summary>
    /// Receives Unity's log messages and forwards them to the console log event stream.
    /// </summary>
    /// <param name="condition">The log message text.</param>
    /// <param name="stacktrace">Associated stack trace (if any).</param>
    /// <param name="type">Unity log type (Log, Warning, Error, etc.).</param>
    void OnUnityLog(string condition, string stacktrace, LogType type)
    {
        EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = condition, Type = type });
    }
}
