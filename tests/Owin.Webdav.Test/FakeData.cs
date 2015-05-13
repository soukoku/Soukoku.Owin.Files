using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Owin.Webdav.Test
{
    class FakeDataStore : IDataStore
    {
        private Func<IEnumerable<Resource>> _getSubDelegate;
        private Func<Resource> _getSingleDelegate;

        public FakeDataStore(Func<Resource> getResource = null, Func<IEnumerable<Resource>> getSubResources = null)
        {
            _getSingleDelegate = getResource;
            _getSubDelegate = getSubResources;
        }


        public Resource GetResource(IOwinContext context, string logicalPath)
        {
            return _getSingleDelegate();
        }

        public IEnumerable<Resource> GetSubResources(IOwinContext context, Resource resource)
        {
            return _getSubDelegate();
        }
    }

    class FakeFolderResource : Resource
    {
        public FakeFolderResource(IOwinContext context, string logicalPath) : base(context, logicalPath)
        {

        }
        public override ResourceType Type
        {
            get
            {
                return ResourceType.Folder;
            }
        }
    }

    class FakeFileResource : Resource
    {
        public FakeFileResource(IOwinContext context, string logicalPath) : base(context, logicalPath)
        {

        }
        public override ResourceType Type
        {
            get
            {
                return ResourceType.File;
            }
        }
    }
}
