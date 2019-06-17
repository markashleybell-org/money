using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Money.Support
{
    public static class Globals
    {
#pragma warning disable SA1310 // Field names must not contain underscore
        public const string USER_SESSION_VARIABLE_NAME = "USER_ID";

        public const string PERSISTENT_LOGIN_COOKIE_NAME = "PL";

        public const string PERSISTENT_COOKIE_VALUE_PARSE_ERROR_MESSAGE = "Could not parse persistent session cookie value";

        public const string CONTROLLER_SUFFIX = "Controller";

        public const string COOKIE_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
#pragma warning restore SA1310 // Field names must not contain underscore
    }
}
