using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiGw_Base.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ApiGw_Base
{
    public class Route
    {
        public string Endpoint { get; set; }
        public Destination Destination { get; set; }
    }

    public class Router
    {
        public List<Route> Routes { get; set; }
        public Destination AuthenticationService { get; set; }

        public Router(string routeConfigFilePath)
        {
            var router = JsonLoader.LoadFromFile<dynamic>(routeConfigFilePath);

            Routes = JsonLoader.Deserialize<List<Route>>(Convert.ToString(router.routes));

            AuthenticationService = JsonLoader.Deserialize<Destination>(Convert.ToString(router.authenticationService));
        }

        public async Task<HttpResponseMessage> RouteRequest(HttpRequest request)
        {
            var path = request.Path.ToString();
            var basePath = '/' + path.Split('/')[1];

            Destination destination;
            try
            {
                destination = Routes.First(r => r.Endpoint.Equals(basePath)).Destination;
            }
            catch
            {
                return ConstructErrorMessage("The path could not be found.");
            }

            if (destination.RequiresAuthentication)
            {
                string token = request.Headers["token"];
                request.Query.Append(new KeyValuePair<string, StringValues>("token", new StringValues(token)));
                var authResponse = await AuthenticationService.SendRequest(request);

                if (!authResponse.IsSuccessStatusCode)
                    return ConstructErrorMessage("Authentication failed.");
            }

            return await destination.SendRequest(request);
        }

        private HttpResponseMessage ConstructErrorMessage(string error)
        {
            var errorMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(error)
            };
            return errorMessage;
        }
    }
}