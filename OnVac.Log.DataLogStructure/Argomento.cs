using System;

namespace Onit.OnAssistnet.OnVac.Log.DataLogStructure
{
	/// <summary>
	/// Descrizione di riepilogo per Argomento.
	/// </summary>
	[Serializable]
	public class Argomento
	{
        #region Properties

        private string _Codice;
        /// <summary>
        /// Codice argomento log
        /// </summary>
		public string Codice
		{
			get{return _Codice;}
			set{_Codice=value;}
		}

        private string _Descrizione;
        /// <summary>
        /// Descrizione argomento log
        /// </summary>
		public string Descrizione
		{
			get{return _Descrizione;}
			set{_Descrizione=value;}
		}

        private Criticita _Criticita;
        /// <summary>
        /// Criticità argomento
        /// </summary>
		public Criticita Criticita
		{
			get{return _Criticita;}
			set{_Criticita=value;}
        }

        private Boolean _Attivo;
        /// <summary>
        /// Flag che indica se l'argomento è attivo 
        /// </summary>
        public Boolean Attivo
        {
            get { return _Attivo; }
            set { _Attivo = value; }
        }

        #endregion

        #region Constructors 

        public Argomento() : this(string.Empty, string.Empty, Criticita.Log, true)
        { 
        }

        /// <summary>
        /// Costruttore: crea un argomento attivo con il codice specificato e criticità di livello "Log".
        /// </summary>
        /// <param name="codice"></param>
		public Argomento(string codice) : this(codice, string.Empty, Criticita.Log, true)
		{
		}

        /// <summary>
        /// Costruttore: crea un argomento attivo con il codice e la descrizione specificati, con criticità di livello "Log".
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="descrizione"></param>
		public Argomento(string codice, string descrizione) : this(codice, descrizione, Criticita.Log, true)
		{
		}
        
        /// <summary>
        /// Costruttore: crea un argomento attivo con codice, descrizione e criticità specificati.
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="descrizione"></param>
        /// <param name="criticita"></param>
		public Argomento(string codice, string descrizione, Criticita criticita) : this(codice, descrizione, criticita, true)
		{
        }
                
        /// <summary>
        /// Costruttore: crea un argomento attivo con codice, descrizione e criticità specificati.
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="descrizione"></param>
        /// <param name="criticita"></param>
		public Argomento(string codice, string descrizione, Criticita criticita, Boolean attivo)
		{
			this.Codice = codice;
			this.Descrizione = descrizione;
			this.Criticita = criticita;
            this.Attivo = attivo;
        }

        #endregion       
    }    	
}
