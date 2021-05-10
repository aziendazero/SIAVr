namespace Onit.OnAssistnet.OnVac.Queries.AssociabilitaVaccinazioni
{
	/// <summary>
	/// Query oracle relative all'associabilità dei vaccini.
	/// </summary>
	public static class OracleQueries
	{

        /// <summary>
        /// La query restituirà 1 se la coppia di vaccinazioni è presente nella tabella delle non associabilità
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
