namespace Game.Utils
{
    public static class StringUtils
    {
        public static string ToUpperFirst(this string s)
        {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }
 
            return char.ToUpperInvariant(s[0]) + s.Substring(1);
        }
        
        public static string ToLowerFirst(this string s)
        {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }
 
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }

        public static string Substitute(this string s, string substitution, string newText)
        {
            return string.IsNullOrEmpty(s) ? s : s.Replace(substitution, newText);
        }
    }
}