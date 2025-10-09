using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugConsoleEventBridge : MonoBehaviour
{
    [Header("Console")]
    [SerializeField] private bool _showConsole = true;
    private string _input = "";
    private Vector2 _scroll;
    private readonly List<string> _logLines = new();

    private GUIStyle _logStyle;

    [Serializable]
    public class CommandMeta { public string Id; public string Description; public string Format; }

    [SerializeField]
    private List<CommandMeta> _commands = new()
    {
        new CommandMeta{ Id="help", Description="List commands", Format="help" },
        new CommandMeta{ Id="test_command", Description="Run test", Format="test_command" },
        new CommandMeta{ Id="message", Description="Print a message", Format="message <text...>" }
    };

    private EventBinding<ConsoleLog> _logBinding;

    void Awake()
    {
        EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = "Console ready. ` to toggle. Type 'help'.", Type = LogType.Log });

        Application.logMessageReceived += OnUnityLog;
    }

    void OnEnable()
    {
        _logBinding = new EventBinding<ConsoleLog>(e =>
        {
            _logLines.Add($"[{e.Type}] {e.Message}");
            if (_logLines.Count > 500) _logLines.RemoveAt(0);
            _scroll.y = float.MaxValue;
        });
        EventBus<ConsoleLog>.Register(_logBinding);
    }

    void OnDisable()
    {
        EventBus<ConsoleLog>.DeRegister(_logBinding);
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= OnUnityLog;
    }

    public void OnToggleDebug(InputValue _) => _showConsole = !_showConsole;

    public void OnReturn(InputValue _)
    {
        if (_showConsole) 
        {
            Submit(_input); _input = ""; 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) _showConsole = !_showConsole;
        if (_showConsole && Keyboard.current.enterKey.wasPressedThisFrame) { Submit(_input); _input = ""; }
    }

    void OnGUI()
    {
        if (!_showConsole) return;

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
        for (int i = 0; i < _logLines.Count; i++)
        {
            float h = _logStyle.CalcHeight(new GUIContent(_logLines[i]), contentW);
            y += h + 4f; 
        }
        float contentH = Mathf.Max(0f, y);

        _scroll = GUI.BeginScrollView(
            new Rect(8, logTop, outerW, Screen.height - logTop - 10),
            _scroll,
            new Rect(0, 0, contentW, contentH));

        float drawY = 0f;
        for (int i = 0; i < _logLines.Count; i++)
        {
            var gc = new GUIContent(_logLines[i]);
            float h = _logStyle.CalcHeight(gc, contentW);
            GUI.Label(new Rect(4, drawY, contentW - 10, h), gc, _logStyle);
            drawY += h + 5f;
        }

        GUI.EndScrollView();
    }

    void Submit(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        var tokens = Tokenize(line);
        var id = tokens.Count > 0 ? tokens[0].ToLowerInvariant() : "";
        var args = tokens.Count > 1 ? tokens.GetRange(1, tokens.Count - 1).ToArray() : Array.Empty<string>();

        if (id == "help")
        {
            foreach (var c in _commands)
                EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = $"{c.Id} — {c.Description}\n  {c.Format}", Type = LogType.Log });
            return;
        }

        EventBus<CommandRequested>.RaiseScoped(CommandScope.Key(id), new CommandRequested { Id = id, Args = args });
    }

    static List<string> Tokenize(string line)
    {
        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return new List<string>(parts);
    }


    void OnUnityLog(string condition, string stacktrace, LogType type)
    {
        EventBus<ConsoleLog>.Raise(new ConsoleLog { Message = condition, Type = type });
    }
}
