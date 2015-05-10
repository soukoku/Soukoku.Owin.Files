using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav.Models
{
    public interface IDataStore
    {
        Resource GetResource(IOwinContext context, string logicalPath);

        IEnumerable<Resource> GetSubResources(IOwinContext context, Resource resource);
    }
}
