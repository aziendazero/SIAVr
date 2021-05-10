using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnVac.LogNotifiche.Dto
{
	public class NotificaRicezioneUpdate
	{
		public long Id { get; set; }
		public String IdMessaggio { get; set; }
		public DateTime? DataOraMessaggio { get; set; }
		public string ServiceRequest { get; set; }
	}
}