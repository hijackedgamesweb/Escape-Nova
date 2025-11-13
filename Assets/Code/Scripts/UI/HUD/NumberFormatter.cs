using System;

namespace Code.Scripts.Utils
{
    public static class NumberFormatter
    {
        private static readonly string[] suffixes = { "", "K", "M", "B", "T" };

        public static string FormatNumber(int number)
        {
            if (number == 0) return "0";

            bool isNegative = number < 0;
            long absNumber = Math.Abs((long)number);

            // Para números menores a 10000, mostrar normal
            if (absNumber < 10000)
            {
                return isNegative ? "-" + absNumber.ToString() : absNumber.ToString();
            }

            int suffixIndex = 0;
            double formattedNumber = absNumber;

            // Convertir a la unidad apropiada
            while (formattedNumber >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                formattedNumber /= 1000;
                suffixIndex++;
            }

            // Determinar el formato basado en el número de dígitos
            string result;
            if (formattedNumber < 10)
            {
                // Ej: 9.55K (4 caracteres numéricos)
                result = formattedNumber.ToString("0.00");
            }
            else if (formattedNumber < 100)
            {
                // Ej: 10.5K (4 caracteres numéricos)
                result = formattedNumber.ToString("0.0");
            }
            else if (formattedNumber < 1000)
            {
                // Ej: 999K (3 caracteres numéricos) o 100K (3 caracteres)
                result = Math.Round(formattedNumber).ToString("0");
            }
            else
            {
                // Si llegamos aquí, necesitamos el siguiente sufijo
                formattedNumber /= 1000;
                suffixIndex++;
                result = formattedNumber.ToString("0.00");
            }

            // Asegurar que no tengamos más de 4 caracteres numéricos
            result = LimitToFourNumericCharacters(result);

            result += suffixes[suffixIndex];
            return isNegative ? "-" + result : result;
        }

        private static string LimitToFourNumericCharacters(string numberString)
        {
            // Contar solo dígitos y punto decimal
            int numericCharCount = 0;
            foreach (char c in numberString)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    numericCharCount++;
                    if (numericCharCount > 4)
                    {
                        // Encontrar dónde cortar
                        for (int i = numberString.Length - 1; i >= 0; i--)
                        {
                            if (char.IsDigit(numberString[i]))
                            {
                                // Redondear el último dígito
                                string trimmed = numberString.Substring(0, i);
                                if (i < numberString.Length - 1 && numberString[i + 1] == '.')
                                {
                                    // Si cortamos después de un punto, eliminamos el punto también
                                    trimmed = numberString.Substring(0, i);
                                }
                                return trimmed;
                            }
                        }
                    }
                }
            }

            return numberString;
        }

        public static string FormatNumber(float number)
        {
            return FormatNumber((int)number);
        }
    }
}