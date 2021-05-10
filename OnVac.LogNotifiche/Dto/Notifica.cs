using System;

namespace OnVac.LogNotifiche.Dto
{
	public class Notifica
	{   /// <summary>
		/// Data e ora di invio/ricezione del messaggio 
		/// </summary>
		public DateTime? DataOraMessaggio { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string IdMessaggio { get; set; }
		/// <summary>
		/// Contiene l'intera richiesta ricevuta/inviata
		/// </summary>
		public string ServiceRequest { get; set; }

	}
}