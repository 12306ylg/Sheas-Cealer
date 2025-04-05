using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ona_Core
{
    /// <summary>
    /// Provides HTTP utilities for making asynchronous requests.
    /// </summary>
    public static class Http
    {
        // Static HttpClient instance for better performance and socket management
        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        /// <summary>
        /// Sends a GET request to the specified URI and returns the response as the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the response (string, byte[], Stream, or HttpResponseMessage).</typeparam>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="completionOption">An HTTP completion option value that indicates when the operation should complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task<T> GetAsync<T>(
            string requestUri,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            if (string.IsNullOrEmpty(requestUri))
                throw new ArgumentNullException(nameof(requestUri));

            return GetAsync<T>(requestUri, DefaultHttpClient, completionOption);
        }

        /// <summary>
        /// Sends a GET request to the specified URI using the provided HttpClient.
        /// </summary>
        /// <typeparam name="T">The type of the response (string, byte[], Stream, or HttpResponseMessage).</typeparam>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="httpClient">The HttpClient to use for the request.</param>
        /// <param name="completionOption">An HTTP completion option value that indicates when the operation should complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task<T> GetAsync<T>(
            string requestUri,
            HttpClient httpClient,
            HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            if (string.IsNullOrEmpty(requestUri))
                throw new ArgumentNullException(nameof(requestUri));

            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            try
            {
                if (typeof(T) == typeof(string))
                {
                    string result = await httpClient.GetStringAsync(requestUri).ConfigureAwait(false);
                    return (T)(object)result;
                }
                else if (typeof(T) == typeof(byte[]))
                {
                    byte[] result = await httpClient.GetByteArrayAsync(requestUri).ConfigureAwait(false);
                    return (T)(object)result;
                }
                else if (typeof(T) == typeof(Stream))
                {
                    Stream result = await httpClient.GetStreamAsync(requestUri).ConfigureAwait(false);
                    return (T)(object)result;
                }
                else if (typeof(T) == typeof(HttpResponseMessage))
                {
                    HttpResponseMessage result = await httpClient.GetAsync(requestUri, completionOption).ConfigureAwait(false);
                    return (T)(object)result;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Unsupported return type: {typeof(T).Name}. " +
                        $"Supported types are: string, byte[], Stream, and HttpResponseMessage.");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"HTTP request to '{requestUri}' failed", ex);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is ArgumentNullException)
                    throw;

                throw new InvalidOperationException($"An error occurred processing HTTP request to '{requestUri}'", ex);
            }
        }
    }
}