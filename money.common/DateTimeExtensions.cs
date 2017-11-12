﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace money.common
{
    public static partial class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt)
        {
            return dt.FirstDayOfWeek().AddDays(6);
        }

        public static DateTime FirstDayOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static DateTime FirstDayOfNextMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1);
        }

        public static DateTime SetTime(this DateTime dt, int hour, int minute, int second)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, hour, minute, second, 0);
        }

        public static DateTime SetTime(this DateTime dt, int hour, int minute, int second, int millisecond)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, hour, minute, second, millisecond);
        }

        public static DateTime ZeroTime(this DateTime dt)
        {
            return SetTime(dt, 0, 0, 0, 0);
        }

        public static bool IsLastDayOfMonth(this DateTime dt)
        {
            return dt.Date == dt.FirstDayOfMonth().AddMonths(1).AddDays(-1).Date;
        }
    }
}
