using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Onit.OnAssistnet.OnVac.Report.ReportViewer.Reports.EstrazioneControlliCentri;
using Onit.OnAssistnet.OnVac.Report.ReportViewer.Reports.EstrazioneControlliScuole;
using System.Data.OracleClient;
using Onit.OnAssistnet.Data;

namespace Onit.OnAssistnet.OnVac.Report.ReportViewer.Dal.Oracle
{
    public class OracleDalEstrazioneDati : IDalEstrazioneDati
    {
        #region Properties

        public string Provider { get; private set; }
        public IDbConnection Connection { get; private set; }

        // public IDbTransaction Transaction { get; private set; }

        #endregion

        #region Costruttore

        public OracleDalEstrazioneDati(string provider, string connectionString)
        {
            Provider = provider;
            Connection = (IDbConnection)GetOracleConnection(connectionString);
        }

        #endregion

        #region Private

        #region Connessione

        private OracleConnection GetOracleConnection(string connectionString)
        {
            // Se non ho la stringa di connessione, sollevo un'eccezione
            if (String.IsNullOrWhiteSpace(connectionString)) throw new Exception("Onit.OnAssistnet.OnVac.Report.ReportViewer.Dal: Impossibile creare la connessione!");

            return new OracleConnection(connectionString);
        }

        /// <summary>
        /// Se c'è una transazione esistente la associa al command passato per parametro.
        /// Altrimenti, se la connessione è chiusa, la apre e restituisce true.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private bool ConditionalOpenConnection(IDbCommand cmd)
        {
            bool ownConnectionOpened = false;

            //if (this.Transaction != null)
            //{
            //    cmd.Transaction = this.Transaction;
            //}
            //else
            //{
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
                ownConnectionOpened = true;
            }
            //}

            return ownConnectionOpened;
        }

