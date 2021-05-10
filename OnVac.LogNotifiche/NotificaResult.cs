using OnVac.LogNotifiche.Dto;
using System.Runtime.Serialization;

namespace OnVac.LogNotifiche
{
	[DataContract]
	public class NotificaResult	: Notifica
	{
		[DataMember]
		public bool? Success { get; set; }
		[DataMember]
		public string Message { get; set; }
		/// <summary>
		/// Id univoco del log relativo a questo messaggio
		/// </summary>
		public long IdLogNotifica { get; set; }

	}
}