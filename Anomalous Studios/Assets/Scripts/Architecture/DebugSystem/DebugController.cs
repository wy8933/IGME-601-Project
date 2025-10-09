using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = System.Object;

public class DebugController : MonoBehaviour
{
    private bool _showConsole;
    private bool _showHelp;

    private string _input;

    private Vector2 _scroll;

    public static DebugCommand Test;
    public static DebugCommand Help;
    public static DebugCommand<string> MessageCommand;

    public List<Object> commandList;


    public void Awake()
    {
        Test = new DebugCommand("test_command", "This is a testing command", "test_command", () => 
        {
            Debug.Log("test command works");
        });

        Help = new DebugCommand("help", "show a list of commands", "help", () =>
        {
            _showHelp = true;
        });

        MessageCommand = new DebugCommand<string>("message_command", "This is a command that prints a message", "message_command", (value) =>
        {
            Debug.Log(value);
        });

        commandList = new List<Object>
        {
            Test,
            Help,
            MessageCommand
        };
    }

    public void OnReturn(InputValue value) 
    {
        if (_showConsole) 
        {
            HandleInput();
            _input = "";
        }
    }

    public void OnToggleDebug(InputValue value) 
    {
        _showConsole = !_showConsole;
    }

    // Remove this
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) 
        {
            if (_showConsole)
            {
                HandleInput();
                _input = "";
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _showConsole = !_showConsole;
        }
    }

    private void OnGUI()
    {


        if (_showConsole) 
        {
            float y = 0f;

            if (_showHelp) 
            {
                GUI.Box(new Rect(0, y, Screen.width, 100), "");

                Rect viewport = new Rect(0, 0, Screen.width -30, 20*commandList.Count);

                _scroll = GUI.BeginScrollView(new Rect(0, y+5f, Screen.width, 90), _scroll, viewport);

                for (int i = 0; i < commandList.Count; i++) 
                {
                    DebugCommandBase command = commandList[i] as DebugCommandBase;

                    string label = $"{command.CommandFormat} - {command.CommandDescription}";

                    Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);

                    GUI.Label(labelRect, label);
                }

                GUI.EndScrollView();

                y += 100;
            }

            GUI.Box(new Rect(0, y, Screen.width, 30), "");

            GUI.backgroundColor = Color.white;

            _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), _input);
        }
    }

    private void HandleInput() 
    {
        string[] properties = _input.Split(' ');

        for (int i = 0; i < commandList.Count; i++) 
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (_input.Contains(commandBase.CommandID)) 
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null) 
                {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
                else if (commandList[i] as DebugCommand<float> != null)
                {
                    (commandList[i] as DebugCommand<float>).Invoke(float.Parse(properties[1]));
                }
                else if (commandList[i] as DebugCommand<string> != null)
                {
                    (commandList[i] as DebugCommand<string>).Invoke(properties[1]);
                }
            }
        }
    }
}
