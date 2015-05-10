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
            Properties.Add(new ContentLength(resource.Length));
            var resType = new ResourceType();
            if (resource.Type == Resource.ResourceType.Folder)
            {
                Properties.Add(new ContentType());
                // TODO: add if not fully dav capable
                //resType.Values.Add(new ResourceType.CollectionType());
            }
            else if (resource.Type == Resource.ResourceType.File)
            {
                Properties.Add(new ContentType(MimeTypes.MimeTypeMap.GetMimeType(resource.Name)));
            }
            Properties.Add(resType);
            Properties.Add(new CreationDate(resource.CreateDate));
            Properties.Add(new ModifiedDate(resource.ModifyDate));

            // TODO: not use the fake locks
            Properties.Add(new SupportedLock
            {
                Locks = new List<LockEntry>
                {
                    new LockEntry { Scopes = new List<LockScope> { new LockScope.Exclusive() }, Types = new List<LockType> { new LockType.Write() } },
                    new LockEntry { Scopes = new List<LockScope> { new LockScope.Shared() }, Types = new List<LockType> { new LockType.Write() } },
                }
            });

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
