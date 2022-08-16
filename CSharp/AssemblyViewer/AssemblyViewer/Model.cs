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
        Class,
        Interface,
        Enum,
        Struct,
        Method,
        Property,
        Event,
        Field
    }
    public enum AccessRight
    {
        Public,
        Protected,
        Private,
        Internal
    }
    public class ModelObject
    {
        public string Name { get; set; }
        public AccessRight AccessRights { get; set; }
        public ViewerType Type { get; set; }
        public List<ModelObject> ChildList { get; set; }
    }
    //==========================================================//
    public class ModelBaseClass : ModelObject
    {
        public List<ModelBaseClass> DerivedList { get; set; }
        public ModelBaseClass BaseClass { get; set; }
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
            Type = ViewerType.Enum;
        }
    }
    public class ModelStruct : ModelBaseClass
    {
        public ModelStruct()
        {
            Type = ViewerType.Struct;
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
