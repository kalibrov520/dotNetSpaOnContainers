using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiGw_Base
{
    public class Destination
    {
        public string Uri { get; set; }
        public bool RequiresAuthentication { get; set; }

        public Destination(string uri, bool requiresAuthentication)
        {
            Uri = uri;
            RequiresAuthentication = requiresAuthentication;
        }

        public Destination(string uri) : this(uri, false) { }

        private Destination()
        {
            Uri = "/";
            RequiresAuthentication = false;
        }

        private string CreateDestinationUri(HttpRequest request)
        {
            string requestPath = request.Path.ToString();
            string queryString = request.QueryString.ToString();

            string endpoint = "";
            string[] endpointSplit = requestPath.Substring(1).Split("/");

            if (endpointSplit.Length > 1)
                endpoint = endpointSplit[1];

            return Uri + endpoint + queryString;
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequest request)
        {
            string requestContent;
            using (var receiveStream = request.Body)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    requestContent = readStream.ReadToEnd();
                }
            }

            HttpClient client = new HttpClient();
            HttpRequestMessage newRequest =
                new HttpRequestMessage(new HttpMethod(request.Method), CreateDestinationUri(request));
            HttpResponseMessage response = await client.SendAsync(newRequest);

            return response;
        }
    }
}