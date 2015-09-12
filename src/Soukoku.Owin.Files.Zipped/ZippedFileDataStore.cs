using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Soukoku.Owin;
using Ionic.Zip;
using Soukoku.Owin.Files.Services;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Implements <see cref="IReadOnlyDataStore"/> using files in a zipped file.
    /// </summary>
    public sealed class ZippedFileDataStore : IReadOnlyDataStore
    {
        byte[] _zipData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseFilesDataStore" /> class.
        /// </summary>
        /// <param name="zipData">The zip data.</param>
        public ZippedFileDataStore(byte[] zipData)
        {
            // use array to create a new stream object each time for multithread support hack.
            _zipData = zipData;
        }

        public ResourceResult GetResource(OwinContext context, string logicalPath)
        {
            var zipPath = logicalPath == null ? string.Empty : logicalPath.Trim('/');
            if (string.IsNullOrEmpty(zipPath))
            {
                // root
                return new ResourceResult
                {
                    Resource = new Resource(context, logicalPath, true)
                };
            }

            using (var ms = new MemoryStream(_zipData))
            using (var zipFile = ZipFile.Read(ms))
            {
                var hit = zipFile.FirstOrDefault(entry =>
                {
                    return string.Equals(entry.FileName.Trim('/'), zipPath, StringComparison.OrdinalIgnoreCase);
                });

                if (hit == null)
                {
                    return new ResourceResult { StatusCode = System.Net.HttpStatusCode.NotFound };
                }
                else if (hit.IsDirectory)
                {
                    return new ResourceResult
                    {
                        Resource = new Resource(context, logicalPath, true)
                        {
                            CreationDateUtc = hit.CreationTime.ToUniversalTime(),
                            ModifiedDateUtc = hit.LastModified.ToUniversalTime(),
                        }
                    };
                }
                else
                {
                    return new ResourceResult
                    {
                        Resource = new Resource(context, logicalPath, false)
                        {
                            CreationDateUtc = hit.CreationTime.ToUniversalTime(),
                            ModifiedDateUtc = hit.LastModified.ToUniversalTime(),
                            Length = hit.UncompressedSize,
                        }
                    };
                }
            }
        }

        public IEnumerable<ResourceResult> GetSubResources(OwinContext context, Resource parentFolder)
        {
            if (parentFolder == null) { throw new ArgumentNullException("parentFolder"); }

            if (parentFolder.IsFolder)
            {
                var zipPath = parentFolder.LogicalPath.Trim('/');

                using (var ms = new MemoryStream(_zipData))
                using (var zipFile = ZipFile.Read(ms))
                {
                    var hits = zipFile.Where(entry =>
                    {
                        return string.Equals(GetZipDirectoryName(entry.FileName), zipPath, StringComparison.OrdinalIgnoreCase);
                    }).Select(entry =>
                    {
                        if (entry.IsDirectory)
                        {
                            return new ResourceResult
                            {
                                Resource = new Resource(context, entry.FileName, true)
                                {
                                    CreationDateUtc = entry.CreationTime.ToUniversalTime(),
                                    ModifiedDateUtc = entry.LastModified.ToUniversalTime(),
                                }
                            };
                        }
                        return new ResourceResult
                        {
                            Resource = new Resource(context, entry.FileName, false)
                            {
                                CreationDateUtc = entry.CreationTime.ToUniversalTime(),
                                ModifiedDateUtc = entry.LastModified.ToUniversalTime(),
                                Length = entry.UncompressedSize,
                            }
                        };
                    });

                    return hits;
                }
            }
            return Enumerable.Empty<ResourceResult>();
        }

        public Stream Open(Resource resource)
        {
            if (resource == null) { throw new ArgumentNullException("resource"); }
            if (resource.IsFolder) { throw new InvalidOperationException("Cannot open stream for a folder."); }

            var zipPath = resource.LogicalPath.Trim('/');

            Stream zipStream = null;
            ZipFile zipFile = null;
            Stream fileStream = null;
            bool attached = false;
            try
            {
                zipStream = new MemoryStream(_zipData);
                zipFile = ZipFile.Read(zipStream);

                var hit = zipFile.FirstOrDefault(entry =>
                {
                    return string.Equals(entry.FileName, zipPath, StringComparison.OrdinalIgnoreCase);
                });

                if (hit != null && !hit.IsDirectory)
                {
                    fileStream = hit.OpenReader();
                    attached = true;
                    return new AttachedDisposableStream(fileStream, zipFile, zipStream);
                }
            }
            finally
            {
                if (!attached)
                {
                    if (fileStream != null) { fileStream.Dispose(); }
                    if (zipFile != null) { zipFile.Dispose(); }
                    if (zipStream != null) { zipStream.Dispose(); }
                }
            }
            return null;
        }

        static string GetZipDirectoryName(string zipPath)
        {
            // don't use Path.GetDirectoryName since we need '/'
            var trimmed = zipPath.Trim('/');
            var idx = trimmed.LastIndexOf('/');
            if (idx > -1)
            {
                return trimmed.Substring(0, idx);
            }
            return string.Empty;
        }

    }
}
