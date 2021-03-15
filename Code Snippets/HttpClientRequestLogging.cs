using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities
{
    internal class LogToDebugHandler : DelegatingHandler
    {
        private readonly string[] _types = { "html", "text", "xml", "json", "txt", "x-www-form-urlencoded" };

        public LogToDebugHandler(HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler()) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var req = request;
            var id = Guid.NewGuid().ToString();
            var msg = $"[{id} -  Request ] ";

            Debug.WriteLine($"{msg}========Start==========");
            Debug.WriteLine($"{msg} {req.Method} {req.RequestUri.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
            Debug.WriteLine($"{msg} Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

            foreach (var header in req.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content != null)
            {
                foreach (var (key, value) in req.Content.Headers)
                {
                    Debug.WriteLine($"{msg} {key}: {string.Join(", ", value)}");
                }

                if (req.Content is StringContent || IsTextBasedContentType(req.Headers) || IsTextBasedContentType(req.Content.Headers))
                {
                    var result = await req.Content.ReadAsStringAsync().ConfigureAwait(false);

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {string.Join("", result.Take(255))}...");
                }
            }

            var start = DateTime.Now;

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            var end = DateTime.Now;

            Debug.WriteLine($"{msg} Duration: {end - start}");
            Debug.WriteLine($"{msg}==========End==========");

            msg = $"[{id} - Response ] ";
            Debug.WriteLine($"{msg}=========Start=========");

            var resp = response;

            Debug.WriteLine($"{msg} {req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int) resp.StatusCode} {resp.ReasonPhrase}");

            foreach (var header in resp.Headers)
            {
                Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content != null)
            {
                foreach (var header in resp.Content.Headers)
                {
                    Debug.WriteLine($"{msg} {header.Key}: {string.Join(", ", header.Value)}");
                }

                if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) || IsTextBasedContentType(resp.Content.Headers))
                {
                    start = DateTime.Now;
                    var result = await resp.Content.ReadAsStringAsync();
                    end = DateTime.Now;

                    Debug.WriteLine($"{msg} Content:");
                    Debug.WriteLine($"{msg} {string.Join("", result.Take(255))}...");

                    try
                    {
                        await File.WriteAllTextAsync(id + ".response", result, cancellationToken);
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }

                    Debug.WriteLine($"{msg} Duration: {end - start}");
                }
            }

            Debug.WriteLine($"{msg}==========End==========");
            return response;
        }

        private bool IsTextBasedContentType(HttpHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out var values))
            {
                return false;
            }

            var header = string.Join(" ", values).ToLowerInvariant();

            Debug.WriteLine($"{nameof(IsTextBasedContentType)} Header: {header}");

            return _types.Any(t =>
            {
                var res = header.Contains(t);

                // Debug.WriteLine($"{nameof(IsTextBasedContentType)} Type: {t}");
                // Debug.WriteLine($"{nameof(IsTextBasedContentType)} Result: {res}");

                return res;
            });
        }
    }
    
    public class HttpRequestToCurlHandler : DelegatingHandler
    {
        public static bool LogToFile = false;

        public HttpRequestToCurlHandler(HttpMessageHandler innerHandler = null) : base(innerHandler ?? new HttpClientHandler()) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string content = null;
            if (request.Content != null)
            {
                content = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            // We use this one, because if there's delegate handlers that add additional headers or so one, this will
            // contain all the manipulations.
            var req = response.RequestMessage;

            var sb = new StringBuilder();

            sb.Append($"curl -v --request {req.Method.Method} {req.RequestUri}");
            foreach (var (key, value) in req.Headers)
            {
                sb.Append($" \\\n\t --header '{key}: {string.Join(" ", value)}'");
            }

            sb.Append(" \\\n\t --header 'Content-Type: application/json'");

            if (!string.IsNullOrWhiteSpace(content))
            {
                sb.Append($" \\\n\t --data-raw '{content}'");
            }

            Console.WriteLine(sb.ToString());
            if (LogToFile)
            {
                await File.WriteAllTextAsync($"{Guid.NewGuid():N}.txt", sb.ToString(), cancellationToken).ConfigureAwait(false);
            }

            return response;
        }
    }
}
