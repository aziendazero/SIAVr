namespace Onit.OnAssistnet.OnVac.Queries.MovimentiEsterniCNS
{
    /// <summary>
    /// Queries movimenti
    /// </summary>
    public static class OracleQueries
    {

        /// <summary>
        /// Restituisce i dati degli ultimi consensi del paziente, uno per ogni tipologia
        /// </summary>
        public static string selVacEseguitePaziente
        {
            get
            {
                return @"SELECT   VES_ID,
         VES_VAC_CODICE,
         VES_N_RICHIAMO,
         VES_DATA_EFFETTUAZIONE,
         VES_CNS_CODICE,
         VES_STATO,
         VES_CIC_CODICE,
         VES_N_SEDUTA,
         VES_DATA_REGISTRAZIONE,
         VES_LOT_CODICE,
         VES_OPE_CODICE,
         VES_UTE_ID,
         VES_SII_CODICE,
         VES_NOC_CODICE,
         VES_ASS_CODICE,
         VES_LUOGO,
         VES_PAZ_CODICE_OLD,
         VES_COMUNE_O_STATO,
         VES_MED_VACCINANTE,
         VES_CNV_DATA,
         VES_CNV_DATA_PRIMO_APP,
         VES_IN_CAMPAGNA,
         VES_OPE_IN_AMBULATORIO,
         VES_ESITO,
         VES_FLAG_FITTIZIA,
         VES_NOTE,
         VES_CNS_REGISTRAZIONE,
         VES_ACCESSO,
         VES_AMB_CODICE,
         VES_VII_CODICE,
         VES_DATAORA_EFFETTUAZIONE,
         VES_MAL_CODICE_MALATTIA,
         VES_CODICE_ESENZIONE,
         VES_IMPORTO,
         'N' VES_SCADUTA,
         VES_ASS_N_DOSE,
         MED_DESCRIZIONE,
         OPE_NOME
  FROM   T_VAC_ESEGUITE
    LEFT JOIN T_ANA_MEDICI ON VES_MED_VACCINANTE = MED_CODICE
    LEFT JOIN T_ANA_OPERATORI ON VES_OPE_CODICE = OPE_CODICE
 WHERE   VES_PAZ_CODICE = :codicePaziente
UNION
SELECT   VSC_ID VES_ID,
         VSC_VAC_CODICE VES_VAC_CODICE,
         VSC_N_RICHIAMO VES_N_RICHIAMO,
         VSC_DATA_EFFETTUAZIONE VES_DATA_EFFETTUAZIONE,
         VSC_CNS_CODICE VES_CNS_CODICE,
         VSC_STATO VES_STATO,
         VSC_CIC_CODICE VES_CIC_CODICE,
         VSC_N_SEDUTA VES_N_SEDUTA,
         VSC_DATA_REGISTRAZIONE VES_DATA_REGISTRAZIONE,
         VSC_LOT_CODICE VES_LOT_CODICE,
         VSC_OPE_CODICE VES_OPE_CODICE,
         VSC_UTE_ID VES_UTE_ID,
         VSC_SII_CODICE VES_SII_CODICE,
         VSC_NOC_CODICE VES_NOC_CODICE,
         VSC_ASS_CODICE VES_ASS_CODICE,
         VSC_LUOGO VES_LUOGO,
         VSC_PAZ_CODICE_OLD VES_PAZ_CODICE_OLD,
         VSC_COMUNE_O_STATO VES_COMUNE_O_STATO,
         VSC_MED_VACCINANTE VES_MED_VACCINANTE,
         VSC_CNV_DATA VES_CNV_DATA,
         VSC_CNV_DATA_PRIMO_APP VES_CNV_DATA_PRIMO_APP,
         VSC_IN_CAMPAGNA VES_IN_CAMPAGNA,
         VSC_OPE_IN_AMBULATORIO VES_OPE_IN_AMBULATORIO,
         VSC_ESITO VES_ESITO,
         VSC_FLAG_FITTIZIA VES_FLAG_FITTIZIA,
         VSC_NOTE VES_NOTE,
         VSC_CNS_REGISTRAZIONE VES_CNS_REGISTRAZIONE,
         VSC_ACCESSO VES_ACCESSO,
         VSC_AMB_CODICE VES_AMB_CODICE,
         VSC_VII_CODICE VES_VII_CODICE,
         VSC_DATAORA_EFFETTUAZIONE VES_DATAORA_EFFETTUAZIONE,
         VSC_MAL_CODICE_MALATTIA VES_MAL_CODICE_MALATTIA,
         VSC_CODICE_ESENZIONE VES_CODICE_ESENZIONE,
         VSC_IMPORTO VES_IMPORTO,
         'S' VES_SCADUTA,
         VSC_ASS_N_DOSE VES_ASS_N_DOSE,
         null MED_DESCRIZIONE,
         null OPE_NOME
  FROM   T_VAC_SCADUTE
 WHERE   VSC_PAZ_CODICE = :codicePaziente";
            }
        }

        /// <summary>
        /// Restituisce i dati degli ultimi consensi del paziente, uno per ogni tipologia
        /// </summary>
        public static string insVacScaduta
        {
            get
            {
                return @"INSERT INTO T_VAC_SCADUTE (VSC_ID,
                           VSC_PAZ_CODICE,
                           VSC_PAZ_CODICE_OLD,
                           VSC_VAC_CODICE,
                           VSC_N_RICHIAMO,
                           VSC_DATA_EFFETTUAZIONE,
                           VSC_DATAORA_EFFETTUAZIONE,
                           VSC_CNS_CODICE,
                           VSC_STATO,
                           VSC_CIC_CODICE,
                           VSC_N_SEDUTA,
                           VSC_DATA_REGISTRAZIONE,
                           VSC_LOT_CODICE,
                           VSC_OPE_CODICE,
                           VSC_UTE_ID,
                           VSC_SII_CODICE,
                           VSC_NOC_CODICE,
                           VSC_ASS_CODICE,
                           VSC_LUOGO,
                           VSC_COMUNE_O_STATO,
                           VSC_MED_VACCINANTE,
                           VSC_CNV_DATA,
                           VSC_CNV_DATA_PRIMO_APP,
                           VSC_IN_CAMPAGNA,
                           VSC_OPE_IN_AMBULATORIO,
                           VSC_ESITO,
                           VSC_FLAG_FITTIZIA,
                           VSC_NOTE,
                           VSC_CNS_REGISTRAZIONE,
                           VSC_ACCESSO,
                           VSC_AMB_CODICE,
                           VSC_VII_CODICE,
                           VSC_MAL_CODICE_MALATTIA,
                           VSC_CODICE_ESENZIONE,
                           VSC_IMPORTO,
                           VSC_ASS_N_DOSE)
  VALUES   (:id,
            :codicePaziente,
            :codicePazientePrecedente,
            :codiceVaccinazione,
            :numeroRichiamo,
            :dataEffettuazione,
            :dataOraEffettuazione,
            :codiceConsultorio,
            :stato,
            :codiceCiclo,
            :numeroSeduta,
            :dataRegistrazione,
            :codiceLotto,
            :codiceOperatore,
            :idUtente,
            :codiceSitoInoculazione,
            :codiceNomeCommerciale,
            :codiceAssociazione,
            :luogo,
            :codiceComune,
            :codiceMedicoVaccinante,
            :dataConvocazione,
            :dataPrimoAppConvocazione,
            :inCampagna,
            :operatoreInAmbulatorio,
            :esito,
            :fittizia,
            :note,
            :codiceConsultorioRegistrazione,
            :accesso,
            :codiceAmbulatorio,
            :codiceViaSomministrazione,
            :codiceMalattia,
            :codiceEsenzione,
            :importo,
            :ass_n_dose)";
            }
        }

        /// <summary>
        /// Stati del consenso in anagrafica
        /// </summary>
        public static string insVacEseguita
        {
            get
            {
                return @"INSERT INTO T_VAC_ESEGUITE (VES_ID,
                            VES_PAZ_CODICE,
                            VES_PAZ_CODICE_OLD,
                            VES_VAC_CODICE,
                            VES_N_RICHIAMO,
                            VES_DATA_EFFETTUAZIONE,
                            VES_DATAORA_EFFETTUAZIONE,
                            VES_CNS_CODICE,
                            VES_STATO,
                            VES_CIC_CODICE,
                            VES_N_SEDUTA,
                            VES_DATA_REGISTRAZIONE,
                            VES_LOT_CODICE,
                            VES_OPE_CODICE,
                            VES_UTE_ID,
                            VES_SII_CODICE,
                            VES_NOC_CODICE,
                            VES_ASS_CODICE,
                            VES_LUOGO,
                            VES_COMUNE_O_STATO,
                            VES_MED_VACCINANTE,
                            VES_CNV_DATA,
                            VES_CNV_DATA_PRIMO_APP,
                            VES_IN_CAMPAGNA,
                            VES_OPE_IN_AMBULATORIO,
                            VES_ESITO,
                            VES_FLAG_FITTIZIA,
                            VES_NOTE,
                            VES_CNS_REGISTRAZIONE,
                            VES_ACCESSO,
                            VES_AMB_CODICE,
                            VES_VII_CODICE,
                            VES_MAL_CODICE_MALATTIA,
                            VES_CODICE_ESENZIONE,
                            VES_IMPORTO,
                            VES_ASS_N_DOSE)
  VALUES   (:id,
            :codicePaziente,
            :codicePazientePrecedente,
            :codiceVaccinazione,
            :numeroRichiamo,
            :dataEffettuazione,
            :dataOraEffettuazione,
            :codiceConsultorio,
            :stato,
            :codiceCiclo,
            :numeroSeduta,
            :dataRegistrazione,
            :codiceLotto,
            :codiceOperatore,
            :idUtente,
            :codiceSitoInoculazione,
            :codiceNomeCommerciale,
            :codiceAssociazione,
            :luogo,
            :codiceComune,
            :codiceMedicoVaccinante,
            :dataConvocazione,
            :dataPrimoAppConvocazione,
            :inCampagna,
            :operatoreInAmbulatorio,
            :esito,
            :fittizia,
            :note,
            :codiceConsultorioRegistrazione,
            :accesso,
            :codiceAmbulatorio,
            :codiceViaSomministrazione,
            :codiceMalattia,
            :codiceEsenzione,
            :importo,
            :ass_n_dose)";
            }
        }

    }
}
