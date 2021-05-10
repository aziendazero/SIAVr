using Onit.OnAssistnet.OnVac.Report.ReportViewer.Dal;
using Onit.Shared.Manager.Apps;
using Onit.Shared.NTier.Security;
using System;
using System.Configuration;
using System.Data;
using Microsoft.Reporting.WebForms;

namespace Onit.OnAssistnet.OnVac.Report.ReportViewer
{
    public enum TipoEsportazioneReport
    {
        PDF,
        XLS
    }

    public class ExportReport : IDisposable
    {
        #region Properties & Variables

        private String IdApplicazione { get; set; }
        private Int64 IdUtente { get; set; }
        private String CodiceAzienda { get; set; }

        private readonly IDalEstrazioneDati Dal;

        #endregion

        #region Costruttori

        public ExportReport(string idApplicazione, string codiceAzienda)
        {
            string provider = null;
            string connectionString = null;

            // <add name="Onit.OnAssistnet.ExportReport" providerName="ORACLE" connectionString="..."/>
            ConnectionStringSettings exportReportConnectionStringSettings = ConfigurationManager.ConnectionStrings["Onit.OnAssistnet.ExportReport"];

            if (exportReportConnectionStringSettings != null)
            {
                Crypto crypto = new Crypto(Providers.Rijndael);
                connectionString = crypto.Decrypt(exportReportConnectionStringSettings.ConnectionString);
                provider = exportReportConnectionStringSettings.ProviderName;
            }
            else
            {
                App app = App.getInstance(idApplicazione, codiceAzienda);
                connectionString = app.getConnectionInfo().ConnectionString;
                provider = app.DbmsProvider;
            }

            // Creazione DAL
            this.Dal = GetDALInstance(provider, connectionString);

            this.IdApplicazione = idApplicazione;
            this.CodiceAzienda = codiceAzienda;
        }

        #endregion

        #region Private

        #region Gestione DAL

        private IDalEstrazioneDati GetDalInstance(string provider, ref IDbConnection conn, ref IDbTransaction tx)
        {
            IDalEstrazioneDati dal;

            // Nome della classe dal in base al provider
            string strClass = GetClassName(provider);

            // Richiama il costruttore del dal in base ai parametri passati
            System.Reflection.Assembly asm;
            asm = System.Reflection.Assembly.GetExecutingAssembly();
            dal = (IDalEstrazioneDati)asm.CreateInstance(strClass, true, System.Reflection.BindingFlags.Default, null, new Object[] { provider, conn, tx }, null, null);

            return dal;
        }

        private IDalEstrazioneDati GetDALInstance(string provider, String connectionString)
        {
            IDalEstrazioneDati dal;

            // Nome della classe dal in base al provider
            string strClass = GetClassName(provider);

            // Richiama il costruttore del dal in base ai parametri passati
            System.Reflection.Assembly asm;
            asm = System.Reflection.Assembly.GetExecutingAssembly();
            dal = (IDalEstrazioneDati)asm.CreateInstance(strClass, true, System.Reflection.BindingFlags.Default, null, new Object[] { provider, connectionString }, null, null);

            return dal;
        }

        /// <summary>
        /// Restituisce il nome della classe in base al provider specificato
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        private string GetClassName(string provider)
        {
            string className = string.Empty;
            switch (provider.ToUpper())
            {
                case "ORACLE":
                    className = "Onit.OnAssistnet.OnVac.Report.ReportViewer.Dal.Oracle.OracleDalEstrazioneDati";
                    break;
                case "SQLSERVER":
                    //strClass = "Onit.OnAssistnet.OnVac.Report.ReportViewer.Dal.Sql.SqlDalEstrazioneDati";
                    //break;
                    throw new NotImplementedException("SqlDalEstrazioneDati non implementato");
            }

            return className;
        }

        #endregion

        #region Report

        private byte[] RenderReport(TipoEsportazioneReport tipoReport, LocalReport localReport)
        {
            // rendering del report
            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warning;

            string exportFormat = null;

            switch (tipoReport)
            {
                case TipoEsportazioneReport.PDF:
                    exportFormat = "PDF";
                    break;
                case TipoEsportazioneReport.XLS:
                    exportFormat = "Excel";
                    break;
            }

            return localReport.Render(exportFormat, null, out mimeType, out encoding, out fileNameExtension, out streams, out warning);
        }

        #endregion

        #endregion

        #region Public

        #region Estrazioni Controlli

        public byte[] ExportReportEstrazioneControlliCentri(long idProcessoControllo)
        {
            // nome dato al datasource dentro al .rdlc
            ReportDataSource source = new ReportDataSource("Dettagli");

            // popolamento datasource con i valori da passare al report
            source.Value = Dal.GetDettagliEstrazioneCentri(idProcessoControllo);

            // impostazioni report
            LocalReport localReport = new LocalReport();
            localReport.ReportEmbeddedResource = "Onit.OnAssistnet.OnVac.Report.ReportViewer.Reports.EstrazioneControlliCentri.EstrazioneCentri.rdlc";
            localReport.DataSources.Add(source);
            
            return RenderReport(TipoEsportazioneReport.XLS, localReport);
        }

        public byte[] ExportReportEstrazioneControlliScuole(long idProcessoControllo)
        {
            // nome dato al datasource dentro al .rdlc
            ReportDataSource source = new ReportDataSource("Dettagli");

            // popolamento datasource con i valori da passare al report
            source.Value = Dal.GetDettagliEstrazioneScuole(idProcessoControllo);

            // impostazioni report
            LocalReport localReport = new LocalReport();
            localReport.ReportEmbeddedResource = "Onit.OnAssistnet.OnVac.Report.ReportViewer.Reports.EstrazioneControlliScuole.EstrazioneScuole.rdlc";
            localReport.DataSources.Add(source);

            return RenderReport(TipoEsportazioneReport.XLS, localReport);
        }

        #endregion

        public byte[] ExportReportEstrazioneElencoConsulenze(long idProcessoControllo, string filtroReport)
        {
            // nome dato al datasource dentro al .rdlc
            ReportDataSource source = new ReportDataSource("Dettagli");

            // popolamento datasource con i valori da passare al report
            source.Value = Dal.GetDettagliEstrazioneConsulenze(filtroReport);

            // impostazioni report
            LocalReport localReport = new LocalReport();
            localReport.ReportEmbeddedResource = "Onit.OnAssistnet.OnVac.Report.ReportViewer.Reports.EstrazioneElencoConsulenze.EstrazioneConsulenze.rdlc";
            localReport.DataSources.Add(source);

            return RenderReport(TipoEsportazioneReport.XLS, localReport);
        }

        #endregion

        #region IDisposable

        private bool disposed = false; // to detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Dal != null)
                    {
                        Dal.Dispose();
                    }
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

        //~ExportReport()
        //{
        //    Dispose(false);
        //}

        #endregion

        #endregion
    }
}
