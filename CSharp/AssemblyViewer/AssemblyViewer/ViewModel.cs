using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
            FontBrush = new SolidColorBrush(Colors.Cornsilk);
            ChildList = new List<ViewModelObject>();
        }
        public string Name { get; set; }
        public string LeftBracket { get; set; }
        public string RightBracket { get; set; }
        public string Colon { get; set; }
        public string Argument { get; set; }
        public string ReturnValue { get; set; }
        public AccessRights AccessRights { get; set; }
        public string Icon
        {
            get
            {
                string access = "";
                if (AccessRights != AccessRights.InValid)
                {
                    access = AccessRights.ToString();
                }
                return "pack://application:,,,/Images/" + Type.ToString() + access + ".16.16.png";
            }
        }
        public ViewerType Type { get; set; }
        public SolidColorBrush FontBrush { get; set; }
        public List<ViewModelObject> ChildList { get; set; }
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
            return this.Name.CompareTo(other.Name);
        }
    }

    public class ViewModelNameSpace : ViewModelObject
    {
        public ViewModelNameSpace()
        {
            FontBrush = new SolidColorBrush(Colors.Gold);
            Type = ViewerType.Namespace;
            AccessRights = AccessRights.InValid;
        }
    }

    //==========================================================//

    public class ViewModelBaseClass : ViewModelObject
    {
        public ViewModelBaseClass()
        {
            BaseList = new List<ViewModelObject>();
            MethodList = new List<ViewModelMethod>();
            PropertyList = new List<ViewModelProperty>();
            EventList = new List<ViewModelEvent>();
            FieldList = new List<ViewModelField>();
        }
        public ViewModelObject BaseClass { get; set; }

        public List<ViewModelObject> BaseList { get; set; }
        public List<ViewModelMethod> MethodList { get; set; }
        public List<ViewModelEvent> EventList { get; set; }
        public List<ViewModelProperty> PropertyList { get; set; }
        public List<ViewModelField> FieldList { get; set; }

        public ModelBaseClass Model
        {
            get
            {             
                ModelBaseClass model = new ModelBaseClass();
                model.Name = Name;
                model.AccessRights = AccessRights;
                model.BaseClass = BaseClass.Model;
                model.Type = Type;
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
            Type = ViewerType.Interface;
        }
    }

    public class ViewModelClass : ViewModelBaseClass
    {
        public List<ViewModelConstructor> ConstructorList { get; set; }
        public ViewModelClass()
        {
            FontBrush = new SolidColorBrush(Colors.MediumTurquoise);
            Type = ViewerType.Class;
            ConstructorList = new List<ViewModelConstructor>();
        }
    }

    public class ViewModelEnum : ViewModelBaseClass
    {
        public ViewModelEnum()
        {
            FontBrush = new SolidColorBrush(Colors.LightGreen);
            Type = ViewerType.Enumeration;
        }
    }

    public class ViewModelEnumItem : ViewModelObject
    {
        public ViewModelEnumItem()
        {
            FontBrush = new SolidColorBrush(Colors.Plum);
            Type = ViewerType.EnumerationItem;
            Colon = " : ";
        }
    }

    public class ViewModelStruct: ViewModelBaseClass
    {
        public ViewModelStruct()
        {
            FontBrush = new SolidColorBrush(Colors.LimeGreen);
            Type = ViewerType.Structure;
        }
    }

    //==========================================================//

    public class ViewModelMethod : ViewModelObject
    {
        public ViewModelMethod()
        {
            FontBrush = new SolidColorBrush(Colors.Orange);
            Type = ViewerType.Method;
        }
        public ViewModelMethod(MethodInfo methodInfo)
        {
            FontBrush = new SolidColorBrush(Colors.Orange);
            Type = ViewerType.Method;
            ParameterInfo[] paramInfo = methodInfo.GetParameters();
            string paramString = "";
            foreach (ParameterInfo param in paramInfo)
            {  
                string genericString = Util.getParamString(param.ParameterType);
                string attrString = param.Attributes != ParameterAttributes.None ? param.Attributes.ToString() + " " : param.ParameterType.IsByRef ? "Ref " : "";
                paramString += attrString + Util.getParamString(param.ParameterType) +  ", ";
            }
            if (paramInfo.Length > 0)
            {
                paramString = paramString.Substring(0, paramString.Length - 2);
            }
            Name = methodInfo.Name;
            LeftBracket = "(";
            Argument = paramString;
            RightBracket = ")";
            Colon = " : ";
            ReturnValue = methodInfo.ReturnParameter.ToString().Replace("System.", "");
            AccessRights = Util.getAccessibility(methodInfo);
        }
    }
    public class ViewModelEvent : ViewModelObject
    {
        public ViewModelEvent()
        {
            FontBrush = new SolidColorBrush(Colors.LightPink);
            Type = ViewerType.Event;
        }
    }

    public class ViewModelProperty : ViewModelObject
    {
        public ViewModelProperty()
        {
            FontBrush = new SolidColorBrush(Colors.DarkCyan);
            Type = ViewerType.Property;
        }
    }
    public class ViewModelField : ViewModelObject
    {
        public ViewModelField()
        {
            FontBrush = new SolidColorBrush(Colors.MediumPurple);
            Type = ViewerType.Field;
        }
    }
    public class ViewModelConstructor : ViewModelObject
    {
        public ViewModelConstructor()
        {
            FontBrush = new SolidColorBrush(Colors.MediumTurquoise);
            Type = ViewerType.Method;
        }
        public ViewModelConstructor(ConstructorInfo constructorInfo)
        {
            FontBrush = new SolidColorBrush(Colors.MediumTurquoise);
            Type = ViewerType.Method;
            ParameterInfo[] paramInfo = constructorInfo.GetParameters();
            string paramString = "";
            foreach (ParameterInfo param in paramInfo)
            {
                string genericString = Util.getParamString(param.ParameterType);
                string attrString = param.Attributes != ParameterAttributes.None ? param.Attributes.ToString() + " " : param.ParameterType.IsByRef ? "Ref " : "";
                paramString += attrString + Util.getParamString(param.ParameterType) + ", ";
            }
            if (paramInfo.Length > 0)
            {
                paramString = paramString.Substring(0, paramString.Length - 2);
            }
            Name = constructorInfo.DeclaringType.Name;
            LeftBracket = "(";
            Argument = paramString;
            RightBracket = ")";
            Colon = " : ";
            ReturnValue = "Void";
            AccessRights = Util.getAccessibility(constructorInfo);
        }
    }



}
