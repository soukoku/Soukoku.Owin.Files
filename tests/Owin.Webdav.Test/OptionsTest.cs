using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Soukoku.Owin.Webdav;

namespace Owin.Webdav.Test
{
    [TestClass]
    public class OptionsTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            using (var server = TestServer.Create(app =>
            {
                app.UseWebdav(new WebdavConfig(new FakeDataStore()) { });
            }))
            {
                HttpResponseMessage response = await server.CreateRequest("/")
                                               .AddHeader("header1", "headervalue1")
                                               .GetAsync();
                // TODO: Validate response
                Assert.Fail("Not implemented.");
            }
        }
    }
}
