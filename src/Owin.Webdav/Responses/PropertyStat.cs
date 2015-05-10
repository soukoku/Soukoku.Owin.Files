using Owin.Webdav.Models;
using Owin.Webdav.Responses.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Owin.Webdav.Responses
{
    public class PropertyStat
    {
        public PropertyStat()
        {

        }

        public PropertyStat(Resource resource)
        {
            Properties = new List<PropertyBase>();

            Properties.Add(new DisplayName(resource.Name));
            if (resource.Type == Resource.ResourceType.Folder)
            {
                Properties.Add(new ResourceType(new ResourceType.CollectionType()));
            }
            else if (resource.Type == Resource.ResourceType.File)
            {
                Properties.Add(new ResourceType());
                Properties.Add(new ContentLength(resource.Length));
                Properties.Add(new ContentType(MimeTypes.MimeTypeMap.GetMimeType(resource.Name)));
            }
            Properties.Add(new CreationDate(resource.CreateDate));
            Properties.Add(new ModifiedDate(resource.ModifyDate));

        }

        [XmlElement("status")]
        public string Status { get; set; }

        [XmlArray("prop")]
        [XmlArrayItem("getcontentlength", typeof(ContentLength))]
        [XmlArrayItem("getcontenttype", typeof(ContentType))]
        [XmlArrayItem("creationdate", typeof(CreationDate))]
        [XmlArrayItem("displayname", typeof(DisplayName))]
        [XmlArrayItem("getlastmodified", typeof(ModifiedDate))]
        [XmlArrayItem("resourcetype", typeof(ResourceType))]
        [XmlArrayItem("supportedlock", typeof(SupportedLock))]
        public List<PropertyBase> Properties { get; set; }
    }
}
