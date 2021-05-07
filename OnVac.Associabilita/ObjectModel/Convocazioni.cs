using System;
using System.Collections;


namespace Onit.OnAssistnet.OnVac.Associabilita.ObjectModel
{

    /// <summary>
    /// Classe Convocazioni
    /// </summary>
	public class ConvocazioniClass
	{

		private string _cod_cnv;
		/// <summary>
		/// Codice del ciclo o dell'associazione di cui controllare l'associabilità
		/// </summary>
		public string CodiceConvocazione
		{
			get { return _cod_cnv; }
			set { _cod_cnv = value; }
		}


		private int _num_sed;
        /// <summary>
        /// Numero della seduta
        /// </summary>
		public int NumeroSeduta
		{
			get { return _num_sed; }
			set { _num_sed = value; }
		}


		private DateTime _data_cnv;
        /// <summary>
        /// Data della convocazione
        /// </summary>
		public DateTime DataConvocazione
		{
			get { return _data_cnv; }
			set { _data_cnv = value; }
		}


		private int _intervallo;
        /// <summary>
        /// Intervallo
        /// </summary>
		public int Intervallo
		{
			get { return _intervallo; }
			set { _intervallo = value; }
		}

		
		private ArrayList _lista_vacc;
		/// <summary>
		/// Elenco dei codici delle vaccinazioni appartenenti alla convocazione
		/// </summary>
		public ArrayList ElencoVaccinazioni
		{
			get { return _lista_vacc; }
			set { _lista_vacc = value; }
		}


		// Costruttori

        /// <summary>
        /// Costruttore della classe. Imposta la data minvalue e crea una nuova lista di vaccinazioni, vuota.
        /// </summary>
		public ConvocazioniClass()
		{
			_data_cnv = DateTime.MinValue;
			_lista_vacc = new ArrayList();
		}

        /// <summary>
        /// Costruttore della classe.
        /// </summary>
        /// <param name="cod_cnv"></param>
        /// <param name="num_seduta"></param>
        /// <param name="data_convocazione"></param>
        /// <param name="intervallo"></param>
        /// <param name="elencoVaccinazioni"></param>
		public ConvocazioniClass(string cod_cnv, int num_seduta, DateTime data_convocazione, int intervallo, ArrayList elencoVaccinazioni)
		{
			_cod_cnv = cod_cnv;
			_num_sed = num_seduta;
			_data_cnv = data_convocazione;
			_intervallo = intervallo;
			_lista_vacc = elencoVaccinazioni;
		}
	

	}	// end ConvocazioniClass
	
	

	/// <summary>
	/// Collection di oggetti della classe Convocazioni
	/// </summary>
	public class ConvocazioniCollection : CollectionBase
	{

		/// <summary>
		/// Restituisce l'elemento di indice specificato nella collection, null altrimenti.
		/// </summary>
		/// <param name="index"></param>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public ConvocazioniClass this[int index]
		{
			get
			{
				if (List == null) return null;

				if (index < 0 | index >= List.Count) return null;

				return (ConvocazioniClass) List[index];
			}
		}


		/// <summary>
		/// Aggiunge l'elemento alla collection
		/// </summary>
		/// <param name="cnv"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public int Add(ConvocazioniClass cnv)
		{
			return List.Add(cnv);
		}


		/// <summary>
		/// Rimuove l'elemento dalla collection
		/// </summary>
		/// <param name="cnv"></param>
		/// <remarks></remarks>
		public void Remove(ConvocazioniClass cnv)
		{
			List.Remove(cnv);
		}


		/// <summary>
		/// Restituisce l'elemento con codice e numero seduta specificati. 
		/// Restituisce null se l'elemento non è presente o la collection è vuota.
		/// </summary>
		/// <param name="cod_cnv"></param>
		/// <param name="num_seduta"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public ConvocazioniClass FindByCodCnvSeduta(string cod_cnv, string num_seduta)
		{
			if (List == null) return null;

            ConvocazioniClass _el = null;
			for (int i = 0; i <List.Count; i++)
			{
                _el = (ConvocazioniClass) List[i];
				if (_el.CodiceConvocazione == cod_cnv && _el.NumeroSeduta.ToString() == num_seduta) return _el;
			}
			return null;
		}


		/// <summary>
        /// Costruttore
		/// </summary>
		public ConvocazioniCollection()
		{
		}

		
	}	// end ConvocazioniCollection




	/// <summary>
	/// Classe contenente la data di accorpamento e la collection di convocazioni che sono state accorpate in quella data.
	/// </summary>
	public class ConvocazioniAccorpateClass
	{
		/// <summary>
		/// Data di accorpamento.
		/// </summary>
        public DateTime DataConvocazione { get; set; }

		/// <summary>
		/// Elenco di convocazioni accorpate nella data relativa.
		/// </summary>
        public ConvocazioniCollection ElencoConvocazioni { get; set; }

		
		/// <summary>
        ///  Costruttore
		/// </summary>
		public ConvocazioniAccorpateClass()
		{
            ElencoConvocazioni = new ConvocazioniCollection();
		}
	

	}	// end ConvocazioniAccorpateClass



	/// <summary>
	/// Collection di oggetti di tipo ConvocazioniAccorpateClass.
	/// </summary>
	public class ConvocazioniAccorpateCollection : CollectionBase
	{
		/// <summary>
		/// Restituisce l'elemento di indice specificato nella collection, null altrimenti.
		/// </summary>
		/// <param name="index"></param>
		public ConvocazioniAccorpateClass this[int index]
		{
			get
			{
				if (List == null) return null;

				if (index < 0 | index >= List.Count) return null;

				return (ConvocazioniAccorpateClass) List[index];
			}
		}


		/// <summary>
		/// Aggiunge l'elemento alla collection
		/// </summary>
		/// <param name="cnv"></param>
		public int Add(ConvocazioniAccorpateClass cnv)
		{
			return List.Add(cnv);
		}


		/// <summary>
		/// Rimuove l'elemento dalla collection
		/// </summary>
		/// <param name="cnv"></param>
		public void Remove(ConvocazioniAccorpateClass cnv)
		{
			List.Remove(cnv);
		}


		/// <summary>
		/// Restituisce il primo elemento con data di convocazione specificata.
		/// Restituisce null se l'elemento non è presente o la collection è vuota.
		/// </summary>
        /// <param name="data_cnv"></param>
		public ConvocazioniAccorpateClass FindByDataConvocazione(DateTime data_cnv)
		{
			if (List == null) return null;

            ConvocazioniAccorpateClass _el = null;
            for (int i = 0; i < List.Count; i++ )
            {
                _el = (ConvocazioniAccorpateClass)List[i];
                if (_el.DataConvocazione == data_cnv) return _el;
            }
			return null;
		}

        
		/// <summary>
        /// Costruttore
		/// </summary>
		public ConvocazioniAccorpateCollection()
		{
		}


	}	// end ConvocazioniAccorpateCollection



}	// end namespace


