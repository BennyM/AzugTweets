using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR.Owin;
using Microsoft.AspNet.SignalR.Infrastructure;

[assembly: OwinStartup(typeof(AzugTweetsWeb.StartApp))]

namespace AzugTweetsWeb
{
    public class StartApp
    {
        public void Configuration(IAppBuilder app)
        {
            Microsoft.AspNet.SignalR.GlobalHost.DependencyResolver.Register(typeof(IProtectedData), () => new ProtectedData());
            app.MapSignalR();
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.MapHttpAttributeRoutes();
            app.UseWebApi(httpConfiguration);
        }
    }

    public class ProtectedData : IProtectedData
    {

        // Obviously this isn't doing much to protect the data,
        // assume custom encryption required here

        // To reiterate, no encryption is VERY^4 BAD, see comments.

        public string Protect(string data, string purpose)
        {
            return data;
        }

        public string Unprotect(string protectedValue, string purpose)
        {
            return protectedValue;
        }
    }

}
