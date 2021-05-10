using System;


/// <summary>
/// Descrizione di riepilogo per CryptedSettingException.
/// </summary>
public class CryptedSettingException : Exception
{
	public CryptedSettingException(string msg, string setting_code, Exception inner_exception)
		: base(msg + " Codice parametro: " + setting_code, inner_exception)
	{
	}
}