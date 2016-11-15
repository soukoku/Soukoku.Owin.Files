using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;

namespace Soukoku.Owin.Files.Services.BuiltIn
{
    /// <summary>
    /// An <see cref="IDirectoryListingGenerator"/> that uses bootstrap for look and feel.
    /// </summary>
    public class BootstrapDirectoryListingGenerator : IDirectoryListingGenerator
    {
        public async Task<string> GenerateAsync(OwinContext context, Resource parentResource, IEnumerable<Resource> childResources)
        {
            var rows = new StringBuilder();
            if (childResources != null)
            {
                foreach (var item in childResources.OrderByDescending(r => r.IsFolder).ThenBy(r => r.DisplayName))
                {
                    rows.Append("<tr>");
                    var url = item.GenerateUrl(true);
                    if (item.IsFolder)
                    {
                        rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{2}</td><td></td><td><span class=\"text-warning glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                    }
                    else
                    {
                        rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{3}</td><td class=\"text-right\">{2}</td><td><span class=\"text-info glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.Length.Humanize(), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                    }
                    rows.Append("</tr>");
                }
            }

            var title = WebUtility.HtmlEncode(context.Request.PathBase + parentResource.LogicalPath);
            var content = string.Format(CultureInfo.InvariantCulture, await GetDirectoryListingTemplateAsync().ConfigureAwait(false), title, rows);
            return content;
        }

        static string _template;
        static async Task<string> GetDirectoryListingTemplateAsync()
        {
            if (_template == null)
            {
                _template = await typeof(BootstrapDirectoryListingGenerator).Assembly.GetManifestResourceStream("Soukoku.Owin.Files.Services.BuiltIn.BootstrapDirectoryListing.html").ReadStringAsync().ConfigureAwait(false);
            }
            return _template;
        }
    }
}
