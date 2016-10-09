namespace AssessmentManager
{
    public static class StringExtensionMethods
    {
        public static bool NullOrEmpty(this string str)
        {
            if (str == null || str == "")
                return true;
            return false;
        }
    }
}
