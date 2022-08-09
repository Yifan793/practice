using System.Collections;

namespace list{
    class Node<T>
    {
        public T _Value;
        public Node<T> _Next;
        public Node() : this(default(T)) { }
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

    public class IList<T>
    {
        private Node<T> head = null;
        private int _count = 0;
        public int Count { get
            {
                return _count;
            }
        }
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
                while(node._Next != null)
                {
                    node = node._Next;
                    _count++;
                }
                node._Next = new Node<T>(item);
            }
        }

        public void Clear()
        {
            head = null;
            _count = 0;
        } 

        public bool Contains(T item)
        {
            return IndexOf(item) != -1 ? true : false;
        }

        public bool Remove(T item)
        {
            int idx = IndexOf(item);
            if(idx == -1)
            {
                return false;
            }
            RemoveAt(idx);
            return true;
        }

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

        public void Insert(int index, T item)
        {
            if ((index == 0 && _count == 0) || (index >= 0 && index < _count))
            {
                Node<T> node = head;
                Node<T> pre = null;
                Node<T> next = null;
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
                return;
            }
            throw new Exception("Error! insert out of range");
        }

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

        public void CopyTo(T[] array, int arrayIndex)
        {
            Node<T> node = head;
            for (int i = 0; i < _count; i++)
            {
                array[arrayIndex + i] = node._Value;
                node = node._Next;
            }
        }

        public IEnumerator<T> GetEnumberator()
        {
            Node<T> node = head;
            Node<T> result;
            while(node != null)
            {
                result = node;
                node = node._Next;
                yield return result._Value;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
