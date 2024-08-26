using System;
using System.Xml.Linq;

namespace Maxi.Services.SouthSide.Utils
{
    public static class ErrorUtils
    {
        private const int MaxInnerExceptionDeepness = 15;

        public static string FormatError(string message, Exception ex, string serviceCode)
        {
            string exceptionMessage;
            string stackTrace;
            ErrorUtils.GetExceptionDetail(ex, out exceptionMessage, out stackTrace);
            return string.Format("Service {0} Error: {1} {4}Exception:{4}{2}; {4}StackTrance:{4}{3};", (object)serviceCode, (object)message, (object)exceptionMessage, (object)stackTrace, (object)Environment.NewLine);
        }

        private static void GetExceptionDetail(
          Exception ex,
          out string exceptionMessage,
          out string stackTrace)
        {
            XElement xmlMessage = new XElement((XName)"Detail");
            XElement xmlStackTrance = new XElement((XName)"Detail");
            ErrorUtils.GetInnerExceptionDetail(ex, ref xmlMessage, ref xmlStackTrance, 0);
            exceptionMessage = xmlMessage.ToString();
            stackTrace = xmlStackTrance.ToString();
        }

        private static void GetInnerExceptionDetail(
          Exception ex,
          ref XElement xmlMessage,
          ref XElement xmlStackTrance,
          int deepness)
        {
            if (ex == null || deepness >= 15)
                return;
            xmlMessage.Add((object)new XElement((XName)"Exception", (object)(ex.Message ?? string.Empty)));
            xmlStackTrance.Add((object)new XElement((XName)"StackTrace", (object)(ex.StackTrace ?? string.Empty)));
            ++deepness;
            ErrorUtils.GetInnerExceptionDetail(ex.InnerException, ref xmlMessage, ref xmlStackTrance, deepness);
        }
    }
}
