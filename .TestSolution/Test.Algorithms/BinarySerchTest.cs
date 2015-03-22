using NUnit.Framework;
using TestSolution;

namespace Test.Algorithms
{
    public class BinarySerchTest
    {
        [Test]
        public void LessOrEqualRequired()
        {
            int key = 7, x = 0;
            var array = new[] {1, 2, 3, 4, 5, 6, 7, 8};
            var c = 'c';
            var s = "String";
            var ars  = new[] {"a", "String2", "Long String"};

            var index = BinarySearch(key, array);
            index = BinarySearch(9, array);

            //Assert.AreEqual(3, index);
        }

        public int BinarySearch(int key, int[] array)
        {
            int low = 0, x = 0;
            var high = array.Length - 1;

            while (low <= high)
            {
                var mid = (high + low) / 2;
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
