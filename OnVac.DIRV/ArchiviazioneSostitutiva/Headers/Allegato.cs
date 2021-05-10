using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Onit.OnAssistnet.OnVac.DIRV.ArchiviazioneSostitutiva.Headers
{
    [Serializable]
    public class Allegato
    {
        [XmlAttribute]
        public string file_name { get; set; }

        [XmlText()]
        public string file_nameElement 
        {
            get { return file_name; }
            set { file_name = value; }
        }
    }
}
