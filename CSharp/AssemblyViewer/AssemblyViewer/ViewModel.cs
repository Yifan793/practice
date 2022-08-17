using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AssemblyViewer
{
    public class ViewModelObject : IComparable<ViewModelObject>
    {
        public ViewModelObject()
        {
            FontBrush = new SolidColorBrush(Colors.Cornsilk);
        }
        public string Name { get; set; }
        public string LeftBracket { get; set; }
        public string RightBracket { get; set; }
        public string Colon { get; set; }
        public string Argument { get; set; }
        public string ReturnValue { get; set; }
        public AccessRights AccessRights { get; set; }
        public ViewerType Type { get; set; }
        public SolidColorBrush FontBrush { get; set; }
        public ObservableCollection<ViewModelObject> ChildList { get; set; } = new ObservableCollection<ViewModelObject>();
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
        public int CompareTo(ViewModelObject other)
        {
            return this.Name.CompareTo(other.Name);
        }
        public static string GetParamString(Type type)
        {
            Type[] generic = type.GenericTypeArguments;
            if (generic.Length == 0)
            {
                return type.Name;
            }
            string genericString = type.Name.Substring(0, type.Name.Length - 2) + "<";
            foreach (Type genericType in generic)
            {
                genericString += GetParamString(genericType) + ", ";
            }
            genericString = genericString.Substring(0, genericString.Length - 2);
            genericString += ">";
            return genericString;
        }
    }

    public class ViewModelAssembly : ViewModelObject
    {
        public ViewModelAssembly(string name)
        {
            Name = name;
            Type = ViewerType.Assembly;
            AccessRights = AccessRights.InValid;
        }
    }

    public class ViewModelNameSpace : ViewModelObject
    {
        public ViewModelNameSpace(string name)
        {
            Name = name;
            FontBrush = new SolidColorBrush(Colors.Gold);
            Type = ViewerType.Namespace;
            AccessRights = AccessRights.InValid;
        }
    }

    public class ViewModelBaseAndInterface : ViewModelObject
    {
        public ViewModelBaseAndInterface(string name)
        {
            Name = name;
            FontBrush = new SolidColorBrush(Colors.White);
        }
    }

    //==========================================================//

    public class ViewModelBaseClass : ViewModelObject
    {
        public virtual void DealType(Type type)
        {
            Name = type.Name;
            AccessRights = GetAccessibility(type);
            BaseList.Add(DealBaseType(type));
            MethodList = new ObservableCollection<ViewModelMethod>(DealMethods(type));
            EventList = new ObservableCollection<ViewModelEvent>(DealEvents(type));
            PropertyList = new ObservableCollection<ViewModelProperty>(DealProperties(type));
            FieldList = new ObservableCollection<ViewModelField>(DealField(type));
        }

        public ObservableCollection<ViewModelObject> BaseList { get; set; } = new ObservableCollection<ViewModelObject>();
        public ObservableCollection<ViewModelBaseClass> ClassList { get; set; } = new ObservableCollection<ViewModelBaseClass>();
        public ObservableCollection<ViewModelMethod> MethodList { get; set; } = new ObservableCollection<ViewModelMethod>();
        public ObservableCollection<ViewModelEvent> EventList { get; set; } = new ObservableCollection<ViewModelEvent>();
        public ObservableCollection<ViewModelProperty> PropertyList { get; set; } = new ObservableCollection<ViewModelProperty>();
        public ObservableCollection<ViewModelField> FieldList { get; set; } = new ObservableCollection<ViewModelField>();

        private static List<ViewModelMethod> DealMethods(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<ViewModelMethod> methodList = new List<ViewModelMethod>();
            foreach (MethodInfo method in methods)
            {
                Type declaringType = method.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name) || method.IsSpecialName)
                {
                    continue;
                }
                ViewModelMethod vmMethod = new ViewModelMethod(method);
                methodList.Add(vmMethod);
            }
            return methodList;
        }
        public static List<ViewModelField> DealField(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<ViewModelField> fieldList = new List<ViewModelField>();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Type declaringType = fieldInfo.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name) || fieldInfo.Name.EndsWith("k__BackingField"))
                {
                    continue;
                }
                ViewModelField vmField = new ViewModelField(fieldInfo);
                fieldList.Add(vmField);
            }
            return fieldList;
        }
        private static List<ViewModelProperty> DealProperties(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<ViewModelProperty> propertyList = new List<ViewModelProperty>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Type declaringType = propertyInfo.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name))
                {
                    continue;
                }
                ViewModelProperty vmProperty = new ViewModelProperty(propertyInfo);
                propertyList.Add(vmProperty);
            }
            return propertyList;
        }
        private static List<ViewModelEvent> DealEvents(Type type)
        {
            EventInfo[] eventInfos = type.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<ViewModelEvent> eventList = new List<ViewModelEvent>();
            foreach (EventInfo eventInfo in eventInfos)
            {
                Type declaringType = eventInfo.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name))
                {
                    continue;
                }
                ViewModelEvent vmEvent = new ViewModelEvent(eventInfo);
                eventList.Add(vmEvent);
            }
            return eventList;
        }
        public static ViewModelObject DealBaseType(Type type)
        {
            ViewModelBaseAndInterface baseObject = new ViewModelBaseAndInterface("基类和接口");
            ViewModelClass baseClass = GetBaseClass(type);
            if (baseClass != null)
            {
                baseObject.ChildList.Add(baseClass);
            }
            Type[] interfaces = type.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                ViewModelInterface baseInterface = new ViewModelInterface();
                baseInterface.Name = interfaceType.Name;
                baseObject.ChildList.Add(baseInterface);
            }
            return baseObject;
        }
        private static ViewModelClass GetBaseClass(Type type)
        {
            if (type.BaseType == null)
            {
                return null;
            }
            ViewModelClass cur = new ViewModelClass();
            cur.Name = type.BaseType.Name;
            ViewModelClass baseClass = GetBaseClass(type.BaseType);
            if (baseClass != null)
            {
                cur.BaseList.Add(baseClass);
            }
            return cur;
        }

        private static bool IsPublic(Type t)
        {
            return
                t.IsVisible
                && t.IsPublic
                && !t.IsNotPublic
                && !t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }
        private static bool IsInternal(Type t)
        {
            return
                !t.IsVisible
                && !t.IsPublic
                && t.IsNotPublic
                && !t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }
        private static bool IsProtected(Type t)
        {
            return
                !t.IsVisible
                && !t.IsPublic
                && !t.IsNotPublic
                && t.IsNested
                && !t.IsNestedPublic
                && t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }
        private static bool IsPrivate(Type t)
        {
            return
                !t.IsVisible
                && !t.IsPublic
                && !t.IsNotPublic
                && t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }
        private static AccessRights GetAccessibility(Type type)
        {
            if (IsPublic(type))
            {
                return AccessRights.Public;
            }
            else if (IsProtected(type))
            {
                return AccessRights.Protected;
            }
            else if (IsPrivate(type))
            {
                return AccessRights.Private;
            }
            else if (IsInternal(type))
            {
                return AccessRights.Internal;
            }
            else if (type.IsSealed)
            {
                return AccessRights.Sealed;
            }
            return AccessRights.Public;
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
        public ViewModelClass()
        {
            FontBrush = new SolidColorBrush(Colors.MediumTurquoise);
            Type = ViewerType.Class;
        }
        public override void DealType(Type type)
        {
            base.DealType(type);
            ConstructorInfo[] infoList = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (ConstructorInfo info in infoList)
            {
                ViewModelConstructor method = new ViewModelConstructor(info);
                ConstructorList.Add(method);
            }
        }
        public ObservableCollection<ViewModelConstructor> ConstructorList { get; set; } = new ObservableCollection<ViewModelConstructor>();
    }

    public class ViewModelEnum : ViewModelBaseClass
    {
        public ViewModelEnum()
        {
            FontBrush = new SolidColorBrush(Colors.LightGreen);
            Type = ViewerType.Enumeration;
        }
        public override void DealType(Type type)
        {
            Name = type.Name;
            Type = ViewerType.Enumeration;
            string[] enums = type.GetEnumNames();
            var fieldList = FieldList.ToList();
            fieldList.AddRange(DealField(type));
            FieldList = new ObservableCollection<ViewModelField>(fieldList);
            foreach (string enumName in enums)
            {
                ViewModelEnumItem vmObject = new ViewModelEnumItem();
                vmObject.Name = enumName;
                vmObject.ReturnValue = type.Name;
                EnumItemList.Add(vmObject);
            }
            BaseList.Add(DealBaseType(type));
        }
        public ObservableCollection<ViewModelEnumItem> EnumItemList { get; set; } = new ObservableCollection<ViewModelEnumItem>();
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
        public ViewModelMethod(MethodInfo methodInfo)
        {
            FontBrush = new SolidColorBrush(Colors.Orange);
            Type = ViewerType.Method;
            ParameterInfo[] paramInfo = methodInfo.GetParameters();
            string paramString = "";
            foreach (ParameterInfo param in paramInfo)
            {  
                string genericString = GetParamString(param.ParameterType);
                string attrString = param.Attributes != ParameterAttributes.None ? param.Attributes.ToString() + " " : param.ParameterType.IsByRef ? "Ref " : "";
                paramString += attrString + GetParamString(param.ParameterType) +  ", ";
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
            ReturnValue = GetParamString(methodInfo.ReturnType);
            AccessRights = GetAccessibility(methodInfo);
        }
        private static AccessRights GetAccessibility(MethodInfo method)
        {
            if (method == null)
            {
                return AccessRights.InValid;
            }
            if (method.IsPublic)
            {
                return AccessRights.Public;
            }
            else if (method.IsPrivate)
            {
                return AccessRights.Private;
            }
            else if (method.IsFamily)
            {
                return AccessRights.Protected;
            }
            else if (method.IsAssembly)
            {
                return AccessRights.Internal;
            }
            return AccessRights.InValid;
        }
    }
    public class ViewModelEvent : ViewModelObject
    {
        public ViewModelEvent(EventInfo eventInfo)
        {
            FontBrush = new SolidColorBrush(Colors.LightPink);
            Type = ViewerType.Event;
            Name = eventInfo.Name;
            Colon = " : ";
            ReturnValue = GetParamString(eventInfo.EventHandlerType);

            MethodInfo addMethod = eventInfo.GetAddMethod();
            if (addMethod != null)
            {
                ViewModelMethod vmAddMethod = new ViewModelMethod(addMethod);
                ChildList.Add(vmAddMethod);
            }

            MethodInfo removeMethod = eventInfo.GetRemoveMethod();
            if (removeMethod != null)
            {
                ViewModelMethod vmRemoveMethod = new ViewModelMethod(removeMethod);
                ChildList.Add(vmRemoveMethod);
            }
        }
    }

    public class ViewModelProperty : ViewModelObject
    {
        public ViewModelProperty(PropertyInfo propertyInfo)
        {
            FontBrush = new SolidColorBrush(Colors.DarkCyan);
            Type = ViewerType.Property;
            Name = propertyInfo.Name;
            Colon = " : ";
            ReturnValue = GetParamString(propertyInfo.PropertyType);
            AccessRights = GetAccessibility(propertyInfo);
            MethodInfo[] methods = propertyInfo.GetAccessors(true);
            foreach (MethodInfo method in methods)
            {
                ViewModelMethod vmMethod = new ViewModelMethod(method);
                ChildList.Add(vmMethod);
            }
        }
        private static AccessRights GetAccessibility(PropertyInfo property)
        {
            if (property == null)
            {
                return AccessRights.InValid;
            }
            if (property.GetMethod == null)
            {
                return AccessRights.InValid;
            }
            if (property.GetMethod.IsPublic)
            {
                return AccessRights.Public;
            }
            else if (property.GetMethod.IsPrivate)
            {
                return AccessRights.Private;
            }
            else if (property.GetMethod.IsFamily)
            {
                return AccessRights.Protected;
            }
            else if (property.GetMethod.IsAssembly)
            {
                return AccessRights.Internal;
            }
            return AccessRights.InValid;
        }
    }
    public class ViewModelField : ViewModelObject
    {
        public ViewModelField(FieldInfo fieldInfo)
        {
            FontBrush = new SolidColorBrush(Colors.MediumPurple);
            Type = ViewerType.Field;
            Name = fieldInfo.Name;
            Colon = " : ";
            ReturnValue = GetParamString(fieldInfo.FieldType);
            AccessRights = GetAccessibility(fieldInfo);
        }

        private static AccessRights GetAccessibility(FieldInfo field)
        {
            if (field == null)
            {
                return AccessRights.InValid;
            }
            if (field.IsPublic)
            {
                return AccessRights.Public;
            }
            else if (field.IsPrivate)
            {
                return AccessRights.Private;
            }
            else if (field.IsFamily)
            {
                return AccessRights.Protected;
            }
            else if (field.IsAssembly)
            {
                return AccessRights.Internal;
            }
            return AccessRights.InValid;
        }
    }
    public class ViewModelConstructor : ViewModelObject
    {
        public ViewModelConstructor(ConstructorInfo constructorInfo)
        {
            FontBrush = new SolidColorBrush(Colors.MediumTurquoise);
            Type = ViewerType.Method;
            ParameterInfo[] paramInfo = constructorInfo.GetParameters();
            string paramString = "";
            foreach (ParameterInfo param in paramInfo)
            {
                string genericString = GetParamString(param.ParameterType);
                string attrString = param.Attributes != ParameterAttributes.None ? param.Attributes.ToString() + " " : param.ParameterType.IsByRef ? "Ref " : "";
                paramString += attrString + GetParamString(param.ParameterType) + ", ";
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
            AccessRights = GetAccessibility(constructorInfo);
        }

        private static AccessRights GetAccessibility(ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
            {
                return AccessRights.InValid;
            }
            if (constructorInfo.IsPublic)
            {
                return AccessRights.Public;
            }
            else if (constructorInfo.IsPrivate)
            {
                return AccessRights.Private;
            }
            else if (constructorInfo.IsFamily)
            {
                return AccessRights.Protected;
            }
            else if (constructorInfo.IsAssembly)
            {
                return AccessRights.Internal;
            }
            return AccessRights.InValid;
        }
    }
}
