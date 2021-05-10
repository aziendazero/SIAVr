using Onit.OnAssistnet.OnVac.Entities;
using Onit.OnAssistnet.OnVac.Log.DataLogStructure;
using System;

namespace Onit.OnAssistnet.OnVac.Log.DataLogManager
{
    public static class PazienteLogManager
    {
        public static Record GetLogPaziente(Paziente pazienteModificato, Paziente pazienteOriginale)
        {
            Record recordLog = new Record();

            if (pazienteOriginale != null)
            {
                #region Dichiarazione campi

                string codiceAusiliarioOriginale = null;
                string codiceAusiliarioCorrente = null;
                string cognomeOriginale = null;
                string cognomeCorrente = null;
                string nomeOriginale = null;
                string nomeCorrente = null;
                string codiceFiscaleOriginale = null;
                string codiceFiscaleCorrente = null;
                DateTime? dataNascitaOriginale = null;
                DateTime? dataNascitaCorrente = null;
                string codiceComuneNascitaOriginale = null;
                string codiceComuneNascitaCorrente = null;
                string sessoOriginale = null;
                string sessoCorrente = null;
                string tesseraOriginale = null;
                string tesseraCorrente = null;
                string codiceComuneResidenzaOriginale = null;
                string codiceComuneResidenzaCorrente = null;
                string indirizzoResidenzaOriginale = null;
                string indirizzoResidenzaCorrente = null;
                string capResidenzaOriginale = null;
                string capResidenzaCorrente = null;
                string codiceComuneDomicilioOriginale = null;
                string codiceComuneDomicilioCorrente = null;
                string indirizzoDomicilioOriginale = null;
                string indirizzoDomicilioCorrente = null;
                string capDomicilioOriginale = null;
                string capDomicilioCorrente = null;
                string telefono1Originale = null;
                string telefono1Corrente = null;
                string telefono2Originale = null;
                string telefono2Corrente = null;
                string telefono3Originale = null;
                string telefono3Corrente = null;
                string emailOriginale = null;
                string emailCorrente = null;
                string codiceUslResidenzaOriginale = null;
                string codiceUslResidenzaCorrente = null;
                string codiceUslAssistenzaOriginale = null;
                string codiceUslAssistenzaCorrente = null;
                string codiceCittadinanzaOriginale = null;
                string codiceCittadinanzaCorrente = null;
                string codiceMedicoBaseOriginale = null;
                string codiceMedicoBaseCorrente = null;
                DateTime? dataDecorrenzaMedicoOriginale = null;
                DateTime? dataDecorrenzaMedicoCorrente = null;
                DateTime? dataScadenzaMedicoOriginale = null;
                DateTime? dataScadenzaMedicoCorrente = null; 
                string flagCessatoOriginale = null;
                string flagCessatoCorrente = null;
                string flagAireOriginale = null;
                string flagAireCorrente = null;
                string flagLocaleOriginale = null;
                string flagLocaleCorrente = null;
                string flagRegolarizzatoOriginale = null;
                string flagRegolarizzatoCorrente = null;
                string codiceCircoscrizioneOriginale = null;
                string codiceCircoscrizioneCorrente = null;
                DateTime? dataCNSAssegnazioneOriginale = null;
                DateTime? dataCNSAssegnazioneCorrente = null;
                string codiceCNSOriginale = null;
                string codiceCNSCorrente = null;
                string codiceCNSOldOriginale = null;
                string codiceCNSOldCorrente = null;
                string codiceCNSTerritorialeOriginale = null;
                string codiceCNSTerritorialeCorrente = null;
                Enumerators.StatiVaccinali? statoOriginale = null;
                Enumerators.StatiVaccinali? statoCorrente = null;
                Enumerators.StatoAnagrafico? statoAnagraficoOriginale = null;
                Enumerators.StatoAnagrafico? statoAnagraficoCorrente = null;
                DateTime? dataAggiornamentoDaAnagrafeOriginale = null;
                DateTime? dataAggiornamentoDaAnagrafeCorrente = null;
                DateTime? dataImmigrazioneOriginale = null;
                DateTime? dataImmigrazioneCorrente = null;
                string codiceComuneProvenienzaOriginale = null;
                string codiceComuneProvenienzaCorrente = null;
                DateTime? dataEmigrazioneOriginale = null;
                DateTime? dataEmigrazioneCorrente = null;
                string codiceComuneEmigrazioneOriginale = null;
                string codiceComuneEmigrazioneCorrente = null;
                DateTime? dataInserimentoOriginale = null;
                DateTime? dataInserimentoCorrente = null;
                Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? statoAcquisizioneDatiVaccinaliCentraleOriginale = null;
                Enumerators.StatoAcquisizioneDatiVaccinaliCentrale? statoAcquisizioneDatiVaccinaliCentraleCorrente = null;
                string idACNOriginale = null;
                string idACNCorrente = null;
                string categoriaCittadinoOriginale = null;
                string categoriaCittadinoCorrente = null;
                string motivoCessazioneAssistenzaOriginale = null;
                string motivoCessazioneAssistenzaCorrente = null;

                #endregion

                #region Valorizzazione campi originali

                if (!string.IsNullOrEmpty(pazienteOriginale.CodiceAusiliario))
                {
                    codiceAusiliarioOriginale = pazienteOriginale.CodiceAusiliario;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.PAZ_COGNOME))
                {
                    cognomeOriginale = pazienteOriginale.PAZ_COGNOME;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.PAZ_NOME))
                {
                    nomeOriginale = pazienteOriginale.PAZ_NOME;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.PAZ_CODICE_FISCALE))
                {
                    codiceFiscaleOriginale = pazienteOriginale.PAZ_CODICE_FISCALE;
                }
                if ((pazienteOriginale.Data_Nascita != DateTime.MinValue))
                {
                    dataNascitaOriginale = pazienteOriginale.Data_Nascita;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.ComuneNascita_Codice))
                {
                    codiceComuneNascitaOriginale = pazienteOriginale.ComuneNascita_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Sesso))
                {
                    sessoOriginale = pazienteOriginale.Sesso;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Tessera))
                {
                    tesseraOriginale = pazienteOriginale.Tessera;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.ComuneResidenza_Codice))
                {
                    codiceComuneResidenzaOriginale = pazienteOriginale.ComuneResidenza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.IndirizzoResidenza))
                {
                    indirizzoResidenzaOriginale = pazienteOriginale.IndirizzoResidenza;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.ComuneResidenza_Cap))
                {
                    capResidenzaOriginale = pazienteOriginale.ComuneResidenza_Cap;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.ComuneDomicilio_Codice))
                {
                    codiceComuneDomicilioOriginale = pazienteOriginale.ComuneDomicilio_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.IndirizzoResidenza))
                {
                    indirizzoDomicilioOriginale = pazienteOriginale.IndirizzoDomicilio;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.ComuneDomicilio_Cap))
                {
                    capDomicilioOriginale = pazienteOriginale.ComuneDomicilio_Cap;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Telefono1))
                {
                    telefono1Originale = pazienteOriginale.Telefono1;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Telefono2))
                {
                    telefono2Originale = pazienteOriginale.Telefono2;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Telefono3))
                {
                    telefono3Originale = pazienteOriginale.Telefono3;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Email))
                {
                    emailOriginale = pazienteOriginale.Email;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.UslResidenza_Codice))
                {
                    codiceUslResidenzaOriginale = pazienteOriginale.UslResidenza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.UslAssistenza_Codice))
                {
                    codiceUslAssistenzaOriginale = pazienteOriginale.UslAssistenza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Cittadinanza_Codice))
                {
                    codiceCittadinanzaOriginale = pazienteOriginale.Cittadinanza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.MedicoBase_Codice))
                {
                    codiceMedicoBaseOriginale = pazienteOriginale.MedicoBase_Codice;
                }
                if ((pazienteOriginale.MedicoBase_DataDecorrenza != DateTime.MinValue))
                {
                    dataDecorrenzaMedicoOriginale = pazienteOriginale.MedicoBase_DataDecorrenza;
                }
                if ((pazienteOriginale.MedicoBase_DataScadenza != DateTime.MinValue))
                {
                    dataScadenzaMedicoOriginale = pazienteOriginale.MedicoBase_DataScadenza;
                }               
                if (!string.IsNullOrEmpty(pazienteOriginale.FlagCessato))
                {
                    flagCessatoOriginale = pazienteOriginale.FlagCessato;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.FlagAire))
                {
                    flagAireOriginale = pazienteOriginale.FlagAire;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.FlagLocale))
                {
                    flagLocaleOriginale = pazienteOriginale.FlagLocale;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.FlagRegolarizzato))
                {
                    flagRegolarizzatoOriginale = pazienteOriginale.FlagRegolarizzato;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Circoscrizione_Codice))
                {
                    codiceCircoscrizioneOriginale = pazienteOriginale.Circoscrizione_Codice;
                }
                if (pazienteOriginale.Paz_Cns_Data_Assegnazione.HasValue)
                {
                    dataCNSAssegnazioneOriginale = pazienteOriginale.Paz_Cns_Data_Assegnazione.Value;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Paz_Cns_Codice))
                {
                    codiceCNSOriginale = pazienteOriginale.Paz_Cns_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Paz_Cns_Codice_Old))
                {
                    codiceCNSOldOriginale = pazienteOriginale.Paz_Cns_Codice_Old;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.Paz_Cns_Terr_Codice))
                {
                    codiceCNSTerritorialeOriginale = pazienteOriginale.Paz_Cns_Terr_Codice;
                }
                if (pazienteOriginale.Stato.HasValue)
                {
                    statoOriginale = pazienteOriginale.Stato;
                }
                if (pazienteOriginale.StatoAnagrafico.HasValue)
                {
                    statoAnagraficoOriginale = pazienteOriginale.StatoAnagrafico;
                }
                if (pazienteOriginale.DataAggiornamentoDaAnagrafe.HasValue)
                {
                    dataAggiornamentoDaAnagrafeOriginale = pazienteOriginale.DataAggiornamentoDaAnagrafe.Value;
                }
                if ((pazienteOriginale.DataImmigrazione != DateTime.MinValue))
                {
                    dataImmigrazioneOriginale = pazienteOriginale.DataImmigrazione;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.ComuneProvenienza_Codice))
                {
                    codiceComuneProvenienzaOriginale = pazienteOriginale.ComuneProvenienza_Codice;
                }
                if ((pazienteOriginale.DataEmigrazione != DateTime.MinValue))
                {
                    dataEmigrazioneOriginale = pazienteOriginale.DataEmigrazione;
                }
                if (!string.IsNullOrEmpty(pazienteOriginale.ComuneEmigrazione_Codice))
                {
                    codiceComuneEmigrazioneOriginale = pazienteOriginale.ComuneEmigrazione_Codice;
                }
                if (pazienteOriginale.DataInserimento != DateTime.MinValue)
                {
                    dataInserimentoOriginale = pazienteOriginale.DataInserimento;
                }
                statoAcquisizioneDatiVaccinaliCentraleOriginale = pazienteOriginale.StatoAcquisizioneDatiVaccinaliCentrale;
                idACNOriginale = pazienteOriginale.IdACN;
                categoriaCittadinoOriginale = pazienteOriginale.CategoriaCittadino;
                motivoCessazioneAssistenzaOriginale = pazienteOriginale.MotivoCessazioneAssistenza;

                #endregion

                #region Valorizzazione campi correnti

                if (!string.IsNullOrEmpty(pazienteModificato.CodiceAusiliario))
                {
                    codiceAusiliarioCorrente = pazienteModificato.CodiceAusiliario;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.PAZ_COGNOME))
                {
                    cognomeCorrente = pazienteModificato.PAZ_COGNOME;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.PAZ_NOME))
                {
                    nomeCorrente = pazienteModificato.PAZ_NOME;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.PAZ_CODICE_FISCALE))
                {
                    codiceFiscaleCorrente = pazienteModificato.PAZ_CODICE_FISCALE;
                }
                if ((pazienteModificato.Data_Nascita != DateTime.MinValue))
                {
                    dataNascitaCorrente = pazienteModificato.Data_Nascita;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.ComuneNascita_Codice))
                {
                    codiceComuneNascitaCorrente = pazienteModificato.ComuneNascita_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Sesso))
                {
                    sessoCorrente = pazienteModificato.Sesso;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Tessera))
                {
                    tesseraCorrente = pazienteModificato.Tessera;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.ComuneResidenza_Codice))
                {
                    codiceComuneResidenzaCorrente = pazienteModificato.ComuneResidenza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.IndirizzoResidenza))
                {
                    indirizzoResidenzaCorrente = pazienteModificato.IndirizzoResidenza;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.ComuneResidenza_Cap))
                {
                    capResidenzaCorrente = pazienteModificato.ComuneResidenza_Cap;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.ComuneDomicilio_Codice))
                {
                    codiceComuneDomicilioCorrente = pazienteModificato.ComuneDomicilio_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.IndirizzoResidenza))
                {
                    indirizzoDomicilioCorrente = pazienteModificato.IndirizzoDomicilio;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.ComuneDomicilio_Cap))
                {
                    capDomicilioCorrente = pazienteModificato.ComuneDomicilio_Cap;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Telefono1))
                {
                    telefono1Corrente = pazienteModificato.Telefono1;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Telefono2))
                {
                    telefono2Corrente = pazienteModificato.Telefono2;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Telefono3))
                {
                    telefono3Corrente = pazienteModificato.Telefono3;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Email))
                {
                    emailCorrente = pazienteModificato.Email;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.UslResidenza_Codice))
                {
                    codiceUslResidenzaCorrente = pazienteModificato.UslResidenza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.UslAssistenza_Codice))
                {
                    codiceUslAssistenzaCorrente = pazienteModificato.UslAssistenza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Cittadinanza_Codice))
                {
                    codiceCittadinanzaCorrente = pazienteModificato.Cittadinanza_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.MedicoBase_Codice))
                {
                    codiceMedicoBaseCorrente = pazienteModificato.MedicoBase_Codice;
                }
                if ((pazienteModificato.MedicoBase_DataDecorrenza != DateTime.MinValue))
                {
                    dataDecorrenzaMedicoCorrente = pazienteModificato.MedicoBase_DataDecorrenza;
                }
                if ((pazienteModificato.MedicoBase_DataScadenza != DateTime.MinValue))
                {
                    dataScadenzaMedicoCorrente = pazienteModificato.MedicoBase_DataScadenza;
                }   
                if (!string.IsNullOrEmpty(pazienteModificato.FlagCessato))
                {
                    flagCessatoCorrente = pazienteModificato.FlagCessato;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.FlagAire))
                {
                    flagAireCorrente = pazienteModificato.FlagAire;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.FlagLocale))
                {
                    flagLocaleCorrente = pazienteModificato.FlagLocale;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.FlagRegolarizzato))
                {
                    flagRegolarizzatoCorrente = pazienteModificato.FlagRegolarizzato;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Circoscrizione_Codice))
                {
                    codiceCircoscrizioneCorrente = pazienteModificato.Circoscrizione_Codice;
                }
                if (pazienteModificato.Paz_Cns_Data_Assegnazione.HasValue)
                {
                    dataCNSAssegnazioneCorrente = pazienteModificato.Paz_Cns_Data_Assegnazione.Value;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Paz_Cns_Codice))
                {
                    codiceCNSCorrente = pazienteModificato.Paz_Cns_Codice;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Paz_Cns_Codice_Old))
                {
                    codiceCNSOldCorrente = pazienteModificato.Paz_Cns_Codice_Old;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.Paz_Cns_Terr_Codice))
                {
                    codiceCNSTerritorialeCorrente = pazienteModificato.Paz_Cns_Terr_Codice;
                }
                if (pazienteModificato.Stato.HasValue)
                {
                    statoCorrente = pazienteModificato.Stato;
                }
                if (pazienteModificato.StatoAnagrafico.HasValue)
                {
                    statoAnagraficoCorrente = pazienteModificato.StatoAnagrafico;
                }
                if (pazienteModificato.DataAggiornamentoDaAnagrafe.HasValue)
                {
                    dataAggiornamentoDaAnagrafeCorrente = pazienteModificato.DataAggiornamentoDaAnagrafe.Value;
                }
                if ((pazienteModificato.DataImmigrazione != DateTime.MinValue))
                {
                    dataImmigrazioneCorrente = pazienteModificato.DataImmigrazione;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.ComuneProvenienza_Codice))
                {
                    codiceComuneProvenienzaCorrente = pazienteModificato.ComuneProvenienza_Codice;
                }
                if ((pazienteModificato.DataEmigrazione != DateTime.MinValue))
                {
                    dataEmigrazioneCorrente = pazienteModificato.DataEmigrazione;
                }
                if (!string.IsNullOrEmpty(pazienteModificato.ComuneEmigrazione_Codice))
                {
                    codiceComuneEmigrazioneCorrente = pazienteModificato.ComuneEmigrazione_Codice;
                }
                dataInserimentoCorrente = pazienteModificato.DataInserimento;                
                statoAcquisizioneDatiVaccinaliCentraleCorrente = pazienteModificato.StatoAcquisizioneDatiVaccinaliCentrale;
                idACNCorrente = pazienteModificato.IdACN;
                categoriaCittadinoCorrente = pazienteModificato.CategoriaCittadino;
                motivoCessazioneAssistenzaCorrente = pazienteModificato.MotivoCessazioneAssistenza;

                #endregion

                #region Aggiunta campi al recordLog

                if (pazienteOriginale == null || codiceAusiliarioOriginale != codiceAusiliarioCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CODICE_AUSILIARIO", codiceAusiliarioOriginale, codiceAusiliarioCorrente));
                }
                if (pazienteOriginale == null || cognomeOriginale != cognomeCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_COGNOME", cognomeOriginale, cognomeCorrente));
                }
                if (pazienteOriginale == null || nomeOriginale != nomeCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_NOME", nomeOriginale, nomeCorrente));
                }
                if (pazienteOriginale == null || codiceFiscaleOriginale != codiceFiscaleCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CODICE_FISCALE", codiceFiscaleOriginale, codiceFiscaleCorrente));
                }
                if (pazienteOriginale == null || dataNascitaOriginale != dataNascitaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_DATA_NASCITA", dataNascitaOriginale.HasValue ? dataNascitaOriginale.Value.ToString() : string.Empty, dataNascitaCorrente.HasValue ? dataNascitaCorrente.Value.ToString() : string.Empty));
                }
                if (pazienteOriginale == null || codiceComuneNascitaOriginale != codiceComuneNascitaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_COM_CODICE_NASCITA", codiceComuneNascitaOriginale, codiceComuneNascitaCorrente));
                }
                if (pazienteOriginale == null || sessoOriginale != sessoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_SESSO", sessoOriginale, sessoCorrente));
                }
                if (pazienteOriginale == null || tesseraOriginale != tesseraCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_TESSERA", tesseraOriginale, tesseraCorrente));
                }
                if (pazienteOriginale == null || codiceComuneResidenzaOriginale != codiceComuneResidenzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_COM_CODICE_RESIDENZA", codiceComuneResidenzaOriginale, codiceComuneResidenzaCorrente));
                }
                if (pazienteOriginale == null || indirizzoResidenzaOriginale != indirizzoResidenzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_INDIRIZZO_RESIDENZA", indirizzoResidenzaOriginale, indirizzoResidenzaCorrente));
                }
                if (pazienteOriginale == null || capResidenzaOriginale != capResidenzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CAP_RESIDENZA", capResidenzaOriginale, capResidenzaCorrente));
                }
                if (pazienteOriginale == null || codiceComuneDomicilioOriginale != codiceComuneDomicilioCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_COM_CODICE_DOMICILIO", codiceComuneDomicilioOriginale, codiceComuneDomicilioCorrente));
                }
                if (pazienteOriginale == null || indirizzoDomicilioOriginale != indirizzoDomicilioCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_INDIRIZZO_DOMICILIO", indirizzoDomicilioOriginale, indirizzoDomicilioCorrente));
                }
                if (pazienteOriginale == null || capDomicilioOriginale != capDomicilioCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CAP_DOMICILIO", capDomicilioOriginale, capDomicilioCorrente));
                }
                if (pazienteOriginale == null || telefono1Originale != telefono1Corrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_TELEFONO_1", telefono1Originale, telefono1Corrente));
                }
                if (pazienteOriginale == null || telefono2Originale != telefono2Corrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_TELEFONO_2", telefono2Originale, telefono2Corrente));
                }
                if (pazienteOriginale == null || telefono3Originale != telefono3Corrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_TELEFONO_3", telefono3Originale, telefono3Corrente));
                }
                if (pazienteOriginale == null || emailOriginale != emailCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_EMAIL", emailOriginale, emailCorrente));
                }
                if (pazienteOriginale == null || codiceUslResidenzaOriginale != codiceUslResidenzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_USL_CODICE_RESIDENZA", codiceUslResidenzaOriginale, codiceUslResidenzaCorrente));
                }
                if (pazienteOriginale == null || codiceUslAssistenzaOriginale != codiceUslAssistenzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_USL_CODICE_ASSISTENZA", codiceUslAssistenzaOriginale, codiceUslAssistenzaCorrente));
                }
                if (pazienteOriginale == null || codiceCittadinanzaOriginale != codiceCittadinanzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CIT_CODICE", codiceCittadinanzaOriginale, codiceCittadinanzaCorrente));
                }
                if (pazienteOriginale == null || codiceMedicoBaseOriginale != codiceMedicoBaseCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_MED_CODICE_BASE", codiceMedicoBaseOriginale, codiceMedicoBaseCorrente));
                }
                if (pazienteOriginale == null || dataDecorrenzaMedicoOriginale != dataDecorrenzaMedicoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_DATA_DECORRENZA_MED", (dataDecorrenzaMedicoOriginale.HasValue ? dataDecorrenzaMedicoOriginale.Value.ToString() : string.Empty), (dataDecorrenzaMedicoCorrente.HasValue ? dataDecorrenzaMedicoCorrente.Value.ToString() : string.Empty)));
                }
                if (pazienteOriginale == null || dataScadenzaMedicoOriginale != dataScadenzaMedicoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_DATA_SCADENZA_MED", (dataScadenzaMedicoOriginale.HasValue ? dataScadenzaMedicoOriginale.Value.ToString() : string.Empty), (dataScadenzaMedicoCorrente.HasValue ? dataScadenzaMedicoCorrente.Value.ToString() : string.Empty)));
                }  
                if (pazienteOriginale == null || flagCessatoOriginale != flagCessatoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_FLAG_CESSATO", flagCessatoOriginale, flagCessatoCorrente));
                }
                if (pazienteOriginale == null || flagAireOriginale != flagAireCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_AIRE", flagAireOriginale, flagAireCorrente));
                }
                if (pazienteOriginale == null || flagLocaleOriginale != flagLocaleCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_LOCALE", flagLocaleOriginale, flagLocaleCorrente));
                }
                if (pazienteOriginale == null || flagRegolarizzatoOriginale != flagRegolarizzatoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_REGOLARIZZATO", flagRegolarizzatoOriginale, flagRegolarizzatoCorrente));
                }
                if (pazienteOriginale == null || codiceCircoscrizioneOriginale != codiceCircoscrizioneCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CIR_CODICE", codiceCircoscrizioneOriginale, codiceCircoscrizioneCorrente));
                }
                if (pazienteOriginale == null || dataCNSAssegnazioneOriginale != dataCNSAssegnazioneCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CNS_DATA_ASSEGNAZIONE", (dataCNSAssegnazioneOriginale.HasValue ? dataCNSAssegnazioneOriginale.Value.ToString() : string.Empty), (dataCNSAssegnazioneCorrente.HasValue ? dataCNSAssegnazioneCorrente.Value.ToString() : string.Empty)));
                }
                if (pazienteOriginale == null || codiceCNSOldOriginale != codiceCNSOldCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CNS_CODICE_OLD", codiceCNSOldOriginale, codiceCNSOldCorrente));
                }
                if (pazienteOriginale == null || codiceCNSOriginale != codiceCNSCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CNS_CODICE", codiceCNSOriginale, codiceCNSCorrente));
                }
                if (pazienteOriginale == null || codiceCNSTerritorialeOriginale != codiceCNSTerritorialeCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CNS_TERR_CODICE", codiceCNSTerritorialeOriginale, codiceCNSTerritorialeCorrente));
                }
                if (pazienteOriginale == null || statoOriginale != statoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_STATO", (statoOriginale.HasValue ? statoOriginale.Value.ToString("d") : string.Empty), (statoCorrente.HasValue ? statoCorrente.Value.ToString("d") : string.Empty)));
                }
                if (pazienteOriginale == null || statoAnagraficoOriginale != statoAnagraficoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_STATO_ANAGRAFICO", (statoAnagraficoOriginale.HasValue ? statoAnagraficoOriginale.Value.ToString("d") : string.Empty), (statoAnagraficoCorrente.HasValue ? statoAnagraficoCorrente.Value.ToString("d") : string.Empty)));
                }
                if (pazienteOriginale == null || dataAggiornamentoDaAnagrafeOriginale != dataAggiornamentoDaAnagrafeCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_DATA_AGG_DA_ANAG", (dataAggiornamentoDaAnagrafeOriginale.HasValue ? dataAggiornamentoDaAnagrafeOriginale.Value.ToString() : string.Empty), (dataAggiornamentoDaAnagrafeCorrente.HasValue ? dataAggiornamentoDaAnagrafeCorrente.Value.ToString() : string.Empty)));
                }
                if (pazienteOriginale == null || dataImmigrazioneOriginale != dataImmigrazioneCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_DATA_IMMIGRAZIONE", (dataImmigrazioneOriginale.HasValue ? dataImmigrazioneOriginale.Value.ToString() : string.Empty), (dataImmigrazioneCorrente.HasValue ? dataImmigrazioneCorrente.Value.ToString() : string.Empty)));
                }
                if (pazienteOriginale == null || codiceComuneProvenienzaOriginale != codiceComuneProvenienzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_COM_COMUNE_PROVENIENZA", codiceComuneProvenienzaOriginale, codiceComuneProvenienzaCorrente));
                }
                if (pazienteOriginale == null || dataEmigrazioneOriginale != dataEmigrazioneCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_DATA_EMIGRAZIONE", (dataEmigrazioneOriginale.HasValue ? dataEmigrazioneOriginale.Value.ToString() : string.Empty), (dataEmigrazioneCorrente.HasValue ? dataEmigrazioneCorrente.Value.ToString() : string.Empty)));
                }
                if (pazienteOriginale == null || codiceComuneEmigrazioneOriginale != codiceComuneEmigrazioneCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_COM_COMUNE_EMIGRAZIONE", codiceComuneEmigrazioneOriginale, codiceComuneEmigrazioneCorrente));
                }
                if (pazienteOriginale == null || dataInserimentoOriginale != dataInserimentoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_DATA_INSERIMENTO", (dataInserimentoOriginale.HasValue ? dataInserimentoOriginale.Value.ToString() : string.Empty), (dataInserimentoCorrente.HasValue ? dataInserimentoCorrente.Value.ToString() : string.Empty)));
                }
                if (pazienteOriginale == null || statoAcquisizioneDatiVaccinaliCentraleOriginale != statoAcquisizioneDatiVaccinaliCentraleCorrente)
                {
                    string statoAcquisizioneDatiVaccinaliCentraleOriginaleString = string.Empty;
                    if (statoAcquisizioneDatiVaccinaliCentraleOriginale.HasValue) statoAcquisizioneDatiVaccinaliCentraleOriginaleString = statoAcquisizioneDatiVaccinaliCentraleOriginale.Value.ToString("d");

                    string statoAcquisizioneDatiVaccinaliCentraleCorrenteString = string.Empty;
                    if (statoAcquisizioneDatiVaccinaliCentraleCorrente.HasValue) statoAcquisizioneDatiVaccinaliCentraleCorrenteString = statoAcquisizioneDatiVaccinaliCentraleCorrente.Value.ToString("d");

                    recordLog.Campi.Add(new Campo("PAZ_STATO_ACQUISIZIONE", statoAcquisizioneDatiVaccinaliCentraleOriginaleString, statoAcquisizioneDatiVaccinaliCentraleCorrenteString));
                }
                if (pazienteOriginale == null || idACNOriginale != idACNCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_ID_ACN", idACNOriginale, idACNCorrente));
                }
                if (pazienteOriginale == null || categoriaCittadinoOriginale != categoriaCittadinoCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_CATEGORIA_CITTADINO", categoriaCittadinoOriginale, categoriaCittadinoCorrente));
                }
                if (pazienteOriginale == null || motivoCessazioneAssistenzaOriginale != motivoCessazioneAssistenzaCorrente)
                {
                    recordLog.Campi.Add(new Campo("PAZ_MOTIVO_CESSAZIONE_ASSIST", motivoCessazioneAssistenzaOriginale, motivoCessazioneAssistenzaCorrente));
                }

                #endregion
            }

            return recordLog;
        }
    }
}
