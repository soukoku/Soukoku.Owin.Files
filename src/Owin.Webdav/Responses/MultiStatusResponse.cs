﻿using Soukoku.Owin.Webdav.Models;
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


            XmlNode rootNode = xmlDoc.CreateElement(Consts.ElementName.MultiStatus, Consts.XmlNamespace);
            xmlDoc.AppendChild(rootNode);

            if (resources != null)
            {
                foreach (var resource in resources)
                {
                    XmlNode response = xmlDoc.CreateElement(Consts.ElementName.Response, Consts.XmlNamespace);
                    rootNode.AppendChild(response);

                    XmlNode respHref = xmlDoc.CreateElement(Consts.ElementName.Href, Consts.XmlNamespace);
                    respHref.InnerText = Uri.EscapeUriString(resource.Url); // required to get some clients working
                    respHref.InnerText = resource.Url;
                    response.AppendChild(respHref);

                    XmlNode respProperty = xmlDoc.CreateElement(Consts.ElementName.PropStat, Consts.XmlNamespace);
                    response.AppendChild(respProperty);

                    XmlNode propStatus = xmlDoc.CreateElement(Consts.ElementName.Status, Consts.XmlNamespace);
                    // todo: use real status code
                    propStatus.InnerText = HttpStatusCode.OK.GenerateStatusMessage();
                    respProperty.AppendChild(propStatus);


                    XmlNode propList = xmlDoc.CreateElement(Consts.ElementName.Prop, Consts.XmlNamespace);
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
                    
                    XmlNode resTypeNode = xmlDoc.CreateElement(Consts.PropertyName.ResourceType, Consts.XmlNamespace);
                    if (resource.Type == Resource.ResourceType.Folder)
                    {
                        resTypeNode.AppendChild(xmlDoc.CreateElement("collection", Consts.XmlNamespace));
                    }
                    propList.AppendChild(resTypeNode);

                    XmlNode lockNode = xmlDoc.CreateElement(Consts.PropertyName.SupportedLock, Consts.XmlNamespace);
                    propList.AppendChild(lockNode);

                    #endregion
                }
            }
            return xmlDoc;
        }


    }
}
