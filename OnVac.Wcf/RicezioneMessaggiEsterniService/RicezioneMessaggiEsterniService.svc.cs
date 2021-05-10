using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Onit.OnAssistnet.OnVac.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RicezioneMessaggiEsterniService" in code, svc and config file together.
    public class RicezioneMessaggiEsterniService : ServiceBase, IRicezioneMessaggiEsterniService
    {
        RicezioneMessaggioResponse IRicezioneMessaggiEsterniService.RicezioneMessaggio(RicezioneMessaggioRequest request)
        {
            using (Onit.OnAssistnet.OnVac.Biz.BizRicezioneMessaggiEsterni _bizMsg = new Onit.OnAssistnet.OnVac.Biz.BizRicezioneMessaggiEsterni(
                    this.AppId,
                    this.Azienda,
                    request.EnteInvio,
                    request.EnteRicezione))
            {

                Onit.OnAssistnet.OnVac.Biz.RicezioneMessaggiEsterniResponse r = _bizMsg.RicezioneMessaggio(request.MessageContract);
                return new RicezioneMessaggioResponse()
                {
                    Message = r.Message,
                    Success = r.Success
                };

            }
        }
    }
}
