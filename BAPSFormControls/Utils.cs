using System;

namespace BAPSFormControls
{
    public static class Utils
    {
        public static string TimeToString(int hours, int minutes, int seconds, int centiseconds)
        {
            /** WORK NEEDED: fix me **/
            var htemp = hours.ToString();
            var mtemp = (minutes < 10) ? string.Concat("0", minutes.ToString()) : minutes.ToString();
            var stemp = (seconds < 10) ? string.Concat("0", seconds.ToString()) : seconds.ToString();
            return string.Concat(htemp, ":", mtemp, ":", stemp);
        }

        public static string MillisecondsToTimeString(int msecs)
        {
            /** WORK NEEDED: lots **/
            int secs = msecs / 1000;

            var hours = Math.DivRem(secs, 3600, out _);
            int mins = Math.DivRem(secs, 60, out _) - (hours * 60);

            secs = secs - ((mins * 60) + (hours * 3600));

            return TimeToString(hours, mins, secs, msecs % 1000 / 10);
        }
    }
}
