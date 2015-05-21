using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav
{
    public class WebdavConfig
    {
        public WebdavConfig(IDataStore dataStore)
        {
            if (dataStore == null) { throw new ArgumentNullException("dataStore"); }

            DataStore = dataStore;
            _defaultLog = new NullLog();
        }

        public bool AllowDirectoryBrowsing { get; set; }
        public bool AllowInfiniteDepth { get; set; }
        public IDataStore DataStore { get; private set; }

        NullLog _defaultLog;
        private ILog _log;
        public ILog Log
        {
            get { return _log ?? _defaultLog; }
            set { _log = value; }
        }

    }
}
