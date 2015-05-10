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


        internal byte[] Serialize()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(MultiStatusResponse));


                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                ns.Add("d", WebdavConsts.XmlNamespace);

                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    xs.Serialize(writer, this, ns);
                    return stream.ToArray();
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
