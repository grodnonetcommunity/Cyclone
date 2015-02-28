namespace TestSolution
{
    public static class BinarySearch
    {
        public static int Search(int key, int[] array)
        {
            var low = 0;
            var high = array.Length - 1;

            while (low <= high)
            {
                var mid = low + (high - low)/2;
                var value = array[mid];

                if (key > value)
                {
                    low = mid + 1;
                }
                else if (key < value)
                {
                    high = mid - 1;
                }
                else
                {
                    return mid;
                }
            }

            return -1;
        }
    }
}
