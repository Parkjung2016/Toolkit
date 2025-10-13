namespace PJH.Utility.Utils
{
    public static class RichTagHelper
    {
        public static string RichColor(string text, string color) => $"<color={color}>{text}</color>";
        public static string RichSize(string text, int size) => $"<size={size}>{text}</size>";
        public static string RichBold(string text) => $"<b>{text}</b>";
        public static string RichItalic(string text) => $"<i>{text}</i>";
        public static string RichUnderline(string text) => $"<u>{text}</u>";
        public static string RichStrikethrough(string text) => $"<s>{text}</s>";
        public static string RichFont(string text, string font) => $"<font={font}>{text}</font>";
        public static string RichAlign(string text, string align) => $"<align={align}>{text}</align>";

        public static string RichGradient(string text, string color1, string color2) =>
            $"<gradient={color1},{color2}>{text}</gradient>";

        public static string RichRotation(string text, float angle) => $"<rotate={angle}>{text}</rotate>";
        public static string RichSpace(string text, float space) => $"<space={space}>{text}</space>";
    }
}