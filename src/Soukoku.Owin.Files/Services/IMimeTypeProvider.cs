using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files.Services
{
    /// <summary>
    /// Interface for registring and getting mime types.
    /// </summary>
    public interface IMimeTypeProvider
    {
        /// <summary>
        /// Gets the mime type of a file with the specified extension (including dot).
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns></returns>
        string GetMimeType(string extension);

        /// <summary>
        /// Registers an extension's mime type.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        void Register(string extension, string mimeType);
    }
}
