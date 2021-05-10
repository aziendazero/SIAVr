using System;

namespace Onit.OnAssistnet.OnVac.Log.DataLogManager
{
    public static  class LogManager
    {
        public static Log.DataLogStructure.Testata GetTestataLogMessaggioGenerico(string codiceArgomento, bool automatico)
        {
            return LogManager.GetTestataLogMessaggioGenerico(null, codiceArgomento, string.Empty, string.Empty, automatico);
        }

        public static Log.DataLogStructure.Testata GetTestataLogMessaggioGenerico(string codiceArgomento, String titoloMessaggio, String descrizioneMessaggio, bool automatico)
        {
            return LogManager.GetTestataLogMessaggioGenerico(null, codiceArgomento, titoloMessaggio, descrizioneMessaggio, automatico);
        }
       
        public static Log.DataLogStructure.Testata GetTestataLogMessaggioGenerico(long? idPaziente, string codiceArgomento, bool automatico)
        {
            return LogManager.GetTestataLogMessaggio(idPaziente, DataLogStructure.Operazione.Generico, codiceArgomento, string.Empty, string.Empty, automatico);
        }

        public static Log.DataLogStructure.Testata GetTestataLogMessaggioGenerico(long? idPaziente, string codiceArgomento, String titoloMessaggio, String descrizioneMessaggio, bool automatico)
        {
            return LogManager.GetTestataLogMessaggio(idPaziente, DataLogStructure.Operazione.Generico, codiceArgomento, titoloMessaggio, descrizioneMessaggio, automatico);
        }

        public static Log.DataLogStructure.Testata GetTestataLogMessaggioErrore(string codiceArgomento, bool automatico)
        {
            return LogManager.GetTestataLogMessaggioErrore(null, codiceArgomento, string.Empty, string.Empty, automatico);
        }
     
        public static Log.DataLogStructure.Testata GetTestataLogMessaggioErrore(string codiceArgomento, String titoloMessaggio, String descrizioneMessaggio, bool automatico)
        {
            return LogManager.GetTestataLogMessaggioErrore(null, codiceArgomento, titoloMessaggio, descrizioneMessaggio, automatico);
        }

        public static Log.DataLogStructure.Testata GetTestataLogMessaggioErrore(long? idPaziente, string codiceArgomento, bool automatico)
        {
            return LogManager.GetTestataLogMessaggio(idPaziente, DataLogStructure.Operazione.Eccezione, codiceArgomento, string.Empty, string.Empty, automatico);
        }

        public static Log.DataLogStructure.Testata GetTestataLogMessaggioErrore(long? idPaziente, string codiceArgomento, String titoloMessaggio, String descrizioneMessaggio, bool automatico)
        {
            return LogManager.GetTestataLogMessaggio(idPaziente, DataLogStructure.Operazione.Eccezione, codiceArgomento, titoloMessaggio, descrizioneMessaggio, automatico);
        }

        public static Log.DataLogStructure.Testata GetTestataLogMessaggio(long? idPaziente, DataLogStructure.Operazione operazione, string codiceArgomento, String titoloMessaggio, String descrizioneMessaggio, bool automatico)
        {
            DataLogStructure.Testata testataLog = new DataLogStructure.Testata(codiceArgomento, operazione, automatico);

            if (idPaziente != null)
            {
                testataLog.Paziente = Convert.ToDouble(idPaziente.Value);
            }

            if (!string.IsNullOrEmpty(titoloMessaggio) || !string.IsNullOrEmpty(descrizioneMessaggio))
            {
                testataLog.Records.Add(LogManager.GetRecordLogMessaggio(titoloMessaggio, descrizioneMessaggio));
            }

            return testataLog;
        }

        /// <summary>
        /// Restituisce un record contenente il messaggio specificato.
        /// </summary>
        /// <param name="titoloMessaggio"></param>
        /// <param name="descrizioneMessaggio"></param>
        public static DataLogStructure.Record GetRecordLogMessaggio(String titoloMessaggio, String descrizioneMessaggio)
        {
            DataLogStructure.Record record = new DataLogStructure.Record();

            record.Campi.Add(new DataLogStructure.Campo(titoloMessaggio, descrizioneMessaggio));

            return record;
        }

        /// <summary>
        /// Restituisce un campo contenente il messaggio specificato.
        /// </summary>
        /// <param name="titoloMessaggio"></param>
        /// <param name="descrizioneMessaggio"></param>
        public static DataLogStructure.Campo GetCampoLogMessaggio(String titoloMessaggio, String descrizioneMessaggio)
        {
            return new DataLogStructure.Campo(titoloMessaggio, descrizioneMessaggio);
        }
    }
}
