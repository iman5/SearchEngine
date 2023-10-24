using System;
using System.Globalization;

namespace SearchEngine.API.Helpers;

/// <summary>
/// Helper class to convert the DateTimeToString of yyyyMMddHHmmss and vice-versa.
/// </summary>
public static class DateTimeHelper
{
    public static string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }

    public static DateTime StringToDateTime(string str)
    {
        return DateTime.ParseExact(str, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }
}
