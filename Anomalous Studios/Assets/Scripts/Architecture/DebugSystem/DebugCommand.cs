using System;
using UnityEngine;

public class DebugCommandBase
{
    private string _commandID;
    private string _commandDescription;
    private string _commandFormat;

    public string CommandID
    {
        get => _commandID;
        private set => _commandID = value;
    }
    public string CommandDescription 
    {
        get => _commandDescription;
        private set => _commandDescription = value;
    }
    public string CommandFormat 
    {
        get => _commandFormat;
        private set => _commandFormat = value;
    }

    public DebugCommandBase(string id, string description, string format) 
    {
        _commandID = id;
        _commandDescription = description;
        _commandFormat = format;
    }
}

public class DebugCommand : DebugCommandBase
{
    private Action _command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format) 
    {
        _command = command;
   
    }

    public void Invoke() 
    {
        _command.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> _command;

    public DebugCommand(string id, string description, string format, Action<T> command) : base(id, description, format)
    {
        _command = command;

    }

    public void Invoke(T value)
    {
        _command.Invoke(value);
    }
}
