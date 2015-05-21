using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    public interface IResource
    {
        ResourceType Type { get; }
        string Url { get; }
        IEnumerable<IProperty> Properties { get; }
        T FindProperty<T>(string name, string nameSpace) where T : class, IProperty;

        void AddProperties(IEnumerable<IProperty> properties);
        void AddProperty(IProperty property);
    }

    public enum ResourceType
    {
        Resource,
        Collection,
    }
}
