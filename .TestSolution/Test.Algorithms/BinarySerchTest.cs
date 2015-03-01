using NUnit.Framework;
using TestSolution;

namespace Test.Algorithms
{
    public class BinarySerchTest
    {
        [Test]
        public void LessOrEqualRequired()
        {
            var key = 30;
            var array = new[] {1, 2, 3, 4, 5, 6};

            var index = BinarySearch(key, array);

            //Assert.AreEqual(3, index);
        }

        public int BinarySearch(int key, int[] array)
        {
            var low = 0;
            var high = array.Length - 1;

            while (true)
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
