using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace SampleApp.Pages;

public class IndexModel : PageModel
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    public AppSettings AppSettings { get; set; }

    public void OnGet()
    {
        AppSettings = _appSettings;
    }
}