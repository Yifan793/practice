using System;

namespace Program
{
    class QuickSort
    {
        static void Main(string[] args)
        {
            string str = Console.ReadLine();
            string[] strArray = str.Split(' ');
            List<int> input = new List<int>(11);
            foreach (string s in strArray)
            {
                try
                {
                    int num = int.Parse(s);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! please enter valid number");
                    return;
                }
                input.Add(int.Parse(s));
            }
            if (input.Count != 10)
            {
                Console.WriteLine("Error! please enter 10 numbers");
                return;
            }
            Sort(input, 0, input.Count - 1);
            Console.Write("result is ");
            foreach (int num in input)
            {
                Console.Write(num + " ");
            }
            Console.WriteLine(" ");
        }

        static void Sort(List<int> list, int left, int right)
        {
            if (left > right) {
                return;
            }
            int tmp = list[left];
            int i = left;
            int j = right;
            while(i != j)
            {
                while (i < j && list[j] >= tmp)
                {
                    j--;
                }
                while (i < j && list[i] <= tmp)
                {
                    i++;
                }
                if (i < j)
                {
                    int t = list[i];
                    list[i] = list[j];
                    list[j] = t;
                }
            }
            list[left] = list[i];
            list[i] = tmp;
            Sort(list, left, i - 1);
            Sort(list, i + 1, right);
        }
    }
}
