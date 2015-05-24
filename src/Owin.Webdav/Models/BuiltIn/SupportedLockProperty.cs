using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Soukoku.Owin.Webdav.Models.BuiltIn
{
    sealed class SupportedLockProperty : PropertyBase
    {
        public SupportedLockProperty(IResource resource) : base(resource) { }
        public override string Name
        {
            get
            {
                return DavConsts.PropertyNames.SupportedLock;
            }
        }

        public override void SerializeValue(XmlElement element, NewElementFunc newElementMethod)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            if (newElementMethod == null) { throw new ArgumentNullException("newElementMethod"); }

            if (Resource.SupportedLocks.HasFlag(LockScopes.Exclusive))
            {
                var entry = newElementMethod(DavConsts.ElementNames.LockEntry, DavConsts.XmlNamespace);
                element.AppendChild(entry);

                var scope = newElementMethod(DavConsts.ElementNames.LockScope, DavConsts.XmlNamespace);
                scope.AppendChild(newElementMethod(DavConsts.ElementNames.Exclusive, DavConsts.XmlNamespace));
                entry.AppendChild(scope);

                var type = newElementMethod(DavConsts.ElementNames.LockType, DavConsts.XmlNamespace);
                type.AppendChild(newElementMethod(DavConsts.ElementNames.Write, DavConsts.XmlNamespace));
                entry.AppendChild(type);
            }

            if (Resource.SupportedLocks.HasFlag(LockScopes.Shared))
            {
                var entry = newElementMethod(DavConsts.ElementNames.LockEntry, DavConsts.XmlNamespace);
                element.AppendChild(entry);

                var scope = newElementMethod(DavConsts.ElementNames.LockScope, DavConsts.XmlNamespace);
                scope.AppendChild(newElementMethod(DavConsts.ElementNames.Shared, DavConsts.XmlNamespace));
                entry.AppendChild(scope);

                var type = newElementMethod(DavConsts.ElementNames.LockType, DavConsts.XmlNamespace);
                type.AppendChild(newElementMethod(DavConsts.ElementNames.Write, DavConsts.XmlNamespace));
                entry.AppendChild(type);
            }
        }
    }
}
