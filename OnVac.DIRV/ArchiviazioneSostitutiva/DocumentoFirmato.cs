using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva
{
    public class DocumentoFirmato
    {
        public string Nome { get; set; }
        public byte[] Contenuto { get; set; }

        public string Hash
        {
            get { return this.GetHashString(); }
        }

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="name"></param>
        /// <param name="base64Content"></param>
        /// <param name="isContentCrypted">Indica se il contenuto è criptato o in chiaro</param>
        public DocumentoFirmato(string name, string base64Content, bool isContentCrypted)
        {
            this.Nome = name;

            if (isContentCrypted)
            {
                this.Contenuto = Convert.FromBase64String(base64Content);
            }
            else
            {
                this.Contenuto = System.Text.Encoding.UTF8.GetBytes(base64Content);
            }
        }

        /// <summary>
        /// Restituisce l'impronta hash relativa al contenuto del documento
        /// </summary>
        /// <returns></returns>
        private string GetHashString()
        {
            if (this.Contenuto == null) return null;

            SHA1 sha1 = SHA1Managed.Create();
            byte[] hash = sha1.ComputeHash(this.Contenuto);

            StringBuilder result = new StringBuilder();

            foreach (var item in hash)
            {
                result.Append(Convert.ToInt32(item).ToString("x2"));
            }

            return result.ToString();
        }
    }
}
