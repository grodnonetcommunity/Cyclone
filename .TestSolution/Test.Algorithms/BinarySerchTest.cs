using NUnit.Framework;
using TestSolution;

namespace Test.Algorithms
{
    public class BinarySerchTest
    {
        [Test]
        public void LessOrEqualRequired()
        {
            int key = 4, x = 0;
            var array = new[] {1, 2, 3, 4, 5, 6};

            var index = BinarySearch(key, array);

            //Assert.AreEqual(3, index);
        }

        public int BinarySearch(int key, int[] array)
        {
            int low = 0, x = 1;
            var high = array.Length - 1;

			int i = 0;
			while (i < 4)
			{
				int j = 0;
				while (j < 3)
				{
					x = i * 3 + j;
					j = j + 1;
				}
				i = i + 1;
			}

            return -1;
        }
    }
}
