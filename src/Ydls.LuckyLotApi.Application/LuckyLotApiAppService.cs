using Ydls.LuckyLotApi.Localization;
using Volo.Abp.Application.Services;

namespace Ydls.LuckyLotApi;

/* Inherit your application services from this class.
 */
public abstract class LuckyLotApiAppService : ApplicationService
{
    protected LuckyLotApiAppService()
    {
        LocalizationResource = typeof(LuckyLotApiResource);
    }
}
