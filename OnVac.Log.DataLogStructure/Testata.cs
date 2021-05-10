using System;

namespace Onit.OnAssistnet.OnVac.Log.DataLogStructure
{
    #region Types

    /// <summary>
    /// Tipo di operazione
    /// </summary>
    public enum Operazione
    {
        /// <summary>
        /// Inserimento
        /// </summary>
        Inserimento = 0,

        /// <summary>
        /// Eliminazione
        /// </summary>
        Eliminazione = 1,

        /// <summary>
        /// Modifica
        /// </summary>
        Modifica = 2,

        /// <summary>
        /// Eccezione
        /// </summary>
        Eccezione = 3,

        /// <summary>
        /// Generico
        /// </summary>
        Generico = 4
    }

    /// <summary>
    /// Livello di Criticità
    /// </summary>
    public enum Criticita
    {
        /// <summary>
        /// Log
        /// </summary>
        Log = 0,
        /// <summary>
        /// Warning
        /// </summary>
        Warning,
        /// <summary>
        /// Errore
        /// </summary>
        Errore,
        /// <summary>
        /// ErroreGrave
        /// </summary>
        ErroreGrave
    }

    /// <summary>
    /// Classe AddCampoEventArgs
    /// </summary>
    public class AddCampoEventArgs : EventArgs
    {
        /// <summary>
        /// campo
        /// </summary>
        public readonly Campo campo;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="campo"></param>
        public AddCampoEventArgs(Campo campo)
        {
            this.campo = campo;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// AddCampoEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void AddCampoEventHandler(object sender, AddCampoEventArgs e);

    #endregion
	
    /// <summary>
	/// Descrizione di riepilogo per Testata.
	/// </summary>
	[Serializable]
	public class Testata
    {
        #region Constructors 

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="argomentoLog"></param>
        /// <param name="operazioneLog"></param>
		public Testata(Argomento argomentoLog, Operazione operazioneLog)
		{
			this._Records = new RecordCollection();
			this._ArgomentoGenerico = argomentoLog;
			this._OperazioneBase = operazioneLog;
			this._DataOperazione = DateTime.Now;
			this._Records.AddCampo += new AddCampoEventHandler(this.AddedCampo);
		}

         /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="codiceArgomento"></param>
        /// <param name="operazioneLog"></param>
        public Testata(string codiceArgomento, Operazione operazioneLog)
		{
			this._Records = new RecordCollection();
            this._ArgomentoGenerico = new Argomento(codiceArgomento);
			this._OperazioneBase = operazioneLog;
			this._DataOperazione = DateTime.Now;
			this._Records.AddCampo += new AddCampoEventHandler(this.AddedCampo);
		}
         
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="argomentoLog"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="automatico"></param>
        public Testata(Argomento argomentoLog, Operazione operazioneLog, bool automatico)
            : this(argomentoLog, operazioneLog)
        {
            this._Automatico = automatico;
        }
         
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="argomentoLog"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        public Testata(Argomento argomentoLog, Operazione operazioneLog, double codicePaziente)
            : this(argomentoLog, operazioneLog)
		{
			this._Paziente = codicePaziente;
		}
         
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="codiceArgomento"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="automatico"></param>
        public Testata(string codiceArgomento, Operazione operazioneLog, bool automatico)
            : this(codiceArgomento, operazioneLog)
        {
            this._Automatico = automatico;
        }
         
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="codiceArgomento"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        public Testata(string codiceArgomento, Operazione operazioneLog, double codicePaziente)
            : this(codiceArgomento, operazioneLog)
        {
            this._Paziente = codicePaziente;
        }
         
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="argomentoLog"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        /// <param name="automatico"></param>
        public Testata(Argomento argomentoLog, Operazione operazioneLog, double codicePaziente, bool automatico)
            : this(argomentoLog, operazioneLog, codicePaziente)
        {
            this._Automatico = automatico;
        }
         
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="codiceArgomento"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        /// <param name="automatico"></param>
        public Testata(string codiceArgomento, Operazione operazioneLog, double codicePaziente, bool automatico)
            : this(codiceArgomento, operazioneLog, codicePaziente)
        {
            this._Automatico = automatico;
        }
        
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="argomentoLog"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        /// <param name="automatico"></param>
        /// <param name="maschera"></param>
        public Testata(Argomento argomentoLog, Operazione operazioneLog, double codicePaziente, bool automatico, string maschera)
            : this(argomentoLog, operazioneLog, codicePaziente, automatico)
        {
            this._Maschera = maschera;
        }
 
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="codiceArgomento"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        /// <param name="automatico"></param>
        /// <param name="maschera"></param>
        public Testata(string codiceArgomento, Operazione operazioneLog, double codicePaziente, bool automatico, string maschera)
            : this(codiceArgomento, operazioneLog, codicePaziente, automatico)
		{
			this._Maschera = maschera;
		}
 
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="argomentoLog"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        /// <param name="automatico"></param>
        /// <param name="maschera"></param>
        /// <param name="idUtente"></param>
        public Testata(Argomento argomentoLog, Operazione operazioneLog, double codicePaziente, bool automatico, string maschera, int idUtente)
            : this(argomentoLog, operazioneLog, codicePaziente, automatico, maschera)
        {
            this._Utente = idUtente;
        }
         
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="codiceArgomento"></param>
        /// <param name="operazioneLog"></param>
        /// <param name="codicePaziente"></param>
        /// <param name="automatico"></param>
        /// <param name="maschera"></param>
        /// <param name="idUtente"></param>
        public Testata(string codiceArgomento, Operazione operazioneLog, double codicePaziente, bool automatico, string maschera, int idUtente)
            : this(codiceArgomento, operazioneLog, codicePaziente, automatico, maschera)
        {
            this._Utente = idUtente;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Codice progressivo della testata
        /// </summary>
        public long? CodiceTestata { get; set; }
        
		private Argomento _ArgomentoGenerico;
        /// <summary>
        /// ArgomentoGenerico
        /// </summary>
		public Argomento ArgomentoGenerico
		{
			get { return this._ArgomentoGenerico; }
            set { this._ArgomentoGenerico = value; }
		}
 
		private bool _Automatico;
        /// <summary>
        /// Automatico
        /// </summary>
		public bool Automatico
		{
			get
			{ return this._Automatico; }
			set { this._Automatico = value;
			}
		}
 
		private bool _ChangedValues;
        /// <summary>
        /// ChangedValues
        /// </summary>
		public bool ChangedValues
		{
			get { return this._ChangedValues; }
			set { this._ChangedValues = value; }
		}
 
		private string _ComputerName;
        /// <summary>
        /// ComputerName
        /// </summary>
		public string ComputerName
		{
			get { return this._ComputerName; }
			set { this._ComputerName = value; }
		}
 
		private string _Consultorio;
        /// <summary>
        /// Consultorio
        /// </summary>
		public string Consultorio
		{
			get { return this._Consultorio; }
			set { this._Consultorio = value; }
		}
 
		private DateTime _DataOperazione;
        /// <summary>
        /// DataOperazione
        /// </summary>
		public DateTime DataOperazione
		{
			get { return this._DataOperazione; }
			set { this._DataOperazione = value; }
		}
 
		private bool _Intestazione;
        /// <summary>
        /// Intestazione
        /// </summary>
		public bool Intestazione
		{
			get { return this._Intestazione; }
			set { this._Intestazione = value; }
		}
 
		private string _Maschera;
        /// <summary>
        /// Maschera
        /// </summary>
		public string Maschera
		{
			get { return this._Maschera; }
			set { this._Maschera = value; }
		}
 
		private Operazione _OperazioneBase;
        /// <summary>
        /// OperazioneBase
        /// </summary>
		public Operazione OperazioneBase
		{
			get { return this._OperazioneBase; }
			set { this._OperazioneBase = value; }
		}
 
		private double _Paziente;
        /// <summary>
        /// Paziente
        /// </summary>
		public double Paziente
		{
			get { return this._Paziente; }
			set { this._Paziente = value; }
		}
 
		private RecordCollection _Records; 
        /// <summary>
        /// Records
        /// </summary>
		public RecordCollection Records
		{
			get { return this._Records; }
        }
        /*
         *
            set { this._Records = value; }
            get { return this._ArgomentoGenerico; }
                   set { this._ArgomentoGenerico = value; }*/
        private string _Stack; 
        /// <summary>
        /// Stack
        /// </summary>
		public string Stack
		{
			get { return this._Stack; }
			set { this._Stack = value; }
		}
 
		private int _Utente;
        /// <summary>
        /// Utente
        /// </summary>
		public int Utente
		{
			get { return this._Utente; }
			set { this._Utente = value; }
        }

        #endregion

        #region Private

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddedCampo(object sender, AddCampoEventArgs e)
        {
            if (e.campo.ValoreNuovo != e.campo.ValoreVecchio)
            {
                this._ChangedValues = true;
            }
        }

        #endregion
    }
}
