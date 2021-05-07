using System;
using System.Data;
using System.Collections;
using Onit.OnAssistnet.OnVac.Associabilita.ObjectModel;


namespace Onit.OnAssistnet.OnVac.Associabilita
{
	/// <summary>
	/// Classe per il controllo dell'associabilità delle convocazioni.
	/// </summary>
	public class ControlloAssociabilita
	{

		private string _provider;
		private IDbConnection _conn;
		private IDbTransaction _tx;


		// Nome campi del datatable da cui viene generata la collection delle convocazioni
		// Fissi per OnVac. Se servisse cambiarli, si possono passare dall'esterno.
		private string CAMPO_CODICE_CICLO;
		private string CAMPO_CODICE_ASSOCIAZIONE;
		private string CAMPO_NUMERO_SEDUTA;
		private string CAMPO_NUMERO_RICHIAMO;
		private string CAMPO_DATA_CONVOCAZIONE;
		private string CAMPO_INTERVALLO;
		private string CAMPO_CODICE_VACCINAZIONE;



		#region --- Costruttori ---
        

		/// <summary>
        /// Classe per gestire i nomi dei campi del datatable da cui creare la collection delle convocazioni
		/// </summary>
		public class CampiDataTable
		{
			private string _campoCodiceCiclo;
            /// <summary>
            /// Codice Ciclo
            /// </summary>
			public string CampoCodiceCiclo
			{
				get { return _campoCodiceCiclo; }
			}

			private string _campoCodiceAssociazione;
            /// <summary>
            /// Codice Associazione
            /// </summary>
			public string CampoCodiceAssociazione
			{
				get { return _campoCodiceAssociazione; }
			}

			private string _campoNumeroSeduta;
            /// <summary>
            /// Numero Seduta
            /// </summary>
			public string CampoNumeroSeduta
			{
				get { return _campoNumeroSeduta; }
			}

            private string _campoNumeroRichiamo;
            /// <summary>
            /// Numero Richiamo
            /// </summary>
			public string CampoNumeroRichiamo
			{
				get { return _campoNumeroRichiamo; }
			}

			private string _campoDataConvocazione;
            /// <summary>
            /// Data Convocazione
            /// </summary>
			public string CampoDataConvocazione
			{
				get { return _campoDataConvocazione; }
			}
			
			private string _campoIntervallo;
            /// <summary>
            /// Intervallo
            /// </summary>
			public string CampoIntervallo
			{
				get { return _campoIntervallo; }
			}

			private string _campoCodiceVaccinazione;
            /// <summary>
            /// Codice Vaccinazione
            /// </summary>
			public string CampoCodiceVaccinazione
			{
				get { return _campoCodiceVaccinazione; }
			}


            /// <summary>
            /// Campi DataTable
            /// </summary>
            /// <param name="campo_codice_ciclo"></param>
            /// <param name="campo_codice_associazione"></param>
            /// <param name="campo_num_seduta"></param>
            /// <param name="campo_num_richiamo"></param>
            /// <param name="campo_data_cnv"></param>
            /// <param name="campo_cod_vac"></param>
            /// <param name="campo_intervallo"></param>
			public CampiDataTable(string campo_codice_ciclo, string campo_codice_associazione, string campo_num_seduta, string campo_num_richiamo, string campo_data_cnv, string campo_cod_vac, string campo_intervallo)
			{
				_campoCodiceCiclo = campo_codice_ciclo;
				_campoCodiceAssociazione = campo_codice_associazione;
				_campoNumeroSeduta = campo_num_seduta;
				_campoNumeroRichiamo = campo_num_richiamo;
				_campoDataConvocazione = campo_data_cnv;
				_campoCodiceVaccinazione = campo_cod_vac;
				_campoIntervallo = campo_intervallo;
			}

		}


