using System;
using System.Collections.Generic;
using System.Text;

namespace DipolNokia3310.Models
{
    /// <summary>
    /// Класс для конвертации последовательности нажатий клавиш в текст
    /// </summary>
    public static class KeypadConverter
    {
        /// <summary>
        /// Маппинг клавиш на символы
        /// </summary>
        private static readonly Dictionary<char, string[]> KeyMappings = new Dictionary<char, string[]>
        {
            {'1', new[] {"1"}},
            {'2', new[] {"A", "B", "C", "2"}},
            {'3', new[] {"D", "E", "F", "3"}},
            {'4', new[] {"G", "H", "I", "4"}},
            {'5', new[] {"J", "K", "L", "5"}},
            {'6', new[] {"M", "N", "O", "6"}},
            {'7', new[] {"P", "Q", "R", "S", "7"}},
            {'8', new[] {"T", "U", "V", "8"}},
            {'9', new[] {"W", "X", "Y", "Z", "9"}},
            {'0', new[] {" ", "0"}},
            {'*', new[] {"*"}}
        };

        /// <summary>
        /// Преобразует последовательность нажатий клавиш в текст
        /// </summary>
        /// <param name="sequence">Последовательность нажатий клавиш (например, "44 45*#")</param>
        /// <returns>Преобразованный текст</returns>
        public static string Convert(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
                return string.Empty;

            // Удаляем символ "#" в конце, если он есть
            if (sequence.EndsWith("#"))
                sequence = sequence.Substring(0, sequence.Length - 1);

            StringBuilder result = new StringBuilder();
            string[] groups = sequence.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string group in groups)
            {
                if (group.Length == 0)
                    continue;

                char key = group[0];
                if (!KeyMappings.ContainsKey(key))
                    continue;

                string[] possibleChars = KeyMappings[key];
                int pressCount = group.Length;
                int charIndex = (pressCount - 1) % possibleChars.Length;

                result.Append(possibleChars[charIndex]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Преобразует последовательность без пробелов в текст путем группировки 
        /// последовательных нажатий одной и той же клавиши
        /// </summary>
        /// <param name="sequence">Последовательность нажатий клавиш без пробелов</param>
        /// <returns>Преобразованный текст</returns>
        public static string ConvertSequenceWithoutSpaces(string sequence)
        {
            if (string.IsNullOrEmpty(sequence))
                return string.Empty;

            // Удаляем символ "#" в конце, если он есть
            if (sequence.EndsWith("#"))
                sequence = sequence.Substring(0, sequence.Length - 1);

            StringBuilder result = new StringBuilder();
            StringBuilder currentGroup = new StringBuilder();

            // Добавляем первый символ в группу
            if (sequence.Length > 0)
                currentGroup.Append(sequence[0]);

            // Проходим по оставшимся символам
            for (int i = 1; i < sequence.Length; i++)
            {
                // Если текущий символ такой же, как предыдущий, добавляем его к текущей группе
                if (sequence[i] == sequence[i - 1])
                {
                    currentGroup.Append(sequence[i]);
                }
                else // Иначе обрабатываем группу и начинаем новую
                {
                    ProcessGroup(currentGroup.ToString(), result);
                    currentGroup.Clear();
                    currentGroup.Append(sequence[i]);
                }
            }

            // Обрабатываем последнюю группу
            ProcessGroup(currentGroup.ToString(), result);

            return result.ToString();
        }

        /// <summary>
        /// Обрабатывает группу повторяющихся символов
        /// </summary>
        private static void ProcessGroup(string group, StringBuilder result)
        {
            if (string.IsNullOrEmpty(group))
                return;

            char key = group[0];
            if (!KeyMappings.ContainsKey(key))
                return;

            string[] possibleChars = KeyMappings[key];
            int pressCount = group.Length;
            int charIndex = (pressCount - 1) % possibleChars.Length;

            // Добавляем специальную обработку для клавиши "0"
            if (key == '0' && charIndex == 0)
            {
                // Добавляем настоящий пробел вместо символа из массива
                result.Append(" ");
            }
            else
            {
                result.Append(possibleChars[charIndex]);
            }
        }
    }
}