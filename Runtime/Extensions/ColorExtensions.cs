using System;
using UnityEngine;

namespace PJH.Utility.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// 색상의 알파(alpha) 값을 설정합니다.
        /// </summary>
        /// <param name="color">원본 색상입니다.</param>
        /// <param name="alpha">새로 설정할 알파 값입니다.</param>
        /// <returns>지정된 알파 값을 가진 새 색상을 반환합니다.</returns>
        public static Color SetAlpha(this Color color, float alpha)
            => new Color(color.r, color.g, color.b, alpha);

        /// <summary>
        /// 두 색상을 더하고, 그 결과를 0과 1 사이로 변환합니다.
        /// </summary>
        /// <param name="thisColor">첫 번째 색상입니다.</param>
        /// <param name="otherColor">두 번째 색상입니다.</param>
        /// <returns>두 색상을 더한 결과를 0과 1 사이로 변환한 새 색상을 반환합니다.</returns>
        public static Color Add(this Color thisColor, Color otherColor)
            => (thisColor + otherColor).Clamp01();

        /// <summary>
        /// 한 색상에서 다른 색상을 빼고, 그 결과를 0과 1 사이로 변환합니다.
        /// </summary>
        /// <param name="thisColor">기준이 되는 색상입니다.</param>
        /// <param name="otherColor">뺄 색상입니다.</param>
        /// <returns>두 색상의 차이를 0과 1 사이로 변환한 새 색상을 반환합니다.</returns>
        public static Color Subtract(this Color thisColor, Color otherColor)
            => (thisColor - otherColor).Clamp01();

        /// <summary>
        /// 색상의 RGBA 성분을 각각 0과 1 사이로 변환합니다.
        /// </summary>
        /// <param name="color">원본 색상입니다.</param>
        /// <returns>모든 성분이 0과 1 사이로 변환된 새 색상을 반환합니다.</returns>
        static Color Clamp01(this Color color)
        {
            return new Color
            {
                r = Mathf.Clamp01(color.r),
                g = Mathf.Clamp01(color.g),
                b = Mathf.Clamp01(color.b),
                a = Mathf.Clamp01(color.a)
            };
        }

        /// <summary>
        /// 색상을 16진수 문자열로 변환합니다.
        /// </summary>
        /// <param name="color">변환할 색상입니다.</param>
        /// <returns>해당 색상의 16진수 문자열을 반환합니다.</returns>
        public static string ToHex(this Color color)
            => $"#{ColorUtility.ToHtmlStringRGBA(color)}";

        /// <summary>
        /// 16진수 문자열을 색상으로 변환합니다.
        /// </summary>
        /// <param name="hex">변환할 16진수 문자열입니다.</param>
        /// <returns>해당 16진수 문자열이 나타내는 색상을 반환합니다.</returns>
        public static Color FromHex(this string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }

            throw new ArgumentException("Invalid hex string", nameof(hex));
        }

        /// <summary>
        /// 두 색상을 지정된 비율로 혼합합니다.
        /// </summary>
        /// <param name="color1">첫 번째 색상입니다.</param>
        /// <param name="color2">두 번째 색상입니다.</param>
        /// <param name="ratio">혼합 비율입니다 (0~1 사이).</param>
        /// <returns>혼합된 색상을 반환합니다.</returns>
        public static Color Blend(this Color color1, Color color2, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            return new Color(
                color1.r * (1 - ratio) + color2.r * ratio,
                color1.g * (1 - ratio) + color2.g * ratio,
                color1.b * (1 - ratio) + color2.b * ratio,
                color1.a * (1 - ratio) + color2.a * ratio
            );
        }

        /// <summary>
        /// 색상을 반전시킵니다.
        /// </summary>
        /// <param name="color">반전할 색상입니다.</param>
        /// <returns>반전된 색상을 반환합니다.</returns>
        public static Color Invert(this Color color)
            => new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
    }
}