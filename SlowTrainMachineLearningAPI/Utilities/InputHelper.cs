namespace API.SlowTrainMachineLearning.Utilities
{
    public static class InputHelper
    {
        public static int[] ToIntArray(this string input)
        {
            string[] s1 = input.Trim('[', ']').Split(',');
            int[] myArr = Array.ConvertAll(s1, n => int.Parse(n));

            return myArr;
        }
    }
}
