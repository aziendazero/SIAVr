namespace Onit.OnAssistnet.OnVac.Queries.AssociabilitaVaccinazioni
{
	/// <summary>
	/// Query oracle relative all'associabilit� dei vaccini.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// La query restituir� 1 se la coppia di vaccinazioni � presente nella tabella delle non associabilit�
        /// </summary>
		public static string chkVacAssociabili
		{
			get 
			{
				return @"select 1 
from t_ana_vac_non_associabili 
where vna_vac_codice = :cod_vac1 
and vna_vac_codice_non_associabile = :cod_vac2";
			}
		}

	}
}
