using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Owin;

namespace Soukoku.Owin.Webdav.Responses
{
    class MultiStatusResponse
    {
        public static XmlDocument Create(IEnumerable<Resource> resources = null)
        {
            XmlDocument xmlDoc = new XmlDocument();


            XmlNode rootNode = xmlDoc.CreateElement(Consts.Xml.ResponseList, Consts.Xml.Namespace);
            xmlDoc.AppendChild(rootNode);

            if (resources != null)
            {
                foreach (var resource in resources)
                {
                    XmlNode response = xmlDoc.CreateElement(Consts.Xml.Response, Consts.Xml.Namespace);
                    rootNode.AppendChild(response);

                    XmlNode respHref = xmlDoc.CreateElement(Consts.Xml.RespHref, Consts.Xml.Namespace);
                    respHref.InnerText = Uri.EscapeUriString(resource.Url); // required to get some clients working
                    respHref.InnerText = resource.Url;
                    response.AppendChild(respHref);

                    XmlNode respProperty = xmlDoc.CreateElement(Consts.Xml.RespProperty, Consts.Xml.Namespace);
                    response.AppendChild(respProperty);

                    XmlNode propStatus = xmlDoc.CreateElement(Consts.Xml.PropertyStatus, Consts.Xml.Namespace);
                    // todo: use real status code
                    propStatus.InnerText = HttpStatusCode.OK.GenerateStatusMessage();
                    respProperty.AppendChild(propStatus);


                    XmlNode propList = xmlDoc.CreateElement(Consts.Xml.PropertyList, Consts.Xml.Namespace);
                    respProperty.AppendChild(propList);

                    #region dav-properties

                    foreach (var prop in resource.Properties)
                    {
                        XmlNode propNode = prop.Serialize(xmlDoc);
                        if (propNode != null)
                        {
                            propList.AppendChild(propNode);
                        }
                    }
                    
                    XmlNode resTypeNode = xmlDoc.CreateElement(Consts.PropertyNames.ResourceType, Consts.Xml.Namespace);
                    if (resource.Type == Resource.ResourceType.Folder)
                    {
                        resTypeNode.AppendChild(xmlDoc.CreateElement("collection", Consts.Xml.Namespace));
                    }
                    propList.AppendChild(resTypeNode);

                    XmlNode lockNode = xmlDoc.CreateElement(Consts.PropertyNames.SupportedLock, Consts.Xml.Namespace);
                    propList.AppendChild(lockNode);

                    #endregion
                }
            }
            return xmlDoc;
        }


    }
}