        private void ConditionalCloseConnection(bool ownConnection)
        {
            if (ownConnection && Connection != null && Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }

        #endregion

        #endregion

        #region Public
        
        public IEnumerable<DettagliCentri> GetDettagliEstrazioneCentri(long idProcesso)
        {
            List<DettagliCentri> dettagli = new List<DettagliCentri>();

            using (OracleCommand cmd = new OracleCommand())
            {
                string query = @"select EXC_COGNOME, EXC_NOME, EXC_DATA_NASCITA, EXC_SESSO, EXC_CODICE_FISCALE,
EXC_COMUNE_SCUOLA, EXC_NOME_SCUOLA, EXC_INDIRIZZO_SCUOLA, EXC_ESITO, EXC_ERRORE, EXS_CODICE_MECCANOGRAFICO, EXS_CF_SCUOLA, EXS_DESCRIZIONE_SCUOLA, EXS_COMUNE_NASCITA,
EXC_VACCINAZIONE, EXC_DOSE, EXC_TIPO_OPERAZIONE, EXC_DATA, EXC_INFORMAZIONI, EXC_NOTE, EXS_CODICE_MECCANOGRAFICO, EXS_CF_SCUOLA, EXS_DESCRIZIONE_SCUOLA, EXS_COMUNE_NASCITA
from V_EXPORT_CENTRO
where EXC_PRC_ID_CONTROLLO = :prc_id
order by EXC_COGNOME, EXC_NOME, EXC_DATA_NASCITA, EXC_VACCINAZIONE";

                cmd.Parameters.AddWithValue("prc_id", idProcesso);
              
                cmd.Connection = (OracleConnection)Connection;
                cmd.CommandText = query;

                bool ownConnection = false;

                try
                {
                    ownConnection = ConditionalOpenConnection(cmd);

                    using (IDataReader idr = cmd.ExecuteReader())
                    {
                        if (idr != null)
                        {
                            int EXC_COGNOME = idr.GetOrdinal("EXC_COGNOME");
                            int EXC_NOME = idr.GetOrdinal("EXC_NOME");
                            int EXC_DATA_NASCITA = idr.GetOrdinal("EXC_DATA_NASCITA");
                            int EXC_SESSO = idr.GetOrdinal("EXC_SESSO");
                            int EXC_CODICE_FISCALE = idr.GetOrdinal("EXC_CODICE_FISCALE");
                            int EXC_COMUNE_SCUOLA = idr.GetOrdinal("EXC_COMUNE_SCUOLA");
                            int EXC_NOME_SCUOLA = idr.GetOrdinal("EXC_NOME_SCUOLA");
                            int EXC_INDIRIZZO_SCUOLA = idr.GetOrdinal("EXC_INDIRIZZO_SCUOLA");
                            int EXC_ESITO = idr.GetOrdinal("EXC_ESITO");
                            int EXC_ERRORE = idr.GetOrdinal("EXC_ERRORE");
                            int EXC_VACCINAZIONE = idr.GetOrdinal("EXC_VACCINAZIONE");
                            int EXC_DOSE = idr.GetOrdinal("EXC_DOSE");
                            int EXC_TIPO_OPERAZIONE = idr.GetOrdinal("EXC_TIPO_OPERAZIONE");
                            int EXC_DATA = idr.GetOrdinal("EXC_DATA");
                            int EXC_INFORMAZIONI = idr.GetOrdinal("EXC_INFORMAZIONI");
                            int EXC_NOTE = idr.GetOrdinal("EXC_NOTE");
                            int EXS_CODICE_MECCANOGRAFICO = idr.GetOrdinal("EXS_CODICE_MECCANOGRAFICO");
                            int EXS_CF_SCUOLA = idr.GetOrdinal("EXS_CF_SCUOLA");
                            int EXS_DESCRIZIONE_SCUOLA = idr.GetOrdinal("EXS_DESCRIZIONE_SCUOLA");
                            int EXS_COMUNE_NASCITA = idr.GetOrdinal("EXS_COMUNE_NASCITA");

                            while (idr.Read())
                            {
                                DettagliCentri item = new DettagliCentri();

                                item.Cognome = idr.GetStringOrDefault(EXC_COGNOME);
                                item.Nome = idr.GetStringOrDefault(EXC_NOME);
                                item.DataNascita = idr.GetNullableDateTimeOrDefault(EXC_DATA_NASCITA);
                                item.Sesso = idr.GetStringOrDefault(EXC_SESSO);
                                item.CodiceFiscale = idr.GetStringOrDefault(EXC_CODICE_FISCALE);
                                item.Scuola_Comune = idr.GetStringOrDefault(EXC_COMUNE_SCUOLA);
                                item.Scuola_Nome = idr.GetStringOrDefault(EXC_NOME_SCUOLA);
                                item.Scuola_Indirizzo = idr.GetStringOrDefault(EXC_INDIRIZZO_SCUOLA);
                                item.Esito = idr.GetStringOrDefault(EXC_ESITO);
                                item.Errore = idr.GetStringOrDefault(EXC_ERRORE);
                                item.Vaccinazione = idr.GetStringOrDefault(EXC_VACCINAZIONE);
                                item.Dose = idr.GetNullableInt32OrDefault(EXC_DOSE);
                                item.TipoOperazione = idr.GetStringOrDefault(EXC_TIPO_OPERAZIONE);
                                item.Data = idr.GetStringOrDefault(EXC_DATA);
                                item.Informazioni = idr.GetStringOrDefault(EXC_INFORMAZIONI);
                                item.Note = idr.GetStringOrDefault(EXC_NOTE);
                                item.CodiceMeccanografico = idr.GetStringOrDefault(EXS_CODICE_MECCANOGRAFICO);
                                item.CodiceFiscaleScuola = idr.GetStringOrDefault(EXS_CF_SCUOLA);
                                item.DescrizioneScuola = idr.GetStringOrDefault(EXS_DESCRIZIONE_SCUOLA);
                                item.ComuneNascita = idr.GetStringOrDefault(EXS_COMUNE_NASCITA);


                                dettagli.Add(item);
                            }
                        }
                    }
                }
                finally
                {
                    ConditionalCloseConnection(ownConnection);
                }
            }

            return dettagli.AsEnumerable();
        }

        public IEnumerable<DettagliScuole> GetDettagliEstrazioneScuole(long idProcesso)
        {
            List<DettagliScuole> dettagli = new List<DettagliScuole>();

            using (OracleCommand cmd = new OracleCommand())
            {
                string query = @"select EXS_COGNOME, EXS_NOME, EXS_DATA_NASCITA, EXS_SESSO, EXS_CODICE_FISCALE, 
EXS_COMUNE_SCUOLA, EXS_NOME_SCUOLA, EXS_INDIRIZZO_SCUOLA, EXS_ESITO, EXS_ERRORE,EXS_CODICE_MECCANOGRAFICO, EXS_CF_SCUOLA, EXS_DESCRIZIONE_SCUOLA, EXS_COMUNE_NASCITA
from V_EXPORT_SCUOLA
where EXS_PRC_ID_CONTROLLO = :prc_id
order by EXS_COGNOME, EXS_NOME, EXS_DATA_NASCITA";

                cmd.Parameters.AddWithValue("prc_id", idProcesso);

                cmd.Connection = (OracleConnection)Connection;
                cmd.CommandText = query;

                bool ownConnection = false;

                try
                {
                    ownConnection = ConditionalOpenConnection(cmd);

                    using (IDataReader idr = cmd.ExecuteReader())
                    {
                        if (idr != null)
                        {
                            int EXS_COGNOME = idr.GetOrdinal("EXS_COGNOME");
                            int EXS_NOME = idr.GetOrdinal("EXS_NOME");
                            int EXS_DATA_NASCITA = idr.GetOrdinal("EXS_DATA_NASCITA");
                            int EXS_SESSO = idr.GetOrdinal("EXS_SESSO");
                            int EXS_CODICE_FISCALE = idr.GetOrdinal("EXS_CODICE_FISCALE");
                            int EXS_COMUNE_SCUOLA = idr.GetOrdinal("EXS_COMUNE_SCUOLA");
                            int EXS_NOME_SCUOLA = idr.GetOrdinal("EXS_NOME_SCUOLA");
                            int EXS_INDIRIZZO_SCUOLA = idr.GetOrdinal("EXS_INDIRIZZO_SCUOLA");
                            int EXS_ESITO = idr.GetOrdinal("EXS_ESITO");
                            int EXS_ERRORE = idr.GetOrdinal("EXS_ERRORE");
                            int EXS_CODICE_MECCANOGRAFICO = idr.GetOrdinal("EXS_CODICE_MECCANOGRAFICO");
                            int EXS_CF_SCUOLA= idr.GetOrdinal("EXS_CF_SCUOLA");
                            int EXS_DESCRIZIONE_SCUOLA= idr.GetOrdinal("EXS_DESCRIZIONE_SCUOLA");
                            int EXS_COMUNE_NASCITA= idr.GetOrdinal("EXS_COMUNE_NASCITA");

                            while (idr.Read())
                            {
                                DettagliScuole item = new DettagliScuole();

                                item.Cognome = idr.GetStringOrDefault(EXS_COGNOME);
                                item.Nome = idr.GetStringOrDefault(EXS_NOME);
                                item.DataNascita = idr.GetNullableDateTimeOrDefault(EXS_DATA_NASCITA);
                                item.Sesso = idr.GetStringOrDefault(EXS_SESSO);
                                item.CodiceFiscale = idr.GetStringOrDefault(EXS_CODICE_FISCALE);
                                item.Scuola_Comune = idr.GetStringOrDefault(EXS_COMUNE_SCUOLA);
                                item.Scuola_Nome = idr.GetStringOrDefault(EXS_NOME_SCUOLA);
                                item.Scuola_Indirizzo = idr.GetStringOrDefault(EXS_INDIRIZZO_SCUOLA);
                                item.Esito = idr.GetStringOrDefault(EXS_ESITO);
                                item.Errore = idr.GetStringOrDefault(EXS_ERRORE);
                                item.CodiceMeccanografico = idr.GetStringOrDefault(EXS_CODICE_MECCANOGRAFICO);
                                item.CodiceFiscaleScuola = idr.GetStringOrDefault(EXS_CF_SCUOLA);
                                item.DescrizioneScuola = idr.GetStringOrDefault(EXS_DESCRIZIONE_SCUOLA);
                                item.ComuneNascita = idr.GetStringOrDefault(EXS_COMUNE_NASCITA);

                                dettagli.Add(item);
                            }
                        }
                    }
                }
                finally
                {
                    ConditionalCloseConnection(ownConnection);
                }
            }

            return dettagli.AsEnumerable();
        }

        public IEnumerable<ReportViewer.Reports.EstrazioneElencoConsulenze.DettaglioConsulenza> GetDettagliEstrazioneConsulenze(string filtroReport)
        {
            List<ReportViewer.Reports.EstrazioneElencoConsulenze.DettaglioConsulenza> dettagli = new List<ReportViewer.Reports.EstrazioneElencoConsulenze.DettaglioConsulenza>();

            using (OracleCommand cmd = new OracleCommand())
            {

                string filtro = @"";
                if (!string.IsNullOrWhiteSpace(filtroReport))
                {
                    filtro = @" " + filtroReport + " ";
                }

                string query = @"select PAZ_COGNOME, PAZ_NOME, PAZ_DATA_NASCITA, PAZ_CODICE_FISCALE, 
PAZ_CNS_CODICE, CNS_DESCRIZIONE, INT_DESCRIZIONE, PIT_DATA_INTERVENTO, PIT_DURATA, OPE_NOME, PIT_NOTE, COM_DESCRIZIONE
from T_PAZ_INTERVENTI
inner join T_ANA_INTERVENTI on PIT_INT_CODICE = INT_CODICE
inner join T_PAZ_PAZIENTI on PIT_PAZ_CODICE = PAZ_CODICE
left join T_ANA_CONSULTORI on PAZ_CNS_CODICE = CNS_CODICE
left join T_ANA_OPERATORI on PIT_OPE_CODICE = OPE_CODICE
left join T_ANA_COMUNI on PAZ_COM_CODICE_RESIDENZA=COM_CODICE
where PIT_UTE_ID_ELIMINAZIONE is null and PIT_DATA_ELIMINAZIONE is null " + filtro + " order by CNS_DESCRIZIONE, INT_DESCRIZIONE, PAZ_COGNOME, PAZ_NOME";

                cmd.Connection = (OracleConnection)Connection;
                cmd.CommandText = query;

                bool ownConnection = false;

                try
                {
                    ownConnection = ConditionalOpenConnection(cmd);

                    using (IDataReader idr = cmd.ExecuteReader())
                    {
                        if (idr != null)
                        {
                            int PAZ_COGNOME = idr.GetOrdinal("PAZ_COGNOME");
                            int PAZ_NOME = idr.GetOrdinal("PAZ_NOME");
                            int PAZ_DATA_NASCITA = idr.GetOrdinal("PAZ_DATA_NASCITA");
                            int PAZ_CODICE_FISCALE = idr.GetOrdinal("PAZ_CODICE_FISCALE");
                            int PAZ_CNS_CODICE = idr.GetOrdinal("PAZ_CNS_CODICE");
                            int CNS_DESCRIZIONE = idr.GetOrdinal("CNS_DESCRIZIONE");
                            int INT_DESCRIZIONE = idr.GetOrdinal("INT_DESCRIZIONE");
                            int PIT_DATA_INTERVENTO = idr.GetOrdinal("PIT_DATA_INTERVENTO");
                            int PIT_DURATA = idr.GetOrdinal("PIT_DURATA");
                            int OPE_NOME = idr.GetOrdinal("OPE_NOME");
                            int PIT_NOTE = idr.GetOrdinal("PIT_NOTE"); 
                            int COM_DESCRIZIONE = idr.GetOrdinal("COM_DESCRIZIONE");

                            while (idr.Read())
                            {
                                ReportViewer.Reports.EstrazioneElencoConsulenze.DettaglioConsulenza item = new ReportViewer.Reports.EstrazioneElencoConsulenze.DettaglioConsulenza();

                                item.Cognome = idr.GetStringOrDefault(PAZ_COGNOME);
                                item.Nome = idr.GetStringOrDefault(PAZ_NOME);
                                item.DataNascita = idr.GetNullableDateTimeOrDefault(PAZ_DATA_NASCITA);
                                item.CodiceFiscale = idr.GetStringOrDefault(PAZ_CODICE_FISCALE);
                                item.CodCNS = idr.GetStringOrDefault(PAZ_CNS_CODICE);
                                item.DescrCNS = idr.GetStringOrDefault(CNS_DESCRIZIONE);
                                item.DescrTipoConsulenza = idr.GetStringOrDefault(INT_DESCRIZIONE);
                                item.DataConsulenza = idr.GetNullableDateTimeOrDefault(PIT_DATA_INTERVENTO);
                                item.DurataConsulenza = idr.GetNullableInt32OrDefault(PIT_DURATA);
                                item.OperatoreDescr = idr.GetStringOrDefault(OPE_NOME);
                                item.Note = idr.GetStringOrDefault(PIT_NOTE);
                                item.ComuneResisdenza = idr.GetStringOrDefault(COM_DESCRIZIONE);

                                dettagli.Add(item);
                            }
                        }
                    }
                }
                finally
                {
                    ConditionalCloseConnection(ownConnection);
                }
            }

            return dettagli.AsEnumerable();
        }


        #endregion

        #region Disposing

        private bool disposed = false; // to detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //if (this.Transaction != null) this.Transaction.Dispose();
                    if (this.Connection != null) this.Connection.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            disposed = true;

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            //base.Dispose(disposing);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region  Finalize dell'oggetto

        // Da implementare solo se si utilizzano risorse non gestite.

        //~OracleDalEstrazioneDati()
        //{
        //    Dispose(false);
        //}

        #endregion

        #endregion
    }
}
