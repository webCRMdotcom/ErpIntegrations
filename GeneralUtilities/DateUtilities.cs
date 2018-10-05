using System;

namespace Webcrm.ErpIntegrations.GeneralUtilities
{
    public static class DateUtilities
    {
        public static DateTime FromUtcToSwedish(this DateTime dateTime)
        {
            // TODO FORTNOX: This should be "W. Europe Standard Time". https://docs.microsoft.com/en-us/windows-hardware/manufacture/desktop/default-time-zones.
            // TODO FORTNOX: What about summer time? SupportsDaylightSavingTime should be build in to .Net. https://docs.microsoft.com/en-us/dotnet/api/system.timezoneinfo.supportsdaylightsavingtime?view=netcore-2.1
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            if (timeZoneInfo.IsDaylightSavingTime(dateTime))
                return dateTime.AddHours(2);

            return dateTime.AddHours(1);
        }
    }
}