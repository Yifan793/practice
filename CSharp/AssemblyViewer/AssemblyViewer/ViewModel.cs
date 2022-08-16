using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AssemblyViewer
{
    public class ViewModleRoot
    {
        public List<ViewModelNameSpace> NameSpaceList { get; set; }
    }

    public class ViewModelObject : IComparable<ViewModelObject>
    {
        public ViewModelObject()
        {
            ChildList = new List<ViewModelObject>();
        }
        public string Name { get; set; }
        public AccessRight AccessRights { get; set; }
        public string Icon { get; set; }
        public ViewerType Type { get; set; }
        public List<ViewModelObject> ChildList { get; set; }
        public SolidColorBrush FontBrush { get; set; }
        public ModelObject Model
        {
            get
            {
                ModelObject model = new ModelObject();
                model.Name = Name;
                model.AccessRights = AccessRights;
                model.Type = Type;
                List<ModelObject> list = new List<ModelObject>();
                foreach (ViewModelObject obj in ChildList)
                {
                    ModelObject modelObject = obj.Model;
                    list.Add(modelObject);
                }
                model.ChildList = list;
                return model;
            }
            set
            {
                Name = value.Name;
                AccessRights = value.AccessRights;
                Type = value.Type;
                List<ViewModelObject> list = new List<ViewModelObject>();
                foreach (ModelObject obj in value.ChildList)
                {
                    ViewModelObject viewModelObject = new ViewModelObject();
                    viewModelObject.Model = obj;
                    list.Add(viewModelObject);
                }
                ChildList = list;
            }
        }

        public int CompareTo(ViewModelObject other)
        {
            return other.Name.CompareTo(Name);
        }
    }

    public class ViewModelNameSpace : ViewModelObject
    {
        public ViewModelNameSpace()
        {
            FontBrush = new SolidColorBrush(Colors.Gold);
        }
    }

    //==========================================================//

    public class ViewModelBaseClass : ViewModelObject
    {
        public List<ViewModelBaseClass> DerivedList { get; set; }
        public ViewModelBaseClass BaseClass { get; set; }
        public List<ViewModelMethod> MethodList { get; set; }
        public List<ViewModelProperty> PropertyList { get; set; }
        public List<ViewModelEvent> EventList { get; set; }
        public ModelBaseClass Model
        {
            get
            {             
                ModelBaseClass model = new ModelBaseClass();
                model.Name = Name;
                model.AccessRights = AccessRights;
                model.BaseClass = BaseClass.Model;
                model.Type = Type;
                List<ModelBaseClass> derivedList = new List<ModelBaseClass>();
                foreach (ViewModelBaseClass obj in DerivedList)
                {
                    ModelBaseClass baseClass = obj.Model;
                    derivedList.Add(baseClass);
                }
                model.DerivedList = derivedList;
                List<ModelMethod> methodList = new List<ModelMethod>();
                foreach (ViewModelMethod obj in MethodList)
                {
                    ModelMethod method = (ModelMethod)obj.Model;
                    methodList.Add(method);
                }
                model.MethodList = methodList;
                List<ModelProperty> propertyList = new List<ModelProperty>();
                foreach (ViewModelProperty obj in PropertyList)
                {
                    ModelProperty property = (ModelProperty)obj.Model;
                    propertyList.Add(property);
                }
                model.PropertyList = propertyList;
                List<ModelEvent> eventList = new List<ModelEvent>();
                foreach (ViewModelEvent obj in EventList)
                {
                    ModelEvent modelEvent = (ModelEvent)obj.Model;
                    eventList.Add(modelEvent);
                }
                model.EventList = eventList;
                return model;
            }
            set
            {
                Name = value.Name;
                AccessRights = value.AccessRights;
                BaseClass.Model = value.BaseClass;
                Type = value.Type;
                List<ViewModelBaseClass> derivedList = new();
                foreach (ModelBaseClass obj in value.DerivedList)
                {
                    ViewModelBaseClass baseClass = new()
                    {
                        Model = obj
                    };
                    derivedList.Add(baseClass);
                }
                DerivedList = derivedList;
                List<ViewModelMethod> methodList = new();
                foreach (ModelMethod obj in value.MethodList)
                {
                    ViewModelMethod method = new()
                    {
                        Model = obj
                    };
                    methodList.Add(method);
                }
                MethodList = methodList;
                List<ViewModelProperty> propertyList = new();
                foreach (ModelProperty obj in value.PropertyList)
                {
                    ViewModelProperty property = new()
                    {
                        Model = obj
                    };
                    propertyList.Add(property);
                }
                PropertyList = propertyList;
                List<ViewModelEvent> eventList = new();
                foreach (ModelEvent obj in value.EventList)
                {
                    ViewModelEvent viewModelEvent = new()
                    {
                        Model = obj
                    };
                    eventList.Add(viewModelEvent);
                }
                EventList = eventList;
            }
        }
    }


    public class ViewModelInterface : ViewModelBaseClass
    {
        public ViewModelInterface()
        {
            FontBrush = new SolidColorBrush(Colors.DarkGray);
        }
    }

    public class ViewModelClass : ViewModelBaseClass
    {
        public ViewModelClass()
        {
            FontBrush = new SolidColorBrush(Colors.MediumTurquoise);
        }
    }

    public class ViewModelEnum : ViewModelBaseClass
    {
        public ViewModelEnum()
        {
            FontBrush = new SolidColorBrush(Colors.LightGreen);
        }
    }

    public class ViewModelStruct: ViewModelBaseClass
    {
        public ViewModelStruct()
        {
            FontBrush = new SolidColorBrush(Colors.LimeGreen);
        }
    }

    //==========================================================//

    public class ViewModelMethod : ViewModelObject
    {
        public ViewModelMethod()
        {
            FontBrush = new SolidColorBrush(Colors.Orange);
        }
    }
    public class ViewModelEvent : ViewModelObject
    {
        public ViewModelEvent()
        {
            FontBrush = new SolidColorBrush(Colors.LightPink);
        }
    }

    public class ViewModelProperty : ViewModelObject
    {
        public ViewModelProperty()
        {
            FontBrush = new SolidColorBrush(Colors.DarkCyan);
        }
    }
    public class ViewModelField : ViewModelObject
    {
        public ViewModelField()
        {
            FontBrush = new SolidColorBrush(Colors.MediumPurple);
        }
    }


}
