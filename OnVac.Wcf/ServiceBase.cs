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
using Onit.OnAssistnet.OnVac.Log;
using Onit.OnAssistnet.OnVac.Log.DataLogStructure;
using Onit.OnAssistnet.OnVac.Log.LogWriterProvider;

namespace Onit.OnAssistnet.OnVac.Wcf
{
    public class ServiceBase
    {
           // Costruttore statico: viene eseguito solo 1 volta, per istanziare il Log di OnVac.
        static ServiceBase()
        {
            InitLog();        
        }

        public ServiceBase()
        {
            LogBox.PreWriteLog += new LogBox.PreWriteLogEventHandler(LogBox_PreWriteLog);
            LogBox.PreWriteLogGroup += new LogBox.PreWriteLogGroupEventHandler(LogBox_PreWriteLogGroup);
        }
 
        public string Azienda
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CodiceAzienda"]))
                {
                    return ConfigurationManager.AppSettings["CodiceAzienda"].ToString();
                }
                else
                {
                    throw new Exception("Parametro CodiceAzienda non trovato");
                }
            }
        }

        public string AppId
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppId"]))
                {
                    return ConfigurationManager.AppSettings["AppId"].ToString();
                }
                else
                {
                    throw new Exception("Parametro AppId non trovato");
                }
            }
        }

        protected ConnectionInfo GetConnectionInfo()
        {
            Onit.Shared.Manager.Apps.App app = Onit.Shared.Manager.Apps.App.getInstance(this.AppId, this.Azienda);
            Onit.Shared.NTier.Dal.DAAB.DbConnectionInfo cInfo = app.getConnectionInfo();
            return new ConnectionInfo(cInfo.Provider.ToString(), cInfo.ConnectionString);
        }


        #region  Inizializzazione OnVac.Log

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

        private void LogBox_PreWriteLog(ILogWriterProvider log, Testata t, ref string connString)
        {
            CaricaArgomentiLog();
            CaricaCampiTestate(t);
        }

        private void LogBox_PreWriteLogGroup(ILogWriterProvider log, Testata[] t, ref string connString)
        {
            CaricaArgomentiLog();

            for (int i = 0; i < t.Length; i++)
            {
                CaricaCampiTestate(t[i]);
            }
        }

        // Lettura dei vari argomenti che servono al Log per l'inizializzazione. Tutti gli argomenti possibili sono elencati nella t_log_argomenti.
        // In caso di errore, viene sollevata una eccezione di tipo LogBoxWriteData, contenente come innerException quella avvenuta realmente.
        // In più viene impostato il messaggio della innerexception anche nel messaggio dell'eccezione del LogBox.
        // Questo perchè, a video, si può vedere subito se è una oracle exception (ed è più comodo per chi sta facendo l'installazione).
        private void CaricaArgomentiLog()
        {
            if (LogBox.Argomenti.Count == 0)
            {
                lock (LogBox.Argomenti)
                {
                    if (LogBox.Argomenti.Count == 0)
                    {
                        ConnectionInfo connectionInfo = this.GetConnectionInfo();

                        using (IDAM dam = DAMBuilder.CreateDAM(connectionInfo.Provider, connectionInfo.ConnectionString))
                        {
                            dam.QB.NewQuery();
                            dam.QB.AddSelectFields("LOA_CODICE, LOA_DESCRIZIONE, LOA_CRITICITA");
                            dam.QB.AddWhereCondition("LOA_ATTIVO", Comparatori.Uguale, "S", DataTypes.Stringa);
                            dam.QB.AddTables("T_LOG_ARGOMENTI");

                            using (System.Data.IDataReader idr = dam.BuildDataReader())
                            {
                                if (idr != null)
                                {
                                    int pos_cod = idr.GetOrdinal("LOA_CODICE");
                                    int pos_descr = idr.GetOrdinal("LOA_DESCRIZIONE");
                                    int pos_crit = idr.GetOrdinal("LOA_CRITICITA");

                                    try
                                    {
                                        Criticita crit;

                                        while (idr.Read())
                                        {
                                            if (idr.IsDBNull(pos_crit))
                                                crit = Criticita.Log;
                                            else
                                                crit = (Criticita)System.Enum.Parse(typeof(Criticita), idr[pos_crit].ToString());

                                            LogBox.Argomenti.Add(new Argomento(idr[pos_cod].ToString(), idr[pos_descr].ToString(), crit));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new LogBoxWriteDataException(ex, ex.Message);
                                    }
                                    finally
                                    {
                                        dam.Connection.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        // TODO: CaricaCampiTestate x GestioneConsenso
        private void CaricaCampiTestate(Testata t)
        {
            if (t.Maschera == null || t.Maschera == string.Empty) t.Maschera = "Wcf Gestione Consenso";
            
            // Utente ???
            //if (t.Utente == 0) t.Utente = uteId;
        }

        #endregion

    }
}