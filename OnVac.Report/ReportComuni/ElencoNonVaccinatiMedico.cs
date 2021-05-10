namespace Onit.OnAssistnet.OnVac.Report.ReportComuni
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Report1.
    /// </summary>
    public partial class ElencoNonVaccinatiMedico : Telerik.Reporting.Report
    {

        //public string Consultorio
        //{
        //    get
        //    {
        //        return Convert.ToString(this.ReportParameters["Consultorio"].Value);
        //    }
        //    set
        //    {
        //        this.ReportParameters["Consultorio"].Value = value;
        //    }
        //}

        public ElencoNonVaccinatiMedico()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #region " Formule "
        /// <summary>
        /// dditionally if you need to access a function that is not static (instance function), you could use the following expression:
        // "= ReportItem.Report.ItemDefinition.MyFunction(args)
        // where the ReportItem is the current item in which context the expression is evaluated, the Report keyword is used to get reference to the processing report and ItemDefinition gets the definition report.
        /// </summary>
        public string getHeader()
        {
            string result = @"Elenco non vaccinati {0} {1}
per gli assistiti nati dal {2} al {3}
residenti nel comune di {4}, circoscrizione di {5}
di sesso {6} e stato anagrafico {7}
{8}";


            string pConsultorio = this.ReportParameters["Consultorio"].Value.ToString();
            string consultorio = string.Empty;
            if (pConsultorio == "TUTTI")
            {
                consultorio = "per " + pConsultorio + " i centri vaccinali";
            }
            else
            {
                consultorio = "per il centro vaccinale " + pConsultorio;
            }

            string pDistretto = this.ReportParameters["Distretto"].Value.ToString();
            string distretto = string.Empty;
            if (pDistretto == "TUTTI")
            {
                distretto = "(" + pDistretto + " i distretti)";
            }
            else
            {
                distretto = "(distretto " + pDistretto + ")";
            }


            string pSesso = this.ReportParameters["Sesso"].Value.ToString();
            string sesso = string.Empty;
            switch (pSesso)
            {
                case "1":
                    sesso = "MASCHILE "; break;
                case "2":
                    sesso = "FEMMINILE"; break;
                default:
                    sesso = "MASCHILE E FEMMINILE"; break;
            }

            string pSAnagrafico = this.ReportParameters["SAnagrafico"].Value.ToString();
            string statoAnagrafico = string.Empty;
            if (string.IsNullOrEmpty(pSAnagrafico))
            {
                statoAnagrafico = "non specificato";
            }
            else
            {
                statoAnagrafico = pSAnagrafico;
            }


            string pDataEffettuazioneIniz = this.ReportParameters["DataEffettuazioneIniz"].Value.ToString();
            string pDataEffettuazioneFin = this.ReportParameters["DataEffettuazioneFin"].Value.ToString();
            string periodoEffettuazione = string.Empty;
            if ((!string.IsNullOrEmpty(pDataEffettuazioneIniz)) & (!string.IsNullOrEmpty(pDataEffettuazioneFin)))
            {
                periodoEffettuazione = "e vaccinazione effettuata da data " + pDataEffettuazioneIniz + " a data " + pDataEffettuazioneFin;
            }

            return string.Format(result,
                consultorio,
                distretto,
                this.ReportParameters["DataNascita1"].Value.ToString(),
                this.ReportParameters["DataNascita2"].Value.ToString(),
                this.ReportParameters["ComRes"].Value.ToString(),
                this.ReportParameters["Circoscriz"].Value.ToString(),
                sesso,
                statoAnagrafico,
                periodoEffettuazione);
        }


        public string dataStampa()
        {
            return DateTime.Today.ToShortDateString();
        }


        #endregion

    }

}