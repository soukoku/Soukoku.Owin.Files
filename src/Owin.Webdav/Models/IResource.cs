using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav.Models
{
    public interface IResource
    {
        IEnumerable<IProperty> Properties { get; }
        T FindProperty<T>(string name, string nameSpace) where T : class, IProperty;

        void AddProperties(IEnumerable<IProperty> properties);
        void AddProperty(IProperty property);
    }
}
