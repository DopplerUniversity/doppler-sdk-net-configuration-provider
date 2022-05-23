using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DopplerClientSampleApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppSettings _appSettings;

    public AppSettings AppSettings { get; set; }

    public IndexModel(ILogger<IndexModel> logger, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _appSettings = appSettings.Value;

    }

    public void OnGet()
    {
        AppSettings = _appSettings;
    }
}
