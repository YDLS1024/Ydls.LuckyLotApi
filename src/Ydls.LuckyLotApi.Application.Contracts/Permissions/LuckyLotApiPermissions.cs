namespace Ydls.LuckyLotApi.Permissions;

public static class LuckyLotApiPermissions
{
    public const string GroupName = "LuckyLotApi";

    public static class NumberThree
    {
        public const string Default = GroupName + ".NumberThree";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Experts
    {
        public const string Default = GroupName + ".Experts";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class KillNumbers
    {
        public const string Default = GroupName + ".KillNumbers";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
