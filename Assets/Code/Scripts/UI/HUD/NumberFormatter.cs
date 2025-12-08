using System;

namespace Code.Scripts.Utils
{
    public static class NumberFormatter
    {
        private static readonly string[] suffixes = { "", "K", "M" };

        public static string FormatNumber(long number)
        {
            if (number == 0) return "0";

            bool isNegative = number < 0;
            long absNumber = Math.Abs(number);

            // Para números menores a 10000, mostrar normal
            if (absNumber < 10000)
            {
                return isNegative ? "-" + absNumber.ToString() : absNumber.ToString();
            }

            int suffixIndex = 0;
            double formattedNumber = absNumber;

            // Determinar el sufijo (K o M)
            while (formattedNumber >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                formattedNumber /= 1000;
                suffixIndex++;
            }

            string result;

            // Lógica específica para cada rango
            if (suffixIndex == 1) // Miles (K)
            {
                if (absNumber < 100000) // 10,000 a 99,999
                {
                    if (absNumber % 1000 == 0) // Múltiplos exactos de 1000
                    {
                        result = formattedNumber.ToString("0");
                    }
                    else
                    {
                        // Redondear a 1 decimal y quitar .0 si es necesario
                        double rounded = Math.Round(formattedNumber, 1);
                        result = rounded.ToString("0.#");
                    }
                }
                else // 100,000 a 999,999
                {
                    // Redondear al entero más cercano
                    result = Math.Round(formattedNumber).ToString("0");

                    // Si redondea a 1000, convertir a 1M
                    if (formattedNumber >= 999.5)
                    {
                        formattedNumber /= 1000;
                        suffixIndex++;
                        result = formattedNumber.ToString("0.00");
                    }
                }
            }
            else if (suffixIndex == 2) // Millones (M)
            {
                if (formattedNumber < 10)
                {
                    result = formattedNumber.ToString("0.00");
                }
                else if (formattedNumber < 100)
                {
                    result = formattedNumber.ToString("0.0");
                }
                else // 100M o más
                {
                    result = Math.Round(formattedNumber).ToString("0");
                }
            }
            else
            {
                // Para otros casos (no debería ocurrir con los sufijos actuales)
                result = formattedNumber.ToString("0");
            }

            // Quitar .0 o .00 innecesarios
            result = CleanDecimalZeros(result);

            // Limitar a máximo 4 caracteres numéricos (dígitos + punto)
            result = LimitToFourNumericCharacters(result);

            result += suffixes[suffixIndex];
            return isNegative ? "-" + result : result;
        }

        private static string CleanDecimalZeros(string numberString)
        {
            if (numberString.Contains("."))
            {
                numberString = numberString.TrimEnd('0');
                if (numberString.EndsWith("."))
                {
                    numberString = numberString.TrimEnd('.');
                }
            }
            return numberString;
        }

        private static string LimitToFourNumericCharacters(string numberString)
        {
            int numericCharCount = 0;
            int lastValidIndex = 0;

            for (int i = 0; i < numberString.Length; i++)
            {
                char c = numberString[i];
                if (char.IsDigit(c) || c == '.')
                {
                    numericCharCount++;
                    if (numericCharCount <= 4)
                    {
                        lastValidIndex = i;
                    }
                }
            }

            if (numericCharCount > 4)
            {
                // Redondear el último dígito visible
                string trimmed = numberString.Substring(0, lastValidIndex + 1);

                // Si cortamos en medio de un decimal, ajustar
                if (trimmed.EndsWith("."))
                {
                    trimmed = trimmed.Substring(0, trimmed.Length - 1);
                }

                return trimmed;
            }

            return numberString;
        }

        // Sobrecargas para otros tipos numéricos
        public static string FormatNumber(int number)
        {
            return FormatNumber((long)number);
        }

        public static string FormatNumber(float number)
        {
            return FormatNumber((long)Math.Round(number));
        }

        public static string FormatNumber(double number)
        {
            return FormatNumber((long)Math.Round(number));
        }
    }
}