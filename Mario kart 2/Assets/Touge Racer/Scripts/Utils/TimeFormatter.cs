using UnityEngine;

namespace Sain.Utils
{
    public static class TimeFormatterExtensionMethods
    {
        public static string FormatTime(this float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60); // Get minutes
            int seconds = Mathf.FloorToInt(timeInSeconds % 60); // Get remaining seconds
            int miliseconds = Mathf.FloorToInt(timeInSeconds * 1000 % 1000);
            return string.Format("{0:00}\'{1:00}\"{2:000}", minutes, seconds, miliseconds); // Format as MM'SS'mmm
        }
    }
}

