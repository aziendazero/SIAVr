using System;


/// <summary>
/// Descrizione di riepilogo per UnhandledSettingException.
/// </summary>
public class UnhandledSettingException : Exception
{
    public UnhandledSettingException(string message)
        : base(message)
    {
    }

    public UnhandledSettingException(string message, Exception inner_exception)
        : base(message, inner_exception)
    {
    }
}