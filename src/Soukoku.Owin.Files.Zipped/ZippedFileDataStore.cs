using Soukoku.Owin.Files.Internal;
using Soukoku.Owin.Files.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Implements <see cref="IReadOnlyDataStore"/> using files in a zipped file.
    /// </summary>
    public sealed class ZippedFileDataStore : IReadOnlyDataStore
    {
        byte[] _zipData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZippedFileDataStore" /> class.
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
#if FX4
            using (var zip = new IonicZipFile(ms))
#else
            using (var zip = new FXZipFile(ms))
#endif
            {
                var hit = zip.Entries.FirstOrDefault(entry =>
                {
                    return string.Equals(entry.FullPath.Trim('/'), zipPath, StringComparison.OrdinalIgnoreCase);
                });

                if (hit == null)
                {
                    return new ResourceResult { StatusCode = System.Net.HttpStatusCode.NotFound };
                }
                else if (hit.IsFolder)
                {
                    return new ResourceResult
                    {
                        Resource = new Resource(context, logicalPath, true)
                        {
                            CreationDateUtc = hit.Created,
                            ModifiedDateUtc = hit.Modified
                        }
                    };
                }
                else
                {
                    return new ResourceResult
                    {
                        Resource = new Resource(context, logicalPath, false)
                        {
                            CreationDateUtc = hit.Created,
                            ModifiedDateUtc = hit.Modified,
                            Length = hit.OrigSize,
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
#if FX4
                using (var zip = new IonicZipFile(ms))
#else
                using (var zip = new FXZipFile(ms))
#endif
                {
                    var hits = zip.Entries.Where(entry =>
                    {
                        return string.Equals(GetZipDirectoryName(entry.FullPath), zipPath, StringComparison.OrdinalIgnoreCase);
                    }).Select(entry =>
                    {
                        if (entry.IsFolder)
                        {
                            return new ResourceResult
                            {
                                Resource = new Resource(context, entry.FullPath, true)
                                {
                                    CreationDateUtc = entry.Created,
                                    ModifiedDateUtc = entry.Modified
                                }
                            };
                        }
                        return new ResourceResult
                        {
                            Resource = new Resource(context, entry.FullPath, false)
                            {
                                CreationDateUtc = entry.Created,
                                ModifiedDateUtc = entry.Modified,
                                Length = entry.OrigSize,
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
            IZipFile zip = null;
            Stream fileStream = null;
            bool attached = false;
            try
            {
                zipStream = new MemoryStream(_zipData);
#if FX4
                zip = new IonicZipFile(zipStream);
#else
                zip = new FXZipFile(zipStream);
#endif

                var hit = zip.Entries.FirstOrDefault(entry =>
                {
                    return string.Equals(entry.FullPath, zipPath, StringComparison.OrdinalIgnoreCase);
                });

                if (hit != null && !hit.IsFolder)
                {
                    fileStream = hit.Open();
                    attached = true;
                    return new AttachedDisposableStream(fileStream, zip, zipStream);
                }
            }
            finally
            {
                if (!attached)
                {
                    if (fileStream != null) { fileStream.Dispose(); }
                    if (zip != null) { zip.Dispose(); }
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
