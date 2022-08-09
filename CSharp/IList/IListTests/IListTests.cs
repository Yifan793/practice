using list;

namespace IListTests
{
    [TestClass]
    public class IListTests
    {
        [TestMethod]
        public void Add_WithValidIndex()
        {
            list.IList<int> list = new list.IList<int>();
            list.Add(1);
            list.Add(-1);
            list.Add('a');
            list.Add(999999);
        }

        [TestMethod]
        public void Remove_WithValidIndex()
        {
            list.IList<int> list = new list.IList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(2);
            list.Add(3);

            list.Remove(2);
            list.Remove(1);
            list.Remove(2);
            list.Remove(2);
        }
        [TestMethod]
        public void Indexer_WithValidIndex()
        {
            list.IList<int> list = new list.IList<int>();
            list.Insert(0, 1);
            list.Insert(1, 4);
            list.Insert(4, 5);
            list.Insert(-1, 'c');
        }

        [TestMethod]
        public void IEnumerator_WithValidIndex()
        {
            list.IList<int> list = new list.IList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(2);
            list.Add(3);

            foreach(int num in list) {
                Console.WriteLine(num);
            }
        }
    }
}