using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Services.BuiltIn
{
    /// <summary>
    /// ETag generator that uses the modified date.
    /// </summary>
    public class ModifiedDateETagGenerator : IETagGenerator
    {
        /// <summary>
        /// Generates the ETag for the specified resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        public string Generate(Resource resource)
        {
            if (resource == null) { return string.Empty; }

            return resource.ModifiedDateUtc.Ticks.ToString(CultureInfo.InvariantCulture);
        }
    }
}
