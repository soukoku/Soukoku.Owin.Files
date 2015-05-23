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
using Microsoft.Owin;

namespace Soukoku.Owin.Webdav.Responses
{
    static class XmlGenerator
    {
        public static XmlDocument CreateMultiStatus(IOwinContext context, IEnumerable<IResource> resources = null)
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
                    respHref.InnerText = Uri.EscapeUriString(context.GenerateUrl(resource)); // escape required to get some clients working
                    response.AppendChild(respHref);

                    XmlNode respProperty = xmlDoc.CreateElement(Consts.ElementName.PropStat, Consts.XmlNamespace);
                    response.AppendChild(respProperty);

                    XmlNode propStatus = xmlDoc.CreateElement(Consts.ElementName.Status, Consts.XmlNamespace);
                    // todo: use real status code
                    propStatus.InnerText = StatusCode.OK.GenerateStatusMessage();
                    respProperty.AppendChild(propStatus);


                    XmlNode propList = xmlDoc.CreateElement(Consts.ElementName.Prop, Consts.XmlNamespace);
                    respProperty.AppendChild(propList);

                    #region dav-properties

                    foreach (var prop in resource.Properties)
                    {
                        var propNode = xmlDoc.CreateElement(prop.Name, prop.NamespaceUri);
                        prop.SerializeValue(propNode.CreateNavigator());
                        propList.AppendChild(propNode);
                    }

                    //XmlNode lockNode = xmlDoc.CreateElement(Consts.PropertyName.SupportedLock, Consts.XmlNamespace);
                    //propList.AppendChild(lockNode);

                    #endregion
                }
            }
            return xmlDoc;
        }


    }
}
