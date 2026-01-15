namespace VDCD.Helper
{
    public static class SlugHelper
    {
        public static string Generate(string text)
        {
            text = text.ToLowerInvariant();

            text = System.Text.RegularExpressions.Regex.Replace(
                text.Normalize(System.Text.NormalizationForm.FormD),
                @"\p{IsCombiningDiacriticalMarks}+",
                ""
            );

            text = text.Replace("đ", "d");

            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-").Trim('-');

            return text;
        }
    }

}
