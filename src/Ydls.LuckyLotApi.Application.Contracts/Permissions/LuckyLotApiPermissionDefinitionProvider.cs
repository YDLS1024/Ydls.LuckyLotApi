using Ydls.LuckyLotApi.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ydls.LuckyLotApi.Permissions;

public class LuckyLotApiPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(LuckyLotApiPermissions.GroupName, L("Permission:LuckyLotApi"));

        var numberThree = group.AddPermission(LuckyLotApiPermissions.NumberThree.Default, L("Permission:NumberThree"));
        numberThree.AddChild(LuckyLotApiPermissions.NumberThree.Create, L("Permission:NumberThree.Create"));
        numberThree.AddChild(LuckyLotApiPermissions.NumberThree.Edit, L("Permission:NumberThree.Edit"));
        numberThree.AddChild(LuckyLotApiPermissions.NumberThree.Delete, L("Permission:NumberThree.Delete"));

        var experts = group.AddPermission(LuckyLotApiPermissions.Experts.Default, L("Permission:Experts"));
        experts.AddChild(LuckyLotApiPermissions.Experts.Create, L("Permission:Experts.Create"));
        experts.AddChild(LuckyLotApiPermissions.Experts.Edit, L("Permission:Experts.Edit"));
        experts.AddChild(LuckyLotApiPermissions.Experts.Delete, L("Permission:Experts.Delete"));

        var killNumbers = group.AddPermission(LuckyLotApiPermissions.KillNumbers.Default, L("Permission:KillNumbers"));
        killNumbers.AddChild(LuckyLotApiPermissions.KillNumbers.Create, L("Permission:KillNumbers.Create"));
        killNumbers.AddChild(LuckyLotApiPermissions.KillNumbers.Edit, L("Permission:KillNumbers.Edit"));
        killNumbers.AddChild(LuckyLotApiPermissions.KillNumbers.Delete, L("Permission:KillNumbers.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LuckyLotApiResource>(name);
    }
}
