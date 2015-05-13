using Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Owin.Webdav.Responses
{
    class MultiStatusResponse
    {
        public static XmlDocument Create(IEnumerable<Resource> resources = null)
        {
            XmlDocument xmlDoc = new XmlDocument();


            XmlNode rootNode = xmlDoc.CreateElement(WebdavConsts.Xml.ResponseList, WebdavConsts.Xml.Namespace);
            xmlDoc.AppendChild(rootNode);

            if (resources != null)
            {
                foreach (var resource in resources)
                {
                    XmlNode response = xmlDoc.CreateElement(WebdavConsts.Xml.Response, WebdavConsts.Xml.Namespace);
                    rootNode.AppendChild(response);

                    XmlNode respHref = xmlDoc.CreateElement(WebdavConsts.Xml.RespHref, WebdavConsts.Xml.Namespace);
                    respHref.InnerText = Uri.EscapeUriString(resource.Url); // required to get some clients working
                    respHref.InnerText = resource.Url;
                    response.AppendChild(respHref);

                    XmlNode respProperty = xmlDoc.CreateElement(WebdavConsts.Xml.RespProperty, WebdavConsts.Xml.Namespace);
                    response.AppendChild(respProperty);

                    XmlNode propStatus = xmlDoc.CreateElement(WebdavConsts.Xml.PropertyStatus, WebdavConsts.Xml.Namespace);
                    // todo: use real status code
                    propStatus.InnerText = HttpStatusCode.OK.GenerateStatusMessage();
                    respProperty.AppendChild(propStatus);


                    XmlNode propList = xmlDoc.CreateElement(WebdavConsts.Xml.PropertyList, WebdavConsts.Xml.Namespace);
                    respProperty.AppendChild(propList);

                    #region dav-properties

                    //XmlNode nameNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropDisplayName, WebdavConsts.Xml.Namespace);
                    //nameNode.InnerText = Uri.EscapeUriString(Path.GetFileName(resource.Url.Trim('/'))); // must be actual url part name event if root of dav store
                    //propList.AppendChild(nameNode);
                    
                    // properties
                    foreach (var prop in resource.Properties)
                    {
                        XmlNode propNode = prop.Serialize(xmlDoc);
                        if (propNode != null)
                        {
                            propList.AppendChild(propNode);
                        }
                    }
                    
                    XmlNode resTypeNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropResourceType, WebdavConsts.Xml.Namespace);
                    if (resource.Type == Resource.ResourceType.Folder)
                    {
                        resTypeNode.AppendChild(xmlDoc.CreateElement("collection", WebdavConsts.Xml.Namespace));
                    }
                    propList.AppendChild(resTypeNode);

                    XmlNode lockNode = xmlDoc.CreateElement(WebdavConsts.Xml.PropSupportedLock, WebdavConsts.Xml.Namespace);
                    propList.AppendChild(lockNode);

                    #endregion
                }
            }
            return xmlDoc;
        }


    }
}
