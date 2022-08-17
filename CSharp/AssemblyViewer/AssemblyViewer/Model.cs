using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyViewer
{
    public class ModelNameSpace
    {
        public string Name { get; set; }
        public List<ModelObject> ChildList { get; set; }
    }
    public enum ViewerType
    {
        InValid = -1,
        Class,
        Constant,
        Delegate,
        Interface,
        Enumeration,
        EnumerationItem,
        Structure,
        Method,
        Property,
        Event,
        Field,
        Exception,
        Module,
        Operator,
        Namespace,
    }
    public enum AccessRights
    {
        InValid = -1,
        Public,
        Protected,
        Private,
        Internal,
        Shortcut,
        Sealed
    }
    public class ModelObject
    {
        public string Name { get; set; }
        public AccessRights AccessRights { get; set; }
        public ViewerType Type { get; set; }
        public List<ModelObject> ChildList { get; set; }
    }
    //==========================================================//
    public class ModelBaseClass : ModelObject
    {
        public ModelObject BaseClass { get; set; }
        public List<ModelMethod> MethodList { get; set; }
        public List<ModelProperty> PropertyList { get; set; }
        public List<ModelEvent> EventList { get; set; }
    }
    public class ModelInterface : ModelBaseClass
    {
        public ModelInterface()
        {
            Type = ViewerType.Interface;
        }
    }
    public class ModelClass : ModelBaseClass
    {
        public ModelClass()
        {
            Type = ViewerType.Class;
        }
    }
    public class ModelEnum : ModelBaseClass
    {
        public ModelEnum()
        {
            Type = ViewerType.Enumeration;
        }
    }
    public class ModelStruct : ModelBaseClass
    {
        public ModelStruct()
        {
            Type = ViewerType.Structure;
        }
    }
    //==========================================================//
    public class ModelMethod : ModelObject
    {
        public ModelMethod()
        {
            Type = ViewerType.Method;
        }
    }
    public class ModelEvent : ModelObject
    {
        public List<ModelMethod> MethodList { get; set; }
        public ModelEvent()
        {
            Type = ViewerType.Event;
        }
    }

    public class ModelProperty : ModelObject
    {
        public List<ModelMethod> MethodList { get; set; }
        public ModelProperty()
        {
            Type = ViewerType.Property;
        }
    }

    public class ModelField : ModelObject
    {
        public ModelField()
        {
            Type = ViewerType.Field;
        }
    }

}
