namespace adaptive_deep_learning_model.Utilities
{
    public static class ArrayUtilities
    {
        public static float[] ToFloatArray(this string input)
        {
            string[] s1 = input.Trim('[', ']').Split(',');
            float[] myArr = Array.ConvertAll(s1, n => float.Parse(n));

            return myArr;
        }
    }
}
