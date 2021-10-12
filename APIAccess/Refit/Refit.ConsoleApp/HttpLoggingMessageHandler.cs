using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Refit.ConsoleApp
{
    /// <summary>
    /// This message handler is used for logging the Refit requests and responses to the log
    /// <para>
    /// Based off of this Github issue: <a href="https://github.com/reactiveui/refit/issues/258">https://github.com/reactiveui/refit/issues/258</a>
    /// </para>
    /// </summary>
    public class HttpLoggingMessageHandler : DelegatingHandler
    {
        private readonly string[] _types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };
        private readonly ILogger<HttpLoggingMessageHandler> _logger;

        public HttpLoggingMessageHandler(ILogger<HttpLoggingMessageHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = await GetRequestBody(request, cancellationToken);

            _logger.LogDebug("RequestUri: {RequestUri} Request: {@Request}", request.RequestUri, body);

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var responseContent = await GetResponseContent(response, cancellationToken);
            
            _logger.LogDebug("RequestUri: {RequestUri} Response {@Response}", request.RequestUri, responseContent);

            return response;
        }

        private async Task<object> GetResponseContent(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.Content is StringContent || IsTextBasedContentType(response.Headers) ||
                IsTextBasedContentType(response.Content.Headers))
            {
                return await response.Content.ReadAsStringAsync(cancellationToken);
            }

            return response;
        }

        private async Task<string> GetRequestBody(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = "";
            if (request.Content != null && (request.Content is StringContent || IsTextBasedContentType(request.Headers) ||
                                            IsTextBasedContentType(request.Content.Headers)))
            {
                body = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return body;
        }


        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out var values))
                return false;

            var headerTypes = values.Select(x => x.Trim().ToLowerInvariant()).ToList();
            return _types.Any(x => headerTypes.Any(y => y.Contains(x)));
        }
    }
}