using System.Text;

namespace SAPB1SLayerWebAPI.Utils
{
    public class HttpHelper
    {

        private readonly IHttpClientFactory _clientFactory;
        public HttpHelper(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<HttpResponseMessage> PostRequest(string endPoint, string clientName, string body)
        {
            try
            {
                var client = _clientFactory.CreateClient(clientName);
                var response = await client.PostAsync(endPoint, new StringContent(body, Encoding.UTF8, "application/json"));
                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Something went wrong. {ex.Message}");
            }
        }

        public async Task<HttpResponseMessage> GetRequest(string endPoint, string clientName)
        {
            try
            {
                var client = _clientFactory.CreateClient(clientName);
                var response = await client.GetAsync(endPoint);
                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Something went wrong. {ex.Message}");
            }
        }
    }
}
