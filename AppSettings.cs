// Example to show Doppler injected configuration can populate nested classes of configuration values 
public enum Logging
{
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4
}

public class Server
{
    public string HostName { get; set; }
    public string Port { get; set; }
}

public class AppSettings
{
    public string? DopplerProject { get; set; }
    public string? DopplerConfig { get; set; }
    public string? DopplerEnvironment { get; set; }
    public bool Debug { get; set; } = false;
    public Logging Logging { get; set; } = Logging.Error;
    public Server Server { get; set; }
}
