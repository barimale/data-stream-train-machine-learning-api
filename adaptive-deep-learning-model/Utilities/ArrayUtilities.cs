using System.Globalization;

namespace adaptive_deep_learning_model.Utilities
{
    public static class ArrayUtilities
    {
        public static double[] ToDoubleArray(this string input)
        {
            string[] s1 = input.Trim('[', ']').Split(',');
            double[] myArr = Array.ConvertAll(s1, n => double.Parse(n, CultureInfo.InvariantCulture));

            return myArr;
        }
    }
}