		/// <summary>
        /// Costruttore standard (nomi dei campi di OnVac)
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="conn"></param>
		/// <param name="tx"></param>
		public ControlloAssociabilita(string provider, ref IDbConnection conn, ref IDbTransaction tx)
		{
			_provider = provider;
			_conn = conn;
			_tx = tx;

			CAMPO_CODICE_CICLO = "cic_codice";
			CAMPO_CODICE_ASSOCIAZIONE = "sas_ass_codice";
			CAMPO_NUMERO_SEDUTA = "sed_n_seduta";
			CAMPO_DATA_CONVOCAZIONE = "convocazione";
			CAMPO_INTERVALLO = "tsd_intervallo";
			CAMPO_CODICE_VACCINAZIONE = "sed_vac_codice";
			CAMPO_NUMERO_RICHIAMO = "sed_n_richiamo";
		}

		
		/// <summary>
        /// Costruttore previsto per specificare altri nomi dei campi
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="conn"></param>
		/// <param name="tx"></param>
		/// <param name="nomi_campi"></param>
		public ControlloAssociabilita(string provider, ref IDbConnection conn, ref IDbTransaction tx, CampiDataTable nomi_campi)
		{
			_provider = provider;
			_conn = conn;
			_tx = tx;

			CAMPO_CODICE_CICLO = nomi_campi.CampoCodiceCiclo;
			CAMPO_CODICE_ASSOCIAZIONE = nomi_campi.CampoCodiceAssociazione;
			CAMPO_NUMERO_SEDUTA = nomi_campi.CampoNumeroSeduta;
			CAMPO_DATA_CONVOCAZIONE = nomi_campi.CampoDataConvocazione;
			CAMPO_INTERVALLO = nomi_campi.CampoIntervallo;
			CAMPO_CODICE_VACCINAZIONE = nomi_campi.CampoCodiceVaccinazione;
			CAMPO_NUMERO_RICHIAMO = nomi_campi.CampoNumeroRichiamo;
		}

		#endregion



		#region --- Creazione collection convocazioni ---
		

		/// <summary>
		///  Restituisce una collection di Convocazioni, ottenuti in base al datatable specificato.
		/// </summary>
		/// <param name="dta"></param>
		/// <remarks>Il datatable deve i campi seguenti: codice ciclo, numero seduta, data convocazione, codice vaccinazione.
		///  I nomi dei campi possono essere specificati nel costruttore della classe.
		/// </remarks>
		/// <returns></returns>
		public ConvocazioniCollection CreaCollectionConvocazioni(DataTable dta)
		{
			ConvocazioniCollection _coll_cnv = new ObjectModel.ConvocazioniCollection();
			ConvocazioniClass _cnv = null;
			
			string _cod;
			int _num_sed, _interv;
			DateTime _data_cnv;
			bool _to_add;

			
			// Aggiunta convocazioni in base ai dati del datatable
			for (int i=0; i<dta.Rows.Count; i++)
			{
				// Controllo valori dei campi

				// Codice
				if (!dta.Rows[i].Table.Columns.Contains(CAMPO_CODICE_CICLO) || dta.Rows[i][CAMPO_CODICE_CICLO] is System.DBNull)
				{
					// Se il campo contenente il codice del ciclo è nullo, utilizzo il codice dell'associazione
					if (!dta.Rows[i].Table.Columns.Contains(CAMPO_CODICE_ASSOCIAZIONE) || dta.Rows[i][CAMPO_CODICE_ASSOCIAZIONE] is System.DBNull)
					{
						// Se il campo contenente il codice dell'associazione è nullo, utilizzo il codice della vaccinazione
						// Il codice della vaccinazione non lo controllo perchè deve esistere!
						_cod = dta.Rows[i][CAMPO_CODICE_VACCINAZIONE].ToString();
					}
					else
					{
						_cod = dta.Rows[i][CAMPO_CODICE_ASSOCIAZIONE].ToString();
					}
				}
				else
				{
					_cod = dta.Rows[i][CAMPO_CODICE_CICLO].ToString();
				}

				// Data convocazione
				if (!dta.Rows[i].Table.Columns.Contains(CAMPO_DATA_CONVOCAZIONE) || dta.Rows[i][CAMPO_DATA_CONVOCAZIONE] is System.DBNull)
				{
					_data_cnv = DateTime.MinValue;
				}
				else
				{
					_data_cnv = (DateTime) dta.Rows[i][CAMPO_DATA_CONVOCAZIONE];
				}

				// Seduta
				if (!dta.Rows[i].Table.Columns.Contains(CAMPO_NUMERO_SEDUTA) || dta.Rows[i][CAMPO_NUMERO_SEDUTA] is System.DBNull)
				{
					// Se il numero seduta è nullo, utilizzo il numero richiamo (per avere un codice univoco)
					if (!dta.Rows[i].Table.Columns.Contains(CAMPO_NUMERO_RICHIAMO) || dta.Rows[i][CAMPO_NUMERO_RICHIAMO] is System.DBNull)
					{
						_num_sed = 0;
					}
					else
					{
						_num_sed = Convert.ToInt32(dta.Rows[i][CAMPO_NUMERO_RICHIAMO]);
					}					
				}
				else
				{
					_num_sed = Convert.ToInt32(dta.Rows[i][CAMPO_NUMERO_SEDUTA]);
				}

				// Intervallo
				if (!dta.Rows[i].Table.Columns.Contains(CAMPO_INTERVALLO) || dta.Rows[i][CAMPO_INTERVALLO] is System.DBNull)
				{
					_interv = 0;
				}
				else
				{
					_interv = Convert.ToInt32(dta.Rows[i][CAMPO_INTERVALLO]);
				}                


				if (_cod == string.Empty)
				{
					// Codice convocazione nullo (questo caso non dovrebbe mai accadere)
					// Creo un nuovo elemento, che verrà aggiunto alla collection
					_cnv = newCnv(_data_cnv, _cod, _num_sed, _interv);
					_to_add = true;
				}
				else
				{
					// Codice convocazione non nullo
					// Controllo se la convocazione è già nella collection oppure no
					_cnv = _coll_cnv.FindByCodCnvSeduta(_cod, _num_sed.ToString());
					if (_cnv == null)
					{
						// La convocazione non è presente nella collection.
						// Creo un nuovo elemento che inserirò nella collection
						_cnv = newCnv(_data_cnv, _cod, _num_sed, _interv);
						_to_add = true;
					}
					else
					{
						// La convocazione è già presente.
						// La convocazione non è da inserire perchè c'è già. Ad essa aggiungerò solo la vaccinazione.
						_to_add = false;
					}


					// Aggiungo la vaccinazione all'elenco delle vaccinazioni della convocazione
					_cnv.ElencoVaccinazioni.Add(dta.Rows[i][CAMPO_CODICE_VACCINAZIONE].ToString());

					// Aggiunta alla collection
					if (_to_add) _coll_cnv.Add(_cnv);
				}
			}
			
			return _coll_cnv;
		}


