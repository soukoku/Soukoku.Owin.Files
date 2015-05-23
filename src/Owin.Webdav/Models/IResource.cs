using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Models
{
    /// <summary>
    /// Represents a webdav resource.
    /// </summary>
    public interface IResource
    {
        string LogicalPath { get; }

        IEnumerable<IDavProperty> Properties { get; }

        //T FindProperty<T>(string name, string namespaceUri) where T : class, IDavProperty;

        //void AddProperties(IEnumerable<IDavProperty> properties);
        //void AddProperty(IDavProperty davProperty);

        Stream OpenReadStream();

        #region built-in webdav properties

        string DisplayName { get; }
        long Length { get; }
        string ContentType { get; }
        string ContentLanguage { get; }
        DateTime CreationDateUtc { get; }
        DateTime ModifiedDateUtc { get; }
        ResourceType Type { get; }
        string ETag { get; }

        #endregion
    }

    /// <summary>
    /// Indicates the webdav resource type.
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// A simple resource.
        /// </summary>
        Resource,
        /// <summary>
        /// A resource collection.
        /// </summary>
        Collection,
    }
}
