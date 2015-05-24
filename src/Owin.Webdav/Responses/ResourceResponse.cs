using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Responses
{
    public class ResourceResponse
    {
        public ResourceResponse()
        {
            Code = StatusCode.OK;
        }

        public StatusCode Code { get; set; }

        public IResource Resource { get; set; }
    }
}
