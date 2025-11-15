using UnityEngine;

namespace Code.Scripts.Utilities
{
    public static class NumberFormatter
    {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Q" };

        public static string FormatNumber(int value, int decimalPlaces = 1)
        {
            if (value < 0)
            {
                return "-" + FormatNumber(-value, decimalPlaces);
            }
            if (value < 1000)
            {
                return value.ToString();
            }

            int mag = (int)Mathf.Floor(Mathf.Log10(value) / 3);
            long divisor = 1L;
            for (int i = 0; i < mag; i++)
            {
                divisor *= 1000;
            }

            float num = (float)value / divisor;
            string format = "N" + decimalPlaces;
            
            return num.ToString(format) + Suffixes[mag];
        }
    }
}