using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

using Onit.Database.DataAccessManager;

using Onit.OnAssistnet.Contracts;

using Onit.OnAssistnet.OnVac;
using Onit.OnAssistnet.OnVac.DAL;
using Onit.OnAssistnet.OnVac.Biz;
using Onit.OnAssistnet.OnVac.Log;
using Onit.OnAssistnet.OnVac.Log.DataLogStructure;
using Onit.OnAssistnet.OnVac.Log.LogWriterProvider;

namespace Onit.OnAssistnet.OnVac.Wcf
{
    public class GestioneConsensoCallbackService : ServiceBase, IPazienteHL7Soap
    {
        public NotificaInCentraleResponse NotificaInCentrale(NotificaInCentraleRequest request)
        {
            Onit.OnAssistnet.OnVac.Settings.Settings settings = null;
            using (DbGenericProvider genericProvider = new DbGenericProvider(this.GetConnectionInfo()))
            {
                settings = new Onit.OnAssistnet.OnVac.Settings.Settings(genericProvider);
            }

            IEnumerable<Consenso> consensiComunicazione = 
                request.cpaz.DatiAnagrafici.Consensi.Where(p => p.ConsensoID == settings.CONSENSO_ID_COMUNICAZIONE);

            if (consensiComunicazione != null && consensiComunicazione.Count() > 0)
            {
                string flagVisibilitDatiVaccinaliCentrali = null;

                TipoEvento livelloConsenso = consensiComunicazione.First().TipoCodificato;

                switch (livelloConsenso)
                {
                    case Onit.OnAssistnet.Contracts.TipoEvento.NEGAZIONECONSENSO:
                    case Onit.OnAssistnet.Contracts.TipoEvento.REVOCACONSENSO:
                    case Onit.OnAssistnet.Contracts.TipoEvento.REVOCACONCANCELLAZIONECONSENSO:

                        flagVisibilitDatiVaccinaliCentrali = Onit.OnAssistnet.OnVac.Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente;
                        break;

                    case Onit.OnAssistnet.Contracts.TipoEvento.RILASCIOCONSENSO:

                        flagVisibilitDatiVaccinaliCentrali = Onit.OnAssistnet.OnVac.Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente;
                        break;

                    default:

                        throw new NotImplementedException();
                }

                Onit.Shared.Manager.Entities.T_ANA_UTENTI ute = Onit.Shared.Manager.Security.UserDbManager.GetUser(request.strAddetto, this.Azienda);
                using (DbGenericProviderFactory dbGenericProviderFactory = new DbGenericProviderFactory())
                {
                    BizContextInfos bInfo = new BizContextInfos(ute.UTE_ID, this.Azienda, this.AppId);
                    BizLogOptions logOpts = new Onit.OnAssistnet.OnVac.Biz.BizLogOptions(Onit.OnAssistnet.OnVac.Log.DataLogStructure.TipiArgomento.PAZIENTI);

                    using (Onit.OnAssistnet.OnVac.Biz.BizPaziente bizPaziente = new Biz.BizPaziente(dbGenericProviderFactory, settings, bInfo, logOpts))
                    {
                        bizPaziente.AggiornaVisibilitaDatiVaccinaliCentrali(
                            request.cpaz.DatiAnagrafici.GetIDValue(TipoCodificatoID.Aziendale),
                            flagVisibilitDatiVaccinaliCentrali,
                            (flagVisibilitDatiVaccinaliCentrali == Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente ?
                                Constants.ValoriVisibilitaDatiVaccinali.NegatoDaPaziente :
                                Constants.ValoriVisibilitaDatiVaccinali.ConcessoDaPaziente));
                    }
                }
            }

            return new NotificaInCentraleResponse(true, string.Empty);
        }
    }
}