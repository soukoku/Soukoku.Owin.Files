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

namespace Soukoku.Owin.Files.Zipped
{
    /// <summary>
    /// Implements <see cref="IDataStore"/> using files in a zipped file.
    /// </summary>
    public class ZippedFileDataStore : IReadOnlyDataStore, IDisposable
    {
        ZipFile _archive;

        /// <summary>
        /// Initializes a new instance of the <see cref="LooseFilesDataStore" /> class.
        /// </summary>
        /// <param name="zipStream">The zip stream.</param>
        public ZippedFileDataStore(Stream zipStream)
        {
            _archive = ZipFile.Read(zipStream);
        }

        public void Dispose()
        {
            if (_archive != null)
            {
                _archive.Dispose();
                _archive = null;
            }
        }

        public ResourceResult GetResource(Context context, string logicalPath)
        {
            var zipPath = logicalPath.Trim('/');

            var hit = _archive.FirstOrDefault(entry =>
            {
                return string.Equals(entry.FileName.Trim('/'), zipPath);
            });

            if (hit == null)
            {
                if (zipPath == string.Empty)
                {
                    // root
                    return new ResourceResult
                    {
                        Resource = new Resource(context, logicalPath, true)
                    };
                }
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

        public IEnumerable<ResourceResult> GetSubResources(Context context, Resource parentFolder)
        {
            if (parentFolder == null) { throw new ArgumentNullException("parentFolder"); }

            if (parentFolder.IsFolder)
            {
                var zipPath = parentFolder.LogicalPath.Trim('/');
                var hits = _archive.Where(entry =>
                {
                    return string.Equals(GetZipDirectoryName(entry.FileName), zipPath);
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
            return Enumerable.Empty<ResourceResult>();
        }

        public Stream Open(Resource resource)
        {
            if (resource == null) { throw new ArgumentNullException("resource"); }
            if (resource.IsFolder) { throw new InvalidOperationException("Cannot open stream for a folder."); }

            var zipPath = resource.LogicalPath.Trim('/');
            var hit = _archive.FirstOrDefault(entry =>
            {
                return string.Equals(entry.FileName, zipPath);
            });

            if (hit != null && !hit.IsDirectory)
            {
                return hit.OpenReader();
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
