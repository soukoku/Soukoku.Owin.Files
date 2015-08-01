using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Services
{
    /// <summary>
    /// Interface for generating http ETag value.
    /// </summary>
    public interface IETagGenerator
    {
        /// <summary>
        /// Generates the ETag for the specified resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        string Generate(Resource resource);
    }
}
