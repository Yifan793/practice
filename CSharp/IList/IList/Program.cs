using System.Collections;

namespace practice
{
    class Entry
    {
        static void Main(string[] args)
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Insert(1, 2);
            list.Add(3);
            list.Add(4);
            list.Print();
            Console.WriteLine("count is " + list.Count);
            Console.WriteLine("================");
            Console.WriteLine("list contains 4 is " + list.Contains(4));
            Console.WriteLine("list indexof(3) is " + list.IndexOf(3));
            Console.WriteLine("================");
            list.Remove(1);
            list.Print();
            Console.WriteLine("count is " + list.Count);
            Console.WriteLine("================");
            list.RemoveAt(2);
            list.Print();
            Console.WriteLine("count is " + list.Count);
            Console.WriteLine("================");
            Console.WriteLine("list[1] is " + list[1]);
            Console.WriteLine("================");
            foreach (int i in list)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine("================");
            list.Clear();
            list.Print();
            Console.WriteLine("================");
        }
    }

    public class Node<T>
    {
        public T Value { get; set; }
        public Node<T> Next { get; set; }
        public Node(T value)
        {
            Value = value;
            Next = null;
        }
        public Node(T value, Node<T> next)
        {
            Value = value;
            Next = next;
        }
    }

    public class List<T> : IList<T>
    {
        private Node<T> _head = null;
        private int _count = 0;

        public int Count
        {
            get
            {
                return _count;
            }
        }
        public bool IsFixedSize
        {
            get
            {
                return true;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /**
         * @brief 添加
         * @param item 基础类型
         */
        public void Add(T item)
        {
            Insert(_count, item);
        }
        /**
         * @brief 插入
         * @param index 位置
         * @param item 基础类型
         */
        public void Insert(int index, T item)
        {
            if (index < 0 || index > _count)
            {
                throw new Exception("Error! insert out of range");
            }
            Node<T> node = _head;
            Node<T> pre = null;
            Node<T> next;
            if (index == 0)
            {
                next = _head;
                _head = new Node<T>(item);
                _head.Next = next;
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    pre = node;
                    node = node.Next;
                }
                next = node;
                node = new Node<T>(item);
                node.Next = next;
                pre.Next = node;
            }
            _count++;
        }

        /**
         * @brief 是否包含
         * @param item 基础类型
         */
        public bool Contains(T item)
        {
            return IndexOf(item) != -1 ? true : false;
        }
        /**
         * @brief 下标位置
         * @param item 基础类型
         */
        public int IndexOf(T item)
        {
            int result = -1;
            Node<T> node = _head;
            for (int i = 0; i < _count; i++)
            {
                if (node.Value.Equals(item))
                {
                    result = i;
                    break;
                }
                node = node.Next;
            }
            return result;
        }

        /**
         * @brief 清空
         */
        public void Clear()
        {
            _head = null;
            _count = 0;
        }
        /**
         * @brief 删除
         * @param item 基础类型
         */
        public bool Remove(T item)
        {
            int idx = IndexOf(item);
            if (idx == -1)
            {
                return false;
            }
            RemoveAt(idx);
            return true;
        }
        /**
         * @brief 删除
         * @param index 位置
         */
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
            {
                throw new Exception("Error! remvoe out of range");
            }
            Node<T> node = _head;
            if (index == 0)
            {
                _head = _head.Next;
            }
            else
            {
                Node<T> pre = null;
                for (int i = 0; i < index; i++)
                {
                    pre = node;
                    node = node.Next;
                }
                pre.Next = node.Next;
            }
            _count--;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            Node<T> node = _head;
            for (int i = 0; i < _count; i++)
            {
                array[arrayIndex + i] = node.Value;
                node = node.Next;
            }
        }


        /**
         * @brief 索引器
         */
        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < _count)
                {
                    Node<T> node = _head;
                    for (int i = 0; i < index; i++)
                    {
                        node = node.Next;
                    }
                    return node.Value;
                }
                throw new Exception("Error! get out of range");
            }
            set
            {
                if (index >= 0 && index < _count)
                {
                    Node<T> node = _head;
                    for (int i = 0; i < index; i++)
                    {
                        node = node.Next;
                    }
                    node.Value = value;
                }
                throw new Exception("Error! set out of range");
            }
        }

        /**
         * @brief 迭代器
         */
        public IEnumerator<T> GetEnumerator()
        {
            //yield 方式
            //Node<T> node = _head;
            //Node<T> result;
            //while (node != null)
            //{
            //    result = node;
            //    node = node.Next;
            //    yield return result.Value;
            //}

            return new ListEnumerator<T>(_head);
        }
        /**
         * @brief 打印全部元素
         */
        public void Print()
        {
            Node<T> node = _head;
            while (node != null)
            {
                Console.WriteLine(node.Value);
                node = node.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class ListEnumerator<T> : IEnumerator<T>
    {
        private Node<T> _head;
        private Node<T> _curNode;

        public ListEnumerator(Node<T> head)
        {
            _head = head;
        }

        public bool MoveNext()
        {
            if (_curNode == null)
            {
                _curNode = _head;
                return true;
            }

            if (_curNode.Next == null)
            {
                return false;
            }
            else
            {
                _curNode = _curNode.Next;
            }
            return true;
        }

        public void Reset()
        {
            _curNode = null;
        }

        public void Dispose()
        {
        }

        public T Current
        {
            get { return _curNode.Value; }
        }
        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
