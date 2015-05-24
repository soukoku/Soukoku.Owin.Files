using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soukoku.Owin.Webdav.Models;
using System.Net;
using System.Globalization;

namespace Soukoku.Owin.Webdav.DefaultImp
{
    class BootstrapDirGenerator : IDirectoryListingGenerator
    {
        public async Task<string> GenerateAsync(Context context, IResource parentResource, IEnumerable<IResource> childResources)
        {
            var rows = new StringBuilder();
            if (childResources != null)
            {
                foreach (var item in childResources.OrderByDescending(r => r.ResourceType).ThenBy(r => r.DisplayName))
                {
                    rows.Append("<tr>");
                    var url = context.GenerateUrl(item, true);
                    if (item.ResourceType == ResourceType.Collection)
                    {
                        rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{2}</td><td></td><td><span class=\"text-warning glyphicon glyphicon-folder-close\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                    }
                    else
                    {
                        rows.AppendFormat(string.Format(CultureInfo.InvariantCulture, "<td>{3}</td><td class=\"text-right\">{2}</td><td><span class=\"text-info glyphicon glyphicon-file\"></span>&nbsp;<a href=\"{0}\">{1}</a></td>", WebUtility.HtmlEncode(Uri.EscapeUriString(url)), WebUtility.HtmlEncode(item.DisplayName), item.Length.PrettySize(), item.ModifiedDateUtc.ToString("yyyy/MM/dd hh:mm tt")));
                    }
                    rows.Append("</tr>");
                }
            }

            var title = WebUtility.HtmlEncode(parentResource.PathBase + parentResource.LogicalPath);
            var content = string.Format(CultureInfo.InvariantCulture, await GetDirectoryListingTemplateAsync(), title, rows);
            return content;
        }

        static string _template;
        static async Task<string> GetDirectoryListingTemplateAsync()
        {
            if (_template == null)
            {
                _template = await typeof(WebdavMiddleware).Assembly.GetManifestResourceStream("Soukoku.Owin.Webdav.Responses.DirectoryListing.html").ReadStringAsync();
            }
            return _template;
        }
    }
}
