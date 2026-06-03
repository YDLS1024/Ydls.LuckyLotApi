using Ydls.LuckyLotApi.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Ydls.LuckyLotApi.Permissions;

public class LuckyLotApiPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(LuckyLotApiPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(LuckyLotApiPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LuckyLotApiResource>(name);
    }
}
