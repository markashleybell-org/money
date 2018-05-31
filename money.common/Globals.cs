using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace money.common
{
    public static class Globals
    {
#pragma warning disable SA1310 // Field names must not contain underscore
        public const string USER_SESSION_VARIABLE_NAME = "USER_ID";
        public const string CONTROLLER_SUFFIX = "Controller";
#pragma warning restore SA1310 // Field names must not contain underscore
    }
}
