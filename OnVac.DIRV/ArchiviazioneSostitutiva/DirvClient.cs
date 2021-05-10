using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Onit.Shared.Soap.SWA;


namespace Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva
{
    public class DirvClient
    {
        #region Public

        /// <summary>
        /// Invio al servizio. I nomi dei file di intestazione e dei metadati devono essere specificati.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dati"></param>
        /// <param name="nomeFileIntestazione"></param>
        /// <param name="nomeFileMetadatiRichiesta"></param>
        /// <param name="nomeFileMetadatiRisposta"></param>
        /// <returns></returns>
        public static ArchiviazioneSostitutivaResponse Send(string url, ArchiviazioneSostitutivaMessage dati, string nomeFileIntestazione, string nomeFileMetadatiRichiesta, string nomeFileMetadatiRisposta)
        {
            if (string.IsNullOrWhiteSpace(nomeFileIntestazione) || string.IsNullOrWhiteSpace(nomeFileMetadatiRichiesta) || string.IsNullOrWhiteSpace(nomeFileMetadatiRisposta))
                throw new ArgumentNullException("Nome di file mancante per intestazione e/o metadati");

            DirvClient.VerificaDati(dati, nomeFileMetadatiRichiesta);

            string envelope = @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:eng:spagic"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <urn:payload>?</urn:payload>
                    </soapenv:Body>
                </soapenv:Envelope>";

            List<Attachment> ls = new List<Attachment>();
            ls.Add(new Attachment()
            {
                FileName = nomeFileIntestazione,
                FileContent = DirvClient.Serialize(dati.Intestazione),
                ContentType = Constants.MIMEContentType.APPLICATION_OCTET_STREAM,
                TransferEncoding = Constants.TransferEncoding.Binary
            });

            ls.Add(new Attachment()
            {
                FileName = nomeFileMetadatiRichiesta,
                FileContent = DirvClient.Serialize(dati.Metadati),
                Charset = Constants.Charset.US_ASCII,
                ContentType = Constants.MIMEContentType.TEXT_XML,
                TransferEncoding = Constants.TransferEncoding.SevenBit,
            });

            foreach (var doc in dati.Documenti)
            {
                ls.Add(new Attachment()
                {
                    FileName = doc.Nome,
                    FileContent = doc.Contenuto,
                    ContentType = Constants.MIMEContentType.APPLICATION_OCTET_STREAM,
                    TransferEncoding = Constants.TransferEncoding.Binary,
                });
            }

            SoapClient cl = new SoapClient();
            cl.Url = url;

            ArchiviazioneSostitutivaResponse response = new ArchiviazioneSostitutivaResponse();
            response.SwaResponse = cl.SendSWA(envelope, ls);

            var metadatiRisposta = response.SwaResponse.Attachments.Where(a => a.FileName == nomeFileMetadatiRisposta).FirstOrDefault();
            if (metadatiRisposta!=null)
            {
                response.MetadatiRisposta = DirvClient.ParseMetadatiRisposta(metadatiRisposta);
            }

            return response;
        }

        #endregion

        #region Private
        
        private static risposta_archiviazione_conservazione ParseMetadatiRisposta(Attachment metadatiRisposta)
        {
            risposta_archiviazione_conservazione result = null;

            XmlSerializer s = new XmlSerializer(typeof(risposta_archiviazione_conservazione));
            using (MemoryStream ms = new MemoryStream(metadatiRisposta.FileContent))
            {
                result = (risposta_archiviazione_conservazione)s.Deserialize(ms);
            }

            return result;
        }

        private static void VerificaDati(ArchiviazioneSostitutivaMessage dati, string nomeFileMetadatiRichiesta)
        {
            //controllo dell'allegato XML_MESSAGE
            if (dati != null && dati.Intestazione != null
                && dati.Intestazione.RichiestaApplicativa != null
                && dati.Intestazione.RichiestaApplicativa.Allegati != null)
            {
                if (!dati.Intestazione.RichiestaApplicativa.Allegati.Any(a => a.file_name == nomeFileMetadatiRichiesta))
                {
                    dati.Intestazione.RichiestaApplicativa.Allegati.Add(new Headers.Allegato() { file_name = nomeFileMetadatiRichiesta });
                }

                if (dati.Documenti != null) 
                { 
                    var docMancanti = dati.Documenti.Where(d => !dati.Intestazione.RichiestaApplicativa.Allegati.Any(a => a.file_name == d.Nome));
                    dati.Intestazione.RichiestaApplicativa.Allegati.AddRange(docMancanti.Select(d => new Headers.Allegato() { file_name = d.Nome }));
                }
            }
        }

        private static byte[] Serialize(object obj)
        {
            if (obj == null)
                return new byte[] { };

            byte[] buffer;
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            XmlSerializer s = new XmlSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                var settings = new XmlWriterSettings()
                {
                    Indent=true,
                    NamespaceHandling= NamespaceHandling.OmitDuplicates,
                    Encoding = utf8WithoutBom,
                };

                using (XmlWriter tw = XmlWriter.Create(ms, settings))
                {
                    s.Serialize(tw, obj,ns);
                    tw.Flush();
                    ms.Position = 0;
                    buffer = ms.ToArray();
                }
            }

            return buffer;
        }

        #endregion
    }
}
