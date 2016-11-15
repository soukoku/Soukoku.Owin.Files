using Soukoku.Owin.Files.Services;
using Soukoku.Owin.Files.Services.BuiltIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Class for specifying the file store configuration.
    /// </summary>
    public class FilesConfig
    {
        /// <summary>
        /// The owin environment key for this config object.
        /// </summary>
        internal const string OwinKey = "soukoku.FilesConfig";

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesConfig"/> class.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        /// <exception cref="System.ArgumentNullException">dataStore</exception>
        public FilesConfig(IReadOnlyDataStore dataStore)
        {
            if (dataStore == null) { throw new ArgumentNullException("dataStore"); }

            DataStore = dataStore;
            _defaultLog = new NullLog();
            _defaultDirGen = new BootstrapDirectoryListingGenerator();
            _defaultMime = new MimeTypeProvider();
            _defaultETag = new ModifiedDateETagGenerator();

            DefaultDocuments = new List<string>
            {
                "index.html",
                "index.htm",
            };
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow directory browsing.
        /// </summary>
        /// <value>
        /// <c>true</c> to allow directory browsing; otherwise, <c>false</c>.
        /// </value>
        public bool AllowDirectoryBrowsing { get; set; }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public IReadOnlyDataStore DataStore { get; private set; }

        ILog _defaultLog;
        private ILog _log;
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public ILog Log
        {
            get { return _log ?? _defaultLog; }
            set { _log = value; }
        }

        IDirectoryListingGenerator _defaultDirGen;
        private IDirectoryListingGenerator _dirGen;
        /// <summary>
        /// Gets or sets the directory listing generator.
        /// </summary>
        /// <value>
        /// The directory listing generator.
        /// </value>
        public IDirectoryListingGenerator DirectoryGenerator
        {
            get { return _dirGen ?? _defaultDirGen; }
            set { _dirGen = value; }
        }


        IMimeTypeProvider _defaultMime;
        private IMimeTypeProvider _mime;
        /// <summary>
        /// Gets or sets the mime type provider.
        /// </summary>
        /// <value>
        /// The mime type provider.
        /// </value>
        public IMimeTypeProvider MimeTypeProvider
        {
            get { return _mime ?? _defaultMime; }
            set { _mime = value; }
        }


        IETagGenerator _defaultETag;
        private IETagGenerator _etag;
        /// <summary>
        /// Gets or sets the ETag generator.
        /// </summary>
        /// <value>
        /// The ETag generator.
        /// </value>
        public IETagGenerator ETagGenerator
        {
            get { return _etag ?? _defaultETag; }
            set { _etag = value; }
        }

        /// <summary>
        /// Gets the default document name list. This has no effect if <see cref="AllowDirectoryBrowsing"/> is set.
        /// </summary>
        /// <value>
        /// The default documents.
        /// </value>
        public IList<string> DefaultDocuments { get; private set; }
    }

}
