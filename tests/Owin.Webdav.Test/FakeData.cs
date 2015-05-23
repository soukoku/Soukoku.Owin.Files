using Soukoku.Owin;
using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav.Test
{
    class FakeDataStore : IDataStore
    {
        private Func<IEnumerable<IResource>> _getSubDelegate;
        private Func<IResource> _getSingleDelegate;

        public FakeDataStore(Func<IResource> getResource = null, Func<IEnumerable<DavResource>> getSubResources = null)
        {
            _getSingleDelegate = getResource;
            _getSubDelegate = getSubResources;
        }


        public IResource GetResource(string pathBase, string logicalPath)
        {
            return _getSingleDelegate();
        }

        public IEnumerable<IResource> GetSubResources(string pathBase, IResource resource)
        {
            return _getSubDelegate();
        }
    }

    class FakeFolderResource : DavResource
    {
        public FakeFolderResource(string pathBase, string logicalPath) : base(pathBase, logicalPath)
        {

        }
        public override ResourceType ResourceType
        {
            get
            {
                return ResourceType.Collection;
            }
        }
    }

    class FakeFileResource : DavResource
    {
        public FakeFileResource(string pathBase, string logicalPath) : base(pathBase, logicalPath)
        {

        }
        public override ResourceType ResourceType
        {
            get
            {
                return ResourceType.Resource;
            }
        }
    }
}
