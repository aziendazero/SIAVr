using Onit.OnAssistnet.OnVac.Biz;
using Onit.OnAssistnet.OnVac.DAL;
using Onit.OnAssistnet.OnVac.Entities;
using Onit.OnAssistnet.OnVac.Log;
using Onit.OnAssistnet.OnVac.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace OnVac.LogNotifiche
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class LogNotifica : ILogNotifica
    {

        #region Context

        private class ServiceContextInfo
        {
            public int UserId { get; private set; }
            public string AppId { get; private set; }
            public string CodiceAzienda { get; private set; }
            public string CodiceUslCorrente { get; private set; }
            public string CodiceConsultorio { get; private set; }
            public string EventLogAppName { get; private set; }
			public string Inviare { get; private set; }

			public ServiceContextInfo()
            {
                string userId = GetAppSetting(AppSettingName.UserId);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new Exception("AppSetting mancante: UserId (web.config servizio Ricezione Notifiche).");
                }

                UserId = Convert.ToInt32(userId);

                AppId = GetAppSetting(AppSettingName.AppId);
                if (string.IsNullOrWhiteSpace(AppId))
                {
                    throw new Exception("AppSetting mancante: AppId (web.config servizio Ricezione Notifiche).");
                }

                CodiceAzienda = GetAppSetting(AppSettingName.CodiceAzienda);
                if (string.IsNullOrWhiteSpace(CodiceAzienda))
                {
                    throw new Exception("AppSetting mancante: CodiceAzienda (web.config servizio Ricezione Notifiche).");
                }

                CodiceUslCorrente = GetAppSetting(AppSettingName.CodiceUslCorrente);

                CodiceConsultorio = GetAppSetting(AppSettingName.CodiceConsultorio);

				Inviare = GetAppSetting(AppSettingName.Inviare);
            }
        }

        private Settings CreateSettings(string codiceConsultorio, DbGenericProvider genericProvider)
        {
            if (!string.IsNullOrWhiteSpace(codiceConsultorio) && codiceConsultorio != Onit.OnAssistnet.OnVac.Constants.CommonConstants.CodiceConsultorioSistema)
            {
                return new Settings(codiceConsultorio, genericProvider);
            }

            return new Settings(genericProvider);
        }

        private BizContextInfos CreateBizContextInfos(ServiceContextInfo context)
        {
            return new BizContextInfos(context.UserId, context.CodiceAzienda, context.AppId, context.CodiceConsultorio, context.CodiceUslCorrente, null);
        }

        private BizLogOptions CreateBizLogOptions()
        {
            return new BizLogOptions(Onit.OnAssistnet.OnVac.Log.DataLogStructure.TipiArgomento.PAZIENTI, true);
        }

        #endregion

        #region AppSettings

        private enum AppSettingName
        {
            UserId,
            AppId,
            CodiceAzienda,
            CodiceUslCorrente,
            CodiceConsultorio,
            EventLogAppName,
            EventLogLevel,
			Inviare,
		}

        /// <summary>
        /// Restituisce il valore dell'AppSetting specificato, letto dal web.config.
        /// </summary>
        /// <param name="appSettingName"></param>
        /// <returns></returns>
        private static string GetAppSetting(AppSettingName appSettingName)
        {
            // N.B. : l'elemento dell'Enum deve essere uguale al nome del setting!
            string name = Enum.GetName(typeof(AppSettingName), appSettingName);

            return System.Configuration.ConfigurationManager.AppSettings.Get(name);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore statico: viene eseguito solo 1 volta, per istanziare il Log di OnVac.
        /// </summary>
        static LogNotifica()
        {
            InitLog();
        }

        public LogNotifica()
        {
            LogBox.PreWriteLog += new LogBox.PreWriteLogEventHandler(LogBox_PreWriteLog);
            LogBox.PreWriteLogGroup += new LogBox.PreWriteLogGroupEventHandler(LogBox_PreWriteLogGroup);
        }

        #endregion

        #region OnVac.Log

        private static void InitLog()
        {
            try
            {
                LogBox.InitLog();

            }
            catch (LogBoxWriteDataException)
            {
                // Se arriva una eccezione di questo tipo, la lascio risalire tale quale (perchè è già tipizzata)
                throw;
            }
            catch (Exception ex)
            {
                //  Se è stata sollevata un'eccezione generica, ne faccio risalire una tipizzata (con la generica come innerexception)
                throw new LogBoxWriteDataException(ex);
            }
        }

        private void LogBox_PreWriteLog(Onit.OnAssistnet.OnVac.Log.LogWriterProvider.ILogWriterProvider log, Onit.OnAssistnet.OnVac.Log.DataLogStructure.Testata testataLog, ref string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || (LogBox.IsEnabled && LogBox.Argomenti.Count == 0))
            {
                using (DbGenericProviderFactory dbGenericProviderFactory = new DbGenericProviderFactory())
                {
                    ServiceContextInfo context = new ServiceContextInfo();

                    using (DbGenericProvider genericProvider = dbGenericProviderFactory.GetDbGenericProvider(context.AppId, context.CodiceAzienda))
                    {
                        if (string.IsNullOrWhiteSpace(connectionString))
                        {
                            connectionString = genericProvider.Connection.ConnectionString;
                        }

                        if (LogBox.IsEnabled)
                        {
                            CaricaArgomentiLog(genericProvider, context);
                            CaricaCampiTestate(testataLog);
                        }
                    }
                }
            }
        }

        private void LogBox_PreWriteLogGroup(Onit.OnAssistnet.OnVac.Log.LogWriterProvider.ILogWriterProvider log, Onit.OnAssistnet.OnVac.Log.DataLogStructure.Testata[] testateLog, ref string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString) || (LogBox.IsEnabled && LogBox.Argomenti.Count == 0))
            {
                using (DbGenericProviderFactory dbGenericProviderFactory = new DbGenericProviderFactory())
                {
                    ServiceContextInfo context = new ServiceContextInfo();

                    using (DbGenericProvider genericProvider = dbGenericProviderFactory.GetDbGenericProvider(context.AppId, context.CodiceAzienda))
                    {
                        if (string.IsNullOrWhiteSpace(connectionString))
                        {
                            connectionString = genericProvider.Connection.ConnectionString;
                        }

                        if (LogBox.IsEnabled && testateLog != null && testateLog.Length > 0)
                        {
                            for (int i = 0; i < testateLog.Length; i++)
                            {
                                CaricaCampiTestate(testateLog[i]);
                            }
                        }
                    }
                }
            }
        }

        private void CaricaArgomentiLog(DbGenericProvider genericProvider, ServiceContextInfo context)
        {
            if (LogBox.Argomenti.Count == 0)
            {
                lock (LogBox.Argomenti)
                {
                    if (LogBox.Argomenti.Count == 0)
                    {
                        using (BizLog bizLog = new BizLog(ref genericProvider, CreateBizContextInfos(context)))
                        {
                            List<Onit.OnAssistnet.OnVac.Log.DataLogStructure.Argomento> listArgomentiLog = bizLog.GetListArgomentiAttivi();

                            if (listArgomentiLog != null && listArgomentiLog.Count != 0)
                            {
                                foreach (var argomentoLog in listArgomentiLog)
                                {
                                    LogBox.Argomenti.Add(argomentoLog);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CaricaCampiTestate(Onit.OnAssistnet.OnVac.Log.DataLogStructure.Testata testataLog)
        {
            if (testataLog.Maschera == null || testataLog.Maschera == string.Empty)
            {
                testataLog.Maschera = "LogNotifica.Notifica";
            }
        }

        #endregion

        #region Public
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        /// <summary>
        /// Salvataggio della response nel log, per la notifica con id specificato.
        /// </summary>
        /// <param name="idMessaggio"></param>
        /// <param name="risultatoElaborazione"></param>
        /// <param name="messaggioElaborazione"></param>
        /// <returns></returns>
        public NotificaRispostaRicezione LogRispostaNotificaRicevuta(string idMessaggio, bool risultatoElaborazione, string messaggioElaborazione, string serviceResponse)
        {
            NotificaRispostaRicezione ret = new NotificaRispostaRicezione();
            int count = 0;
            using (DbGenericProviderFactory dbGenericProviderFactory = new DbGenericProviderFactory())
            {
                ServiceContextInfo context = new ServiceContextInfo();
                using (DbGenericProvider genericProvider = dbGenericProviderFactory.GetDbGenericProvider(context.AppId, context.CodiceAzienda))
                {
                    using (BizLogNotifiche bizLogNotifiche = new BizLogNotifiche(genericProvider, CreateBizContextInfos(context)))
                    {
                        try
                        {
                            count = bizLogNotifiche.UpdateDatiElaborazioneNotificaRicevuta(idMessaggio, Onit.OnAssistnet.OnVac.Enumerators.OperazioneLogNotifica.RecuperoCertVaccFSE, risultatoElaborazione, messaggioElaborazione, serviceResponse);
                        }
                        catch (Exception ex)
                        {
                            string err = string.Format("Eccezione non gestita durante la scrittura della response per la notifica con id: {0}.", idMessaggio.ToString());
                            ret.Success = false;
                            ret.Message = err;
                            EventLogWrite(ex, err, context);
                        }
                    }
                }
            }

            return ret;
        }
        /// <summary>
        /// Inserimento di un record nella tabella T_LOG_NOTIFICHE_RICEZIONE. Per il paziente è possibile inserire la stringa o utilizzare docoumentUniqueId.
        /// Con docoumentUniqueId viene determinato il paziente ad esso collegato e poi viene impostato come paziente del record da inserire
        /// </summary>
        /// <param name="idMessaggio"></param>
        /// <param name="paziente"></param>
        /// <param name="serviceRequest"></param>
        /// <param name="docoumentUniqueId"></param>
        /// <returns></returns>
        public NotificaInserimentoRicezione LogInserimentoNotificaRicevuta(string idMessaggio, string paziente, string serviceRequest, string docoumentUniqueId)
        {
            NotificaInserimentoRicezione ret = new NotificaInserimentoRicezione();
            int count = 0;
            using (DbGenericProviderFactory dbGenericProviderFactory = new DbGenericProviderFactory())
            {
                ServiceContextInfo context = new ServiceContextInfo();
                using (DbGenericProvider genericProvider = dbGenericProviderFactory.GetDbGenericProvider(context.AppId, context.CodiceAzienda))
                {
                    using (BizLogNotifiche bizLogNotifiche = new BizLogNotifiche(genericProvider, CreateBizContextInfos(context)))
                    {
                        try
                        {


                            if (string.IsNullOrWhiteSpace(paziente) && !string.IsNullOrWhiteSpace(docoumentUniqueId))
                            {
                                Settings settings = CreateSettings(context.CodiceConsultorio, genericProvider);
                                using (BizPaziente bizPaziente = new BizPaziente(genericProvider, settings, CreateBizContextInfos(context), null))
                                {
                                    int? paz = bizPaziente.GetCodicePazienteByDocumentUniqueId(docoumentUniqueId);

                                    if (paz.HasValue) { paziente = bizPaziente.GetCodiceAusiliario(paz.Value); }
                                }

                            }

                            if (!string.IsNullOrWhiteSpace(paziente))
                            {
                                BizLogNotifiche.ManageLogNotificaRicevutaCommand manageLogNotifica = new BizLogNotifiche.ManageLogNotificaRicevutaCommand();
                                manageLogNotifica.IdMessaggio = idMessaggio;
                                manageLogNotifica.CodiceCentralePaziente = paziente;
                                manageLogNotifica.ServiceRequest = serviceRequest;
                                manageLogNotifica.DocumentUniqueId = docoumentUniqueId;
                                manageLogNotifica.EntitySerializzata = null;
                                count = bizLogNotifiche.InsertLogNotificaRicevuta(manageLogNotifica);
                                if (count > 0)
                                {
                                    ret.Success = true;
                                    ret.IdLogNotifica = count;
                                }
                                else
                                {
                                    ret.Success = false;
                                    ret.Message = string.Format("Inserimento nella log ricezione fallito per messaggio {0} e per il paziente {1}.", idMessaggio, paziente);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            string err = string.Format("Eccezione non gestita durante la scrittura della response per la notifica con id: {0}", idMessaggio.ToString());
                            ret.Success = false;
                            ret.Message = err;
                            EventLogWrite(ex, err, context);
                        }
                    }
                }
            }
            return ret;
        }
        /// <summary>
        ///  Inserimento di un record nel log invio notifiche
        /// </summary>
        /// <param name="idMessaggio"></param>
        /// <param name="paziente"></param>
        /// <param name="serviceRequest"></param>
        /// <param name="docoumentUniqueId"></param>
        /// <returns></returns>
        public NotificaInserimentoInvio LogInserimentoNotificaInvio(string idMessaggio, string codPaziente, string documentUniqueId, string Cda, string newDocUniqId, string reposUniqId, string sourceIp, string destUserId, Onit.OnAssistnet.OnVac.Enumerators.OperazioneLogNotifica operazioneLogNotifica)
        {
            NotificaInserimentoInvio logNotificaInviata = new NotificaInserimentoInvio();
            using (DbGenericProviderFactory dbGenericProviderFactory = new DbGenericProviderFactory())
            {
                ServiceContextInfo context = new ServiceContextInfo();
                using (DbGenericProvider genericProvider = dbGenericProviderFactory.GetDbGenericProvider(context.AppId, context.CodiceAzienda))
                {
                    using (BizLogNotifiche bizLogNotifiche = new BizLogNotifiche(genericProvider, CreateBizContextInfos(context)))
                    {
                        try
                        {
                            int? paz = 0;

                            if (string.IsNullOrWhiteSpace(codPaziente) && !string.IsNullOrWhiteSpace(documentUniqueId))
                            {
                                Settings settings = CreateSettings(context.CodiceConsultorio, genericProvider);
                                using (BizPaziente bizPaziente = new BizPaziente(genericProvider, settings, CreateBizContextInfos(context), null))
                                {
                                    paz = bizPaziente.GetCodicePazienteByDocumentUniqueId(documentUniqueId);

                                    if (paz.HasValue) { codPaziente = bizPaziente.GetCodiceAusiliario(paz.Value); }
                                }

                            }

                            if (!string.IsNullOrWhiteSpace(codPaziente))
                            {
                                long paz_locale = Convert.ToInt64(paz.Value);

								// TODO [Log inserimento invio] manca insert e modifica parametri.

								// TODO [FSE]: il polo scritto fisso a 3 non è bellissimo...
								Onit.OnAssistnet.OnVac.Enumerators.StatoLogNotificaInviata? stato = null;

								if (operazioneLogNotifica.Equals(Onit.OnAssistnet.OnVac.Enumerators.OperazioneLogNotifica.StoricizzazioneCertVaccFSE)) {
									// controllo rispetto a parametro
									if (context.Inviare.Equals(0))
									{ 
										stato = Onit.OnAssistnet.OnVac.Enumerators.StatoLogNotificaInviata.DaNonInviare;
									}
								}

                                LogNotificaInviata reslogNotificaInviata = bizLogNotifiche.InsertLogNotificaInviata(paz_locale, codPaziente, null, 3, documentUniqueId, Cda, newDocUniqId, reposUniqId, sourceIp, destUserId, operazioneLogNotifica,stato);

                                logNotificaInviata.Success = true;
                                logNotificaInviata.IdLogNotifica = reslogNotificaInviata.IdNotifica;
                            }

                        }
                        catch (Exception ex)
                        {
                            string error = string.Format("Eccezione non gestita durante la scrittura della response per la notifica con id {0}, {1}", idMessaggio.ToString(), ex.Message);
                            logNotificaInviata.Success = false;
                            logNotificaInviata.Message = error;

                            EventLogWrite(ex, "Eccezione non gestita durante la scrittura della response per la notifica con id: " + idMessaggio.ToString(), context);
                        }
                    }
                }
            }
            return logNotificaInviata;
        }
        #endregion

        #region Private
        private void EventLogWrite(Exception ex, string message, ServiceContextInfo context)
        {
            if (!string.IsNullOrWhiteSpace(context.EventLogAppName))
            {
                if (ex != null)
                {
                    Onit.OnAssistnet.OnVac.Common.Utility.EventLogHelper.EventLogWrite(ex, message, System.Diagnostics.EventLogEntryType.Information, context.EventLogAppName);
                }
                {
                    Onit.OnAssistnet.OnVac.Common.Utility.EventLogHelper.EventLogWrite(message, System.Diagnostics.EventLogEntryType.Information, context.EventLogAppName);
                }
            }
        }
        #endregion


    }
}
