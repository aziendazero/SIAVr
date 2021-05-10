using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OnVac.LogNotifiche
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	[XmlSerializerFormat]
	public interface ILogNotifica
	{

		[OperationContract]
		string GetData(int value);

		[OperationContract]
		CompositeType GetDataUsingDataContract(CompositeType composite);

		// TODO: Add your service operations here
		[OperationContract]
		NotificaRispostaRicezione LogRispostaNotificaRicevuta(string idMessaggio, bool risultatoElaborazione, string messaggioElaborazione, string serviceResponse);
		[OperationContract]
		NotificaInserimentoRicezione LogInserimentoNotificaRicevuta(string idMessaggio, string paziente, string serviceRequest, string docoumentUniqueId);
		[OperationContract]
		NotificaInserimentoInvio LogInserimentoNotificaInvio(string idMessaggio, string codPaziente, string documentUniqueId, string Cda, string newDocUniqId, string reposUniqId, string sourceIp, string destUserId, Onit.OnAssistnet.OnVac.Enumerators.OperazioneLogNotifica operazioneLogNotifica);


	}


	// Use a data contract as illustrated in the sample below to add composite types to service operations.
	[DataContract]
	public class CompositeType
	{
		bool boolValue = true;
		string stringValue = "Hello ";

		[DataMember]
		public bool BoolValue
		{
			get { return boolValue; }
			set { boolValue = value; }
		}

		[DataMember]
		public string StringValue
		{
			get { return stringValue; }
			set { stringValue = value; }
		}
	}
}
