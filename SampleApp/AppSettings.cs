// Example to show Doppler injected configuration can populate nested classes of configuration values 

// A production example would need to decide on defaults, which fields are required vs. nullable and empty

namespace SampleApp;

public enum Logging
{
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4
}

public class Smtp
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string FromAddress { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}

public interface ISecret<T>
{
    public bool Encrypted { get; init; }
    public string Encryption { get; init; }
    public T Value { get; init; }
}

public class ApiKeySecret : ISecret<string>
{
    public bool Encrypted { get; init; }
    public string Encryption { get; init; }
    public string Value { get; init; }
}

public class AppSettings
{
    public string DopplerProject { get; init; }
    public string DopplerConfig { get; init; }
    public string DopplerEnvironment { get; init; }

    public ApiKeySecret ApiKey { get; init; }
    public bool Debug { get; init; } = false;
    public Logging Logging { get; init; } = Logging.Error;
    public Smtp Smtp { get; init; }
}