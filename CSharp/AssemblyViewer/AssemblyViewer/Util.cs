using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace AssemblyViewer
{
    public static class Util
    {
        //========================================================================//
        public static void SortAll(ObservableCollection<ViewModelObject> vmList)
        {
            if (vmList == null)
            {
                return;
            }
            SortCollection(vmList);
            foreach (ViewModelObject vm in vmList)
            {
                SortAll(vm.ChildList);
                if (vm is ViewModelBaseClass BaseClass)
                {
                    SortCollection(BaseClass.MethodList);
                    SortCollection(BaseClass.EventList);
                    SortCollection(BaseClass.PropertyList);
                    SortCollection(BaseClass.FieldList);
                }
            }
        }

        private static void SortCollection<T>(ObservableCollection<T> collection)
        {
            var sortedList = collection.ToList();
            sortedList.Sort();
            collection.Clear();
            foreach (var sortedItem in sortedList)
            {
                collection.Add(sortedItem);
            }
        }


    }
}
