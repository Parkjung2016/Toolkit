using System;
using System.Collections.Generic;
using System.Linq;

namespace PJH.Utility.Extensions
{
    public static class StringExtensions
    {
        /// <summary>문자열이 null이거나 공백(화이트스페이스)인지 확인합니다.</summary>
        public static bool IsNullOrWhiteSpace(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>문자열이 null이거나 빈 문자열인지 확인합니다.</summary>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>문자열이 null, 빈 문자열, 또는 공백인 경우를 확인합니다.</summary>
        public static bool IsBlank(this string val) => val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();

        /// <summary>문자열이 null일 경우 빈 문자열로 반환합니다.</summary>
        public static string OrEmpty(this string val) => val ?? string.Empty;

        /// <summary>
        /// 문자열을 지정한 최대 길이로 자릅니다. 
        /// 문자열 길이가 maxLength보다 짧으면 원본 문자열을 반환합니다.
        /// </summary>
        public static string Shorten(this string val, int maxLength)
        {
            if (val.IsBlank()) return val;
            return val.Length <= maxLength ? val : val.Substring(0, maxLength);
        }

        /// <summary>문자열을 startIndex부터 endIndex까지 자릅니다.</summary>
        /// <returns>잘라낸 문자열</returns>
        public static string Slice(this string val, int startIndex, int endIndex)
        {
            if (val.IsBlank())
            {
                throw new ArgumentNullException(nameof(val), "Value cannot be null or empty.");
            }

            if (startIndex < 0 || startIndex > val.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // If the end index is negative, it will be counted from the end of the string.
            endIndex = endIndex < 0 ? val.Length + endIndex : endIndex;

            if (endIndex < 0 || endIndex < startIndex || endIndex > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }

            return val.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// 입력 문자열을 영문자, 숫자, 밑줄(_) 그리고 옵션에 따라 마침표(.)만 포함하는 문자열로 변환합니다.
        /// </summary>
        /// <param name="input">변환할 입력 문자열</param>
        /// <param name="allowPeriods">마침표(.)를 허용할지 여부</param>
        /// <returns>
        /// 영문자, 숫자, 밑줄, (옵션에 따라 마침표만 포함하는 새 문자열.
        /// 입력이 null이거나 빈 문자열이면 빈 문자열 반환.
        /// </returns>
        public static string ConvertToAlphanumeric(this string input, bool allowPeriods = false)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            List<char> filteredChars = new List<char>();
            int lastValidIndex = -1;

            // Iterate over the input string, filtering and determining valid start/end indices
            foreach (char character in input
                         .Where(character => char
                             .IsLetterOrDigit(character) || character == '_' || (allowPeriods && character == '.'))
                         .Where(character =>
                             filteredChars.Count != 0 || (!char.IsDigit(character) && character != '.')))
            {
                filteredChars.Add(character);
                lastValidIndex = filteredChars.Count - 1; // Update lastValidIndex for valid characters
            }

            // Remove trailing periods
            while (lastValidIndex >= 0 && filteredChars[lastValidIndex] == '.')
            {
                lastValidIndex--;
            }

            // Return the filtered string
            return lastValidIndex >= 0
                ? new string(filteredChars.ToArray(), 0, lastValidIndex + 1)
                : string.Empty;
        }

        /// <summary>문자열 내의 모든 공백(띄어쓰기)을 제거합니다.</summary>
        public static string RemoveAllSpaces(this string val)
        {
            if (val.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            return val.Replace(" ", "");
        }

        /// <summary>문자열 내의 모든 공백 문자(공백, 탭, 줄 바꿈 등)를 제거합니다.</summary>
        public static string RemoveAllWhitespace(this string val)
        {
            if (val.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            return new string(val.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        // Rich text formatting, for Unity UI elements that support rich text.
        public static string RichColor(this string text, string color) => $"<color={color}>{text}</color>";
        public static string RichSize(this string text, int size) => $"<size={size}>{text}</size>";
        public static string RichBold(this string text) => $"<b>{text}</b>";
        public static string RichItalic(this string text) => $"<i>{text}</i>";
        public static string RichUnderline(this string text) => $"<u>{text}</u>";
        public static string RichStrikethrough(this string text) => $"<s>{text}</s>";
        public static string RichFont(this string text, string font) => $"<font={font}>{text}</font>";
        public static string RichAlign(this string text, string align) => $"<align={align}>{text}</align>";

        public static string RichGradient(this string text, string color1, string color2) =>
            $"<gradient={color1},{color2}>{text}</gradient>";

        public static string RichRotation(this string text, float angle) => $"<rotate={angle}>{text}</rotate>";
        public static string RichSpace(this string text, float space) => $"<space={space}>{text}</space>";
    }
}