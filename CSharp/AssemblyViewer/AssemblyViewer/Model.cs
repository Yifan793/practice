using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyViewer
{
    class Model
    {
    }
    class ModelNameSpace
    {
        public string Name { get; set; }
        public List<ModelObject> ObjectList { get; set; }
    }
    class ModelObject
    {
        public string Name { get; set; }
        public string AccessRights { get; set; }
    }
    //==========================================================//
    public enum ClassType
    {
        Class,
        Interface,
        Enum,
        Struct
    }
    class ModelBaseClass : ModelObject
    {
        public List<ModelBaseClass> DerivedList { get; set; }
        public ModelBaseClass BaseClass { get; set; }
        public List<ModelMethod> MethodList { get; set; }
        public List<ModelProperty> PropertyList { get; set; }
        public List<ModelEvent> EventList { get; set; }
        public ClassType Type { get; set; }
    }
    class ModelInterface : ModelBaseClass
    {
        public ModelInterface()
        {
            Type = ClassType.Interface;
        }
    }
    class ModelClass : ModelBaseClass
    {
        public ModelClass()
        {
            Type = ClassType.Class;
        }
    }
    class ModelEnum : ModelBaseClass
    {
        public ModelEnum()
        {
            Type = ClassType.Enum;
        }
    }
    class ModelStruct : ModelBaseClass
    {
        public ModelStruct()
        {
            Type = ClassType.Struct;
        }
    }
    //==========================================================//
    class ModelMethod : ModelObject
    {
    }
    class ModelEvent : ModelObject
    {
        public List<ModelMethod> MethodList { get; set; }
    }

    class ModelProperty : ModelObject
    {
        public List<ModelMethod> MethodList { get; set; }
    }

    class ModelField : ModelObject
    {

    }

}
