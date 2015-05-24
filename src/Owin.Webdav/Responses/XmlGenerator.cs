using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Soukoku.Owin.Webdav.Responses
{
    static class XmlGenerator
    {
        public static XmlDocument CreateMultiStatus(Context context, IEnumerable<IResource> resources = null)
        {
            XmlDocument xmlDoc = new XmlDocument();


            XmlNode rootNode = xmlDoc.CreateElement(DavConsts.ElementNames.MultiStatus, DavConsts.XmlNamespace);
            xmlDoc.AppendChild(rootNode);

            if (resources != null)
            {
                foreach (var resource in resources)
                {
                    XmlNode response = xmlDoc.CreateElement(DavConsts.ElementNames.Response, DavConsts.XmlNamespace);
                    rootNode.AppendChild(response);

                    XmlNode respHref = xmlDoc.CreateElement(DavConsts.ElementNames.Href, DavConsts.XmlNamespace);
                    respHref.InnerText = Uri.EscapeUriString(context.GenerateUrl(resource, false)); // escape required to get some clients working
                    response.AppendChild(respHref);

                    XmlNode respProperty = xmlDoc.CreateElement(DavConsts.ElementNames.PropStat, DavConsts.XmlNamespace);
                    response.AppendChild(respProperty);

                    XmlNode propStatus = xmlDoc.CreateElement(DavConsts.ElementNames.Status, DavConsts.XmlNamespace);
                    // todo: use real status code
                    propStatus.InnerText = StatusCode.OK.GenerateStatusMessage();
                    respProperty.AppendChild(propStatus);


                    XmlNode propList = xmlDoc.CreateElement(DavConsts.ElementNames.Prop, DavConsts.XmlNamespace);
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
