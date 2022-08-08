using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using static System.String;

namespace DopplerSDK.ConfigurationProvider;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
using DopplerSecrets = Dictionary<string, string>;

public static class DopplerNameTransformers
{
    public static readonly string DotNet = "dotnet";
    public static readonly string DotNetEnv = "dotnet-env";
    public static readonly string None = "none";
    public static readonly List<string> List = new() { DotNet, DotNetEnv, None };

    public static bool Validate(string nameTransformer)
    {
        return List.Contains(nameTransformer);
    }
}

public class DopplerClientConfiguration
{
    public string? DopplerToken { get; set; } = Empty;
    public string DopplerNameTransformer { get; set; } = DopplerNameTransformers.DotNet;
    /// <summary>
    /// Fetches a token at runtime using the local Doppler CLI. "doppler setup" must be called before use.
    /// 
    /// Defaults to false.
    /// </summary>
    public bool RuntimeTokenFetch { get; set; } = false;

    internal async ValueTask<string> GetToken()
    {
        // 1. Check explicit configuration
        if (!IsNullOrEmpty(DopplerToken))
            return DopplerToken;


        // 2. See if the doppler client is available to generate us a token
        if (RuntimeTokenFetch)
        {
            // this has been tested to work on windows via a scoop-installed CLI but may require different logic for other platforms.
            var processInfo = new ProcessStartInfo("doppler")
            {
                Arguments = "configs tokens create dotnet-runtime-dev-token --max-age 1m --plain",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            try
            {
                using var process = Process.Start(processInfo);
                if (process is not null)
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    // do something with error?
                    var error = await process.StandardError.ReadToEndAsync();
                    return output;
                }

            }
            catch (Exception)
            {
                // CLI is not installed or unavailable
            }
        }

        return Empty;
    }
}

public class DopplerClientResponse
{
    public readonly DopplerSecrets Secrets;
    public readonly string StatusMessage;
    public bool IsSuccess => Secrets.Count > 0;

    public DopplerClientResponse(DopplerSecrets? secrets, string? statusMessage)
    {
        Secrets = secrets ?? new DopplerSecrets();
        StatusMessage = statusMessage ?? Empty;
    }
}

public class DopplerClient : IDisposable
{
    private readonly HttpClient _httpClient;
    public string ApiHost = "https://api.doppler.com";
    public string ApiPath = "/v3/configs/config/secrets/download";
    public DopplerClientConfiguration DopplerClientConfiguration;

    public DopplerClient(DopplerClientConfiguration dopplerClientConfiguration)
    {
        _httpClient = new HttpClient();
        DopplerClientConfiguration = dopplerClientConfiguration;
    }

    private Uri ClientUrl()
    {
        var clientUrl = $"{ApiHost}/{ApiPath}?format=json" +
                        (DopplerClientConfiguration.DopplerNameTransformer != DopplerNameTransformers.None
                            ? $"&name_transformer={DopplerClientConfiguration.DopplerNameTransformer}"
                            : Empty);
        return new Uri(clientUrl);
    }

    private AuthenticationHeaderValue AuthHeader(string token)
    {
        var basicAuthHeader =
            Convert.ToBase64String(Encoding.Default.GetBytes(token + ":"));
        return new AuthenticationHeaderValue("Basic", basicAuthHeader);
    }

    public async Task<DopplerClientResponse> FetchSecretsAsync()
    {
        var token = await DopplerClientConfiguration.GetToken();

        if (IsNullOrEmpty(token))
            return new DopplerClientResponse(null,
                "Doppler Client Error: DopplerClientConfiguration.DopplerToken not set");

        if (!DopplerNameTransformers.Validate(DopplerClientConfiguration.DopplerNameTransformer))
            return new DopplerClientResponse(null,
                $"Doppler Client Error: DopplerClientConfiguration.DopplerNameTransformer must be one of {DopplerNameTransformers.List}");

        var clientUrl = ClientUrl();
        var authHeader = AuthHeader(token);
        _httpClient.DefaultRequestHeaders.Authorization = authHeader;

        try
        {
            var httpResponseMessage = await _httpClient.GetAsync(clientUrl);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var secrets = await httpResponseMessage.Content.ReadFromJsonAsync<DopplerSecrets>();
                return new DopplerClientResponse(secrets, "Ok");
            }

            var httpContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var errorMessage = $"$Doppler API Error: {httpResponseMessage.StatusCode} {httpContent}";
            return new DopplerClientResponse(null, errorMessage);
        }
        catch (HttpRequestException httpRequestException)
        {
            return new DopplerClientResponse(null,
                $"Doppler Client HttpRequestException: {httpRequestException.Message}");
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}