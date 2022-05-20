namespace SecretOps.DopplerClient;

// ReSharper disable once FieldCanBeMadeReadOnly.Global
public class DopplerClientResponseConsoleDebugger
{
    private readonly DopplerClientResponse _dopplerClientResponse;
    private readonly bool _revealSecretValues;

    private readonly IEnumerable<string> _unmaskedSecretKeys = new List<string>
    {
        "DopplerProject", "DopplerEnvironment", "DopplerConfig", "DOPPLER_PROJECT", "DOPPLER_ENVIRONMENT", "DOPPLER_CONFIG"
    };

    public DopplerClientResponseConsoleDebugger(DopplerClientResponse dopplerClientResponse, bool revealSecretValues = false,
        IEnumerable<string>? unmaskedSecretKeys = null)
    {
        _revealSecretValues = revealSecretValues;
        _dopplerClientResponse = dopplerClientResponse;
        if (unmaskedSecretKeys != null) _unmaskedSecretKeys = _unmaskedSecretKeys.Concat(unmaskedSecretKeys);
    }

    public void Write()
    {
        var divider = new string('-', 80);

        Console.WriteLine(
            $"{divider}\nDoppler Client Response Debug Output\nSuccess: {_dopplerClientResponse.IsSuccess}\nStatusMessage: {_dopplerClientResponse.StatusMessage}\n{divider}");

        if (!_dopplerClientResponse.IsSuccess) return;

        foreach (var secret in _dopplerClientResponse.Secrets)
        {
            var secretValue = _revealSecretValues || _unmaskedSecretKeys.Contains(secret.Key)
                ? secret.Value
                : new string('*', secret.Value.Length);
            Console.WriteLine($"{secret.Key}: {secretValue}");
        }

        Console.WriteLine(divider);
    }
}