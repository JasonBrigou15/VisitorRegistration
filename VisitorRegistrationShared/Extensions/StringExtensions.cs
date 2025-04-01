using System.Globalization;
using System.Text;

namespace VisitorRegistrationShared.Extensions
{
    public static class StringExtensions
    {
        public static string NormalizeForComparison(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove accents (e.g., é → e)
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            // Convert to lowercase and trim whitespace
            return sb.ToString().ToLowerInvariant().Trim();
        }

        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLowerInvariant());
        }
    }
}
