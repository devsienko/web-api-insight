using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace WebApiMonitor.Administrator
{
    public class ProxyController : ApiController
    {
        [AcceptVerbs(WebRequestMethods.Http.Get, WebRequestMethods.Http.Head, WebRequestMethods.Http.MkCol, WebRequestMethods.Http.Post, WebRequestMethods.Http.Put)]
        public async Task<HttpResponseMessage> Get(string url)
        {
            return await Proxy(@"http://localhost:8888/sources/1/dashboards");
        }

        //[Route("chronograf/v1")]
        //[HttpGet]
        //public async Task<HttpResponseMessage> Chronograf_V1()
        //{
        //    return await Proxy(@"http://localhost:8888/chronograf/v1");
        //}

        //[Route("chronograf/v1/me")]
        //[HttpGet]
        //public async Task<HttpResponseMessage> Chronograf_V1_Me()
        //{
        //    return await Proxy(@"http://localhost:8888/chronograf/v1/me");
        //}

        private async Task<HttpResponseMessage> Proxy(string url)
        {
            using (HttpClient http = new HttpClient())
            {
                this.Request.RequestUri = new Uri(url);

                if (this.Request.Method == HttpMethod.Get)
                {
                    this.Request.Content = null;
                }
                var response = await http.SendAsync(this.Request);
                var m = await response.Content.ReadAsStringAsync();
                return response;
            }
        }
    }
    //public class ProxyHandler : IHttpHandler, IRouteHandler
    //{
    //    public bool IsReusable
    //    {
    //        get { return true; }
    //    }

    //    public void ProcessRequest(HttpContext context)
    //    {
    //        string ipAddress = context.Request.QueryString["ipaddress"];

    //        string str = string.Format(@"http://localhost:8888/sources/1/dashboards");

    //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(str);
    //        request.Method = "GET";
    //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //        StreamReader reader = new StreamReader(response.GetResponseStream());

    //        HttpResponse res = context.Response;
    //        res.ContentType = "application/xml; charcode=utf8";
    //        res.StatusCode = 200;
    //        res.Write(reader.ReadToEnd());
    //    }

    //    public IHttpHandler GetHttpHandler(RequestContext requestContext)
    //    {
    //        return this;
    //    }
    //}
}