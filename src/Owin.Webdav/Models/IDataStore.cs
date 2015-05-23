﻿using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Interface for <see cref="IResource"/> data storage.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Gets the resource at the specified logical path.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logicalPath">The logical path.</param>
        /// <returns></returns>
        IResource GetResource(IOwinContext context, string logicalPath);

        /// <summary>
        /// Gets the resources under a collection resource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="collectionResource">The collection resource.</param>
        /// <returns></returns>
        IEnumerable<IResource> GetSubResources(IOwinContext context, IResource collectionResource);
    }
}
