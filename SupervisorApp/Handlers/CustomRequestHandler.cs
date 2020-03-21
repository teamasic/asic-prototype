using CefSharp;
using CefSharp.Handler;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupervisorApp.Handler
{
    public class CustomRequestHandler : CefSharp.Handler.RequestHandler
    {
        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new CustomResourceRequestHandler();
        }
    }

    class CustomResourceRequestHandler : ResourceRequestHandler
    {
        private const string userAgent
            = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36 Edg/80.0.361.69";
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, 
            IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            request.SetHeaderByName("user-agent", userAgent, true);

            return CefReturnValue.Continue;
        }

    }
}
