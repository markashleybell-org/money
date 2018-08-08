namespace money.common
{
    public static class Globals
    {
#pragma warning disable SA1310 // Field names must not contain underscore
        public const string USER_SESSION_VARIABLE_NAME = "USER_ID";

        public const string PERSISTENT_LOGIN_COOKIE_NAME = "PL";

        public const string CONTROLLER_SUFFIX = "Controller";

        public const string COOKIE_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
#pragma warning restore SA1310 // Field names must not contain underscore
    }
}
