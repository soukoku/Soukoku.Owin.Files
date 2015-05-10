using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses
{
    [XmlRoot("multistatus", Namespace = WebdavConsts.XmlNamespace)]
    public class MultiStatusResponse
    {
        public MultiStatusResponse()
        {
            Responses = new List<ResourceResponse>();
        }

        [XmlElement(ElementName = "response")]
        public List<ResourceResponse> Responses { get; set; }

        internal string Serialize()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(MultiStatusResponse));
                using (var writer = new StringWriter())
                {
                    xs.Serialize(writer, this);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
