using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Onit.OnAssistnet.OnVac.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRicezioneMessaggiEsterniService" in both code and config file together.
    [ServiceContract]
    public interface IRicezioneMessaggiEsterniService
    {
        [OperationContract]
        RicezioneMessaggioResponse RicezioneMessaggio(RicezioneMessaggioRequest request);
        
    }

    [DataContract]
    public class RicezioneMessaggioRequest
    {
        [DataMember]
        public Onit.OnAssistnet.Contracts.Messaggio MessageContract { get; set; }
        [DataMember]
        public string EnteInvio { get; set; }
        [DataMember]
        public string EnteRicezione { get; set; }
    }

    [DataContract]
    public class  RicezioneMessaggioResponse
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool Success { get; set; }
    }

}
