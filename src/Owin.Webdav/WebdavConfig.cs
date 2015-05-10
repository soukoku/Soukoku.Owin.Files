using Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin.Webdav
{
    public class WebdavConfig
    {
        public WebdavConfig(IDataStore dataStore)
        {
            if (dataStore == null) { throw new ArgumentNullException("dataStore"); }

            DataStore = dataStore;
        }

        public bool AllowDirectoryBrowsing { get; set; }
        public bool AllowInfiniteDepth { get; set; }
        public IDataStore DataStore { get; private set; }
    }
}
