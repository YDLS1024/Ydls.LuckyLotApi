using Ydls.LuckyLotApi.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ydls.LuckyLotApi.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class LuckyLotApiController : AbpControllerBase
{
    protected LuckyLotApiController()
    {
        LocalizationResource = typeof(LuckyLotApiResource);
    }
}
