using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Soukoku.Owin;

namespace Soukoku.Owin.Files.Services.BuiltIn
{
    /// <summary>
    /// Implements <see cref="IReadOnlyDataStore"/> with a single static file.
    /// </summary>
    public class SingleFilesDataStore : IReadOnlyDataStore
    {
        static readonly DateTime DefaultTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private string _filePath;
        private byte[] _fileData;
        private string _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleFilesDataStore" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.ArgumentException">Invalid root path.;rootPath</exception>
        public SingleFilesDataStore(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) { throw new ArgumentException("Invalid file path.", "filePath"); }
            _filePath = Path.GetFullPath(filePath);
            _fileName = Path.GetFileName(filePath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleFilesDataStore" /> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentException">Invalid root path.;rootPath</exception>
        /// <exception cref="System.ArgumentNullException">data</exception>
        public SingleFilesDataStore(string fileName, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentException("Invalid file name.", "fileName"); }
            if (data == null) { throw new ArgumentNullException("data"); }

            _fileName = fileName;
            _fileData = data;
        }



        public ResourceResult GetResource(OwinContext context, string logicalPath)
        {
            if (logicalPath == "/")
            {
                return new ResourceResult
                {
                    Resource = new Resource(context, logicalPath, true)
                    {
                        CreationDateUtc = DefaultTime,
                        ModifiedDateUtc = DefaultTime
                    }
                };
            }
            else if (IsValidFilePath(logicalPath))
            {
                return GetFileResource(context, logicalPath);
            };

            return new ResourceResult
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
        }

        public IEnumerable<ResourceResult> GetSubResources(OwinContext context, Resource parentFolder)
        {
            if (parentFolder == null) { throw new ArgumentNullException("parentFolder"); }

            // only supports root
            if (parentFolder.IsFolder && parentFolder.LogicalPath == "/")
            {
                yield return GetFileResource(context, Path.Combine(parentFolder.LogicalPath, _fileName));
            }
        }

        public Stream Open(Resource resource)
        {
            if (resource == null) { throw new ArgumentNullException("resource"); }
            if (resource.IsFolder) { throw new InvalidOperationException("Cannot open stream for a folder."); }

            if (IsValidFilePath(resource.LogicalPath))
            {
                if (File.Exists(_filePath))
                {
                    return new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                else
                {
                    return new MemoryStream(_fileData);
                }
            }
            return null;
        }

        #region utilities


        /// <summary>
        /// Checks if the logical path is valid for the single file specified in constructor.
        /// </summary>
        /// <param name="logicalPath">The logical path.</param>
        /// <returns></returns>
        bool IsValidFilePath(string logicalPath)
        {
            if (logicalPath == null)
            {
                logicalPath = "";
            }
            else if (logicalPath.StartsWith("/", StringComparison.Ordinal))
            {
                logicalPath = logicalPath.Substring(1);
            }

            if (string.Equals(logicalPath, _fileName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        private ResourceResult GetFileResource(OwinContext context, string logicalPath)
        {
            if (File.Exists(_filePath))
            {
                var info = new FileInfo(_filePath);
                return new ResourceResult
                {
                    Resource = new Resource(context, logicalPath, false)
                    {
                        CreationDateUtc = info.CreationTimeUtc,
                        ModifiedDateUtc = info.LastWriteTimeUtc,
                        Length = info.Length,
                    }
                };
            }
            else
            {
                return new ResourceResult
                {
                    Resource = new Resource(context, logicalPath, false)
                    {
                        CreationDateUtc = DefaultTime,
                        ModifiedDateUtc = DefaultTime,
                        Length = _fileData.Length
                    }
                };
            }
        }

        #endregion
    }
}
