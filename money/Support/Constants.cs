namespace Money.Support
{
    public static class Constants
    {
#pragma warning disable SA1310 // Field names must not contain underscore
        public const string CONTROLLER_SUFFIX = "Controller";

        public const string DESCRIPTION_DELIMITER_START = "|~";

        public const string DESCRIPTION_DELIMITER_END = "~|";

        public const string DESCRIPTION_DELIMITER_REGEX = @"/\s+\|~(.*?)~\|/gi";
#pragma warning restore SA1310 // Field names must not contain underscore
    }
}
