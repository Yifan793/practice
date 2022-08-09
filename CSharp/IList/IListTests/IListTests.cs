using practice;

namespace IListTests
{
    [TestClass]
    public class IListTests
    {
        [TestMethod]
        public void Add_WithValidIndex()
        {
            practice.List<int> list = new practice.List<int>();
            list.Add(1);
            list.Add(-1);
            list.Add('a');
            list.Add(999999);
            list.print();
        }

        [TestMethod]
        public void Remove_WithValidNum()
        {
            practice.List<int> list = new practice.List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            Console.WriteLine(list.Remove(2));
            Console.WriteLine(list.Remove(1));
            Console.WriteLine(list.Remove(2));
            Console.WriteLine(list.Remove(2));
            list.print();
        }

        [TestMethod]
        public void Insert_WithValidIndex()
        {
            practice.List<int> list = new practice.List<int>();
            list.Insert(0, 1);
            list.Insert(1, 4);

            //list.Insert(4, 5);    //error
            //list.Insert(-1, 'c'); //error
            list.print();
        }

        [TestMethod]
        public void Indexer_Valid()
        {
            practice.List<int> list = new practice.List<int>();
            list.Add(1);
            list.Add(1);
            list.Add(1);
            list.Add(1);

            list[2] = 2;
            list[3] = 3;
            //list[7] = 0;   //error
            //list[-1] = 0;  //error
            list.print();
        }

        [TestMethod]
        public void IEnumerator_Valid()
        {
            practice.List<int> list = new practice.List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            foreach(int num in list) {
                Console.WriteLine(num);
            }
        }
    }
}