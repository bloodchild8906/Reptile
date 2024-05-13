using Microsoft.JSInterop;

namespace Reptile.UI.Data;

public class CookieStorage
{
    private readonly IJSRuntime _jsRuntime;

    public CookieStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

	public async Task<string> GetAsync(string key) => await _jsRuntime.InvokeAsync<string>(JsInteropConstants.GetCookie, key);

	public async void SetAsync<T>(string key, T? value)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync(JsInteropConstants.SetCookie, key, value?.ToString());
        }
        catch
        {
            // ignored
        }
    }
}