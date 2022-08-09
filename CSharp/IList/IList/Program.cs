using System.Collections;

namespace practice
{

    class Entry
    {
        static void Main(string[] args)
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.print();
        }
    }

    class Node<T>
    {
        public T _Value;
        public Node<T>? _Next;
        public Node(T value)
        {
            _Value = value;
            _Next = null;
        }
        public Node(T value, Node<T> next)
        {
            _Value = value;
            _Next = next;
        }
    }

    public class List<T>
    {
        private Node<T>? head = null;
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
            if (head == null)
            {
                head = new Node<T>(item);
                _count++;
            }
            else
            {
                Node<T> node = head;
                while (node._Next != null)
                {
                    node = node._Next;
                    _count++;
                }
                node._Next = new Node<T>(item);
            }
        }
        /**
         * @brief 插入
         * @param index 位置
         * @param item 基础类型
         */
        public void Insert(int index, T item)
        {
            if (index > _count)
            {
                throw new Exception("Error! insert out of range");
            }
            Node<T> node = head;
            Node<T> pre = null;
            Node<T> next;
            if (index == 0)
            {
                next = head;
                head = new Node<T>(item);
                head._Next = next;
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    pre = node;
                    node = node._Next;
                }
                next = node;
                node = new Node<T>(item);
                node._Next = next;
                pre._Next = node;
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
            Node<T> node = head;
            for (int i = 0; i < _count; i++)
            {
                if (node._Value.Equals(item))
                {
                    result = i;
                    break;
                }
                node = node._Next;
            }
            return result;
        }

        /**
         * @brief 清空
         */
        public void Clear()
        {
            head = null;
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
            Node<T> node = head;
            if (index == 0)
            {
                head = head._Next;
            }
            else
            {
                Node<T> pre = null;
                for (int i = 0; i <= index; i++)
                {
                    pre = node;
                    node = node._Next;
                }
                pre._Next = node._Next;
            }
            _count--;
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
                    Node<T> node = head;
                    for (int i = 0; i < index; i++)
                    {
                        node = node._Next;
                    }
                    return node._Value;
                }
                throw new Exception("Error! get out of range");
            }
            set
            {
                Insert(index, value);
            }
        }
        /**
         * @brief 迭代器
         */
        public IEnumerator<T> GetEnumerator()
        {
            Node<T> node = head;
            Node<T> result;
            while (node != null)
            {
                result = node;
                node = node._Next;
                yield return result._Value;
            }
        }
        /**
         * @brief 打印全部元素
         */
        public void print()
        {
            Node<T> node = head;
            while (node != null)
            {
                Console.WriteLine(node._Value);
                node = node._Next;
            }
        }
    }
}
