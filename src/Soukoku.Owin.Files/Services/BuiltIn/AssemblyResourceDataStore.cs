using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Soukoku.Owin;
using Soukoku.Owin.Files.Services;

namespace Soukoku.Owin.Files.Services.BuiltIn
{
    /// <summary>
    /// Implements <see cref="IReadOnlyDataStore"/> using an assembly's manifest resources under a namespace.
    /// </summary>
    public sealed class AssemblyResourceDataStore : IReadOnlyDataStore
    {
        DateTime _createTime;
        DateTime _modifyTime;
        Assembly _ass;

        // item1 = name only
        // item2 = resource full name
        IList<Tuple<string, string>> _knownResourceNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyResourceDataStore" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="rootNamespace">The root namespace.</param>
        public AssemblyResourceDataStore(Assembly assembly, string rootNamespace)
        {
            var info = new FileInfo(assembly.Location);
            _createTime = info.CreationTimeUtc;
            _modifyTime = info.LastWriteTimeUtc;
            _ass = assembly;

            _knownResourceNames = assembly.GetManifestResourceNames()
                .Where(n => n.StartsWith(rootNamespace, StringComparison.OrdinalIgnoreCase))
                .Select(n => new Tuple<string, string>(n.Substring(rootNamespace.Length + 1), n))
                .ToList();
        }

        public ResourceResult GetResource(OwinContext context, string logicalPath)
        {
            var resPath = logicalPath == null ? string.Empty : logicalPath.Trim('/').Replace('/', '.');
            if (string.IsNullOrEmpty(resPath))
            {
                // root
                return new ResourceResult
                {
                    Resource = new Resource(context, logicalPath, true)
                };
            }

            var hit = _knownResourceNames.FirstOrDefault(entry =>
            {
                return string.Equals(entry.Item1, resPath, StringComparison.OrdinalIgnoreCase);
            });

            if (hit == null)
            {
                return new ResourceResult { StatusCode = System.Net.HttpStatusCode.NotFound };
            }
            //else if (hit.IsDirectory)
            //{
            //    return new ResourceResult
            //    {
            //        Resource = new Resource(context, logicalPath, true)
            //        {
            //            CreationDateUtc = hit.CreationTime.ToUniversalTime(),
            //            ModifiedDateUtc = hit.LastModified.ToUniversalTime(),
            //        }
            //    };
            //}
            //else
            //{
            return new ResourceResult
            {
                Resource = new Resource(context, logicalPath, false)
                {
                    CreationDateUtc = _createTime,
                    ModifiedDateUtc = _modifyTime
                }
            };
            //}
        }

        public IEnumerable<ResourceResult> GetSubResources(OwinContext context, Resource parentFolder)
        {
            if (parentFolder == null) { throw new ArgumentNullException("parentFolder"); }

            // only supports root
            if (parentFolder.IsFolder && parentFolder.LogicalPath == "/")
            {
                foreach (var res in _knownResourceNames)
                {
                    yield return new ResourceResult
                    {
                        Resource = new Resource(context, "/" + res.Item1, false)
                        {
                            CreationDateUtc = _createTime,
                            ModifiedDateUtc = _modifyTime,
                            //Length = entry.UncompressedSize,
                        }
                    };
                }
            }
        }

        public Stream Open(Resource resource)
        {
            if (resource == null) { throw new ArgumentNullException("resource"); }
            if (resource.IsFolder) { throw new InvalidOperationException("Cannot open stream for a folder."); }

            var resPath = resource.LogicalPath.Trim('/').Replace('/', '.');

            var hit = _knownResourceNames.FirstOrDefault(r => string.Equals(r.Item1, resPath, StringComparison.OrdinalIgnoreCase));
            if (hit == null)
            {
                return null;
            }

            return _ass.GetManifestResourceStream(hit.Item2);
        }

        //static string GetZipDirectoryName(string zipPath)
        //{
        //    // don't use Path.GetDirectoryName since we need '/'
        //    var trimmed = zipPath.Trim('/');
        //    var idx = trimmed.LastIndexOf('/');
        //    if (idx > -1)
        //    {
        //        return trimmed.Substring(0, idx);
        //    }
        //    return string.Empty;
        //}

    }
}
