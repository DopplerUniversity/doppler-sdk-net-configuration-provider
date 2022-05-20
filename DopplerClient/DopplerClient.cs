using System.Net.Http.Headers;
using System.Text;
using static System.String;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

namespace SecretOps.DopplerClient;

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
    public string? DopplerToken { get; init; } = Empty;
    public string DopplerNameTransformer { get; init; } = DopplerNameTransformers.DotNet;
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

public class DopplerClient
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

    private AuthenticationHeaderValue AuthHeader()
    {
        var basicAuthHeader =
            Convert.ToBase64String(Encoding.Default.GetBytes(DopplerClientConfiguration.DopplerToken + ":"));
        return new AuthenticationHeaderValue("Basic", basicAuthHeader);
    }

    public async Task<DopplerClientResponse> FetchSecretsAsync()
    {
        if (IsNullOrEmpty(DopplerClientConfiguration.DopplerToken))
            return new DopplerClientResponse(null,
                "Doppler Client Error: DopplerClientConfiguration.DopplerToken not set");

        if (!DopplerNameTransformers.Validate(DopplerClientConfiguration.DopplerNameTransformer))
            return new DopplerClientResponse(null,
                $"Doppler Client Error: DopplerClientConfiguration.DopplerNameTransformer must be one of {DopplerNameTransformers.List}");

        var clientUrl = ClientUrl();
        var authHeader = AuthHeader();
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
}