using System;

namespace Maxi.Services.SouthSide.SouthsideFile
{
    public static class Converters
    {
        public static string ToWfDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        public static string ToWfTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("HHmm");
        }

        public static string ToWfDecimalString(this Decimal d)
        {
            return d.ToString("N2").Replace(".", string.Empty).Replace(",", string.Empty).Replace("-", string.Empty);
        }

        public static string CashLetterId(DateTime currentDate)
        {
            return string.Format("ADV{0}{1}", (object)currentDate.ToString("tt").Substring(0, 1), (object)currentDate.ToString("mmss"));
        }

        public static string BundleSequenceNumber(int index)
        {
            return (index + 1).ToString().PadLeft(4, '0');
            //return (index).ToString().PadLeft(4, '0');
        }
    }
}
