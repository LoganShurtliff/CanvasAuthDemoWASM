using Microsoft.JSInterop;

namespace TokenTestingBlazor.Client
{
    /// <summary>
    /// Utility Class to Access Cookies
    /// </summary>
    public class CookieStorageAccessor
    {
        private Lazy<IJSObjectReference> _accessorJsRef = new();
        private readonly IJSRuntime _jsRuntime;

        public CookieStorageAccessor(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task WaitForReference()
        {
            if (_accessorJsRef.IsValueCreated is false)
            {
                _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/CookieStorageAccessor.js"));
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_accessorJsRef.IsValueCreated)
            {
                await _accessorJsRef.Value.DisposeAsync();
            }
        }

        /// <summary>
        /// Gets a value from a cookie
        /// </summary>
        /// <typeparam name="T">Type of object being accessed</typeparam>
        /// <param name="key">Key of the cookie</param>
        /// <returns>The value being access</returns>
        public async Task<T> GetValueAsync<T>(string key)
        {
            await WaitForReference();
            var result = await _accessorJsRef.Value.InvokeAsync<T>("get", key);

            return result;
        }

        /// <summary>
        /// Creates a new cookie
        /// </summary>
        /// <typeparam name="T">Type of data being stored in the cookie</typeparam>
        /// <param name="key">Key of the cookie</param>
        /// <param name="value">Data to store in a cookie</param>
        /// <returns>An async Task</returns>
        public async Task SetValueAsync<T>(string key, T value)
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeVoidAsync("set", key, value);
        }
    }
}
