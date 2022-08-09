using practice;
using Shouldly;

namespace IListTests
{
    [TestClass]
    public class IListTests
    {
        private practice.List<int> list;
        [TestInitialize]
        public void Init()
        {
            list = new practice.List<int>();
        }
        [TestCleanup]
        public void Cleanup()
        {
            list.Clear();
        }

        [TestMethod]
        public void Add_Valid()
        {
            list.Clear();
            list.Add(1);
            list.Add(-1);
            list.Add('a');
            list.Add(999999);

            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(-1, list[1]);
            Assert.AreEqual('a', list[2]);
            list[3].ShouldBe(999999);
            Assert.AreEqual(4, list.Count);
        }

        [TestMethod]
        public void Remove_Valid()
        {
            list.Add(1);
            list.Add(2);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            list.Remove(2);
            Assert.IsTrue(list.Contains(2));
            Assert.AreEqual(4, list.Count);
            
            list.Remove(1);
            Assert.IsFalse(list.Contains(1));
            Assert.AreEqual(3, list.Count);
            
            list.Remove(2);
            Assert.IsFalse(list.Contains(2));
            Assert.AreEqual(2, list.Count);

            Assert.IsFalse(list.Remove(2));
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void Insert_WithValidIndex()
        {
            list.Insert(0, 1);
            list.Insert(1, 2);
            list.Insert(1, 3);
            list[0].ShouldBe(1);
            list[1].ShouldBe(3);
            list[2].ShouldBe(2);
            Assert.AreEqual(3, list.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Insert_WithInValidIndex()
        {
            list.Insert(4, 5);
            Assert.AreEqual(0, list.Count);
            list.Insert(-1, 5);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Indexer_Valid()
        {
            list.Add(-1);
            list.Add(-1);
            list.Add(-1);
            list.Add(-1);
            list[0].ShouldBe(-1);
            list[1].ShouldBe(-1);
            list[2].ShouldBe(-1);
            list[3].ShouldBe(-1);

            list[0] = 0;
            list[1] = 1;
            list[2] = 2;
            list[3] = 3;
            list[0].ShouldBe(0);
            list[1].ShouldBe(1);
            list[2].ShouldBe(2);
            list[3].ShouldBe(3);

            Assert.AreEqual(4, list.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Indexer_InValid()
        {
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            list[7] = 0;
            list[-1] = 0;
            int tmp = list[-1];
            int tmp2 = list[7];
            Assert.IsNull(tmp);
            Assert.IsNull(tmp2);
        }

        [TestMethod]
        public void IEnumerator_Valid()
        {
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            int i = 1;
            foreach(int num in list) {
                Assert.AreEqual(num, i);
                i++;
            }
            Assert.AreEqual(4, list.Count);
        }
    }
}