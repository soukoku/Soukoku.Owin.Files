using Soukoku.Owin.Webdav.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Interface for managing <see cref="IResource"/>.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Gets the resource at the specified logical path.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logicalPath">The logical path.</param>
        /// <returns></returns>
        ResourceResponse GetResource(DavContext context, string logicalPath);

        /// <summary>
        /// Gets the resources under a collection resource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="collectionResource">The collection resource.</param>
        /// <returns></returns>
        IEnumerable<ResourceResponse> GetSubResources(DavContext context, IResource collectionResource);
        
        /// <summary>
        /// Creates a sub collection if applicable.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        StatusCode CreateCollection(IResource parent, string name);

        
        IEnumerable<ResourceResponse> CopyTo(IResource resource, string targetPath);
        IEnumerable<ResourceResponse> MoveTo(IResource resource, string targetPath);
        ResourceResponse Delete(IResource resource);
        
        /// <summary>
        /// Opens the the resource stream for reading if applicable.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        Stream Read(IResource resource);

        /// <summary>
        /// Write the source stream content into the resource if applicable.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="source">The source.</param>
        /// <param name="offset">The starting offset to write.</param>
        /// <returns></returns>
        StatusCode Write(IResource resource, Stream source, int offset);


        IEnumerable<DavLock> GetLocks(IResource resource);
        DavLock Lock(IResource resource, DavLock proposed);
        DavLock RefreshLock(IResource resource, DavLock proposed);
        StatusCode Unlock(IResource resource, DavLock currentLock);
    }
}
