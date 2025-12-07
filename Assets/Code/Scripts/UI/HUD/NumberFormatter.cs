using System;

namespace Code.Scripts.Utils
{
    public static class NumberFormatter
    {
        // Sufijos extendidos hasta quintillones (puedes agregar más si necesitas)
        private static readonly string[] suffixes = { "", "K", "M", "B", "T", "Q", "Qi", "Sx", "Sp", "O", "N", "D", "U", "Dd", "Td" };

        public static string FormatNumber(long number)
        {
            if (number == 0) return "0";

            bool isNegative = number < 0;
            long absNumber = Math.Abs(number);

            // Para números menores a 10,000, mostrar normal
            if (absNumber < 10000)
            {
                return isNegative ? "-" + absNumber.ToString() : absNumber.ToString();
            }

            // Calcular el índice del sufijo basado en log10
            int suffixIndex = (int)Math.Log10(absNumber) / 3;
            double formattedNumber = absNumber / Math.Pow(1000, suffixIndex);

            // Ajustar si el número está cerca de 1000 para evitar "1000K"
            if (formattedNumber >= 999.5 && suffixIndex < suffixes.Length - 1)
            {
                formattedNumber /= 1000;
                suffixIndex++;
            }

            // Formatear con precisión adecuada
            string result;
            if (formattedNumber < 10)
            {
                result = formattedNumber.ToString("0.00");
            }
            else if (formattedNumber < 100)
            {
                result = formattedNumber.ToString("0.0");
            }
            else if (formattedNumber < 1000)
            {
                result = Math.Round(formattedNumber).ToString("0");
            }
            else
            {
                // Caso extremo: si todavía es 1000+, usar el siguiente sufijo
                formattedNumber /= 1000;
                suffixIndex++;
                result = formattedNumber.ToString("0.00");
            }

            // Limitar a 4 caracteres numéricos
            result = LimitToFourNumericCharacters(result);

            // Agregar sufijo o notación científica si se acaban los sufijos
            if (suffixIndex < suffixes.Length)
            {
                result += suffixes[suffixIndex];
            }
            else
            {
                // Notación científica para números muy grandes
                result = formattedNumber.ToString("0.00e+00");
            }

            return isNegative ? "-" + result : result;
        }

        private static string LimitToFourNumericCharacters(string numberString)
        {
            int numericCharCount = 0;
            for (int i = 0; i < numberString.Length; i++)
            {
                char c = numberString[i];
                if (char.IsDigit(c) || c == '.')
                {
                    numericCharCount++;
                    if (numericCharCount > 4)
                    {
                        // Encontrar dónde cortar y redondear
                        if (i + 1 < numberString.Length && char.IsDigit(numberString[i + 1]))
                        {
                            // Verificar si necesitamos redondear
                            int nextDigit = numberString[i + 1] - '0';
                            if (nextDigit >= 5)
                            {
                                // Redondear hacia arriba
                                char[] chars = numberString.ToCharArray();
                                bool rounded = false;
                                for (int j = i; j >= 0; j--)
                                {
                                    if (char.IsDigit(chars[j]))
                                    {
                                        int digit = chars[j] - '0' + 1;
                                        if (digit < 10)
                                        {
                                            chars[j] = digit.ToString()[0];
                                            rounded = true;
                                            break;
                                        }
                                        else
                                        {
                                            chars[j] = '0';
                                        }
                                    }
                                }
                                numberString = new string(chars);
                            }
                        }
                        // Cortar a 4 caracteres numéricos
                        return numberString.Substring(0, i);
                    }
                }
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