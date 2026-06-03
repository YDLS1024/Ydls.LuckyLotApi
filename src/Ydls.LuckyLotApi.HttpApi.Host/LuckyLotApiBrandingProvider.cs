using Microsoft.Extensions.Localization;
using Ydls.LuckyLotApi.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Ydls.LuckyLotApi;

[Dependency(ReplaceServices = true)]
public class LuckyLotApiBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<LuckyLotApiResource> _localizer;

    public LuckyLotApiBrandingProvider(IStringLocalizer<LuckyLotApiResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
