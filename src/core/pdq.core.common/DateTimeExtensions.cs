using System;
namespace pdq.common
{
    public static class DateTimeExtensions
    {
        public static int DatePart(this DateTime dateTime, DatePart datePart) => 0;
        public static int Year(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Year);
        public static int Month(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Month);
        public static int Day(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Day);
        public static int Hour(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Hour);
        public static int Minute(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Minute);
        public static int Second(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Second);
        public static int Millisecond(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Millisecond);
        public static long Epoch(this DateTime dateTime) => DatePart(dateTime, common.DatePart.Epoch);
    }
}

