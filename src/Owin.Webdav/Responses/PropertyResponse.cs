using Soukoku.Owin.Webdav.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Webdav.Responses
{
    public class PropertyResponse
    {
        public StatusCode Code { get; set; }
        public IProperty Property { get; set; }
    }
}