		// Creazione nuovo elemento che rappresenta la convocazione
		private ConvocazioniClass newCnv(DateTime data_cnv, string cod_cnv, int num_seduta, int intervallo)
		{
			ConvocazioniClass _cnv = new ConvocazioniClass();
            
			_cnv.DataConvocazione = data_cnv;
			_cnv.CodiceConvocazione = cod_cnv;
			_cnv.NumeroSeduta = num_seduta;
			_cnv.Intervallo = intervallo;
            
			return _cnv;
		}


		#endregion



		#region --- Metodi per il controllo di associabilità tra vaccinazioni ---
		

		/// <summary>
		/// Restituisce true se le due vaccinazioni specificate sono somministrabili insieme.
		/// </summary>
		/// <param name="cod_vac_da_accorpare"></param>
		/// <param name="cod_vac_riferimento"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool VaccinazioniAssociabili(string cod_vac_da_accorpare, string cod_vac_riferimento)
		{
			return checkVaccinazioniAssociabili(cod_vac_da_accorpare, cod_vac_riferimento);
		}


		/// <summary>
		/// Restituisce true se tutte le vaccinazioni specificate sono somministrabili insieme.
		/// </summary>
		/// <param name="lista_vac_da_accorpare"></param>
		/// <param name="lista_vac_riferimento"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool VaccinazioniAssociabili(ArrayList lista_vac_da_accorpare, ArrayList lista_vac_riferimento)
		{
			return checkVaccinazioniAssociabili(lista_vac_da_accorpare, lista_vac_riferimento);
		}


		// Controllo associabilità tra elenchi di vaccinazioni
		private bool checkVaccinazioniAssociabili(ArrayList lista_vac_da_accorpare, ArrayList lista_vac_riferimento)
		{
			// Per ogni vaccinazione da accorpare
			for (int idxAcc=0; idxAcc<lista_vac_da_accorpare.Count; idxAcc++)
			{
				// Per ogni vaccinazione di riferimento
				for (int idxRif=0; idxRif<lista_vac_riferimento.Count; idxRif++)
				{
					// Se le due vaccinazioni non sono associabili, il controllo restituisce false
					if ( !checkVaccinazioniAssociabili(lista_vac_da_accorpare[idxAcc].ToString(), lista_vac_riferimento[idxRif].ToString()) )
					{
						return false;
					}
				}
			}
			
			// Se tutte le coppie di vaccinazioni sono associabili, restituisce true
			return true;
		}


		// Controllo associabilità di due vaccinazioni
		// Effettua la chiamata al metodo del DAL che cerca la coppia di vaccinazioni nella tabella delle non associabilità.
		private bool checkVaccinazioniAssociabili(string cod_vac_da_accorpare, string cod_vac_riferimento)
		{
			bool _associabili = true;

			DAL.DbGenericProvider genericprovider = null;
			try
			{
				genericprovider = new DAL.DbGenericProvider(_provider, ref _conn, ref _tx);
				_associabili = genericprovider.AssociabilitaVaccinazioni.VaccinazioniAssociabili(cod_vac_da_accorpare, cod_vac_riferimento);
			}
			finally
			{
				if (genericprovider != null) genericprovider.Dispose();
			}

			return _associabili;
		}


		#endregion



	}


}

