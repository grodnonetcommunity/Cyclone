using NUnit.Framework;
using TestSolution;

namespace Test.Algorithms
{
    public class BinarySerchTest
    {
        [Test]
        public void LessOrEqualRequired()
        {
            var key = 4;
            var array = new[] {1, 2, 3, 4, 5, 6};

            var index = BinarySearch.Search(key, array);

            //Assert.AreEqual(3, index);
        }
    }
}
