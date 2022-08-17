﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyViewer
{
    public static class Util
    {
        public static ViewModelObject DealBaseType(Type type)
        {
            ViewModelObject baseObject = new ViewModelObject();
            baseObject.Name = "基类和接口";

            ViewModelClass baseClass = getBaseClass(type);
            if (baseClass != null)
            {
                baseObject.ChildList.Add(baseClass);
            }

            Type[] interfaces = type.GetInterfaces();
            foreach(Type interfaceType in interfaces)
            {
                ViewModelInterface baseInterface = new ViewModelInterface();
                baseInterface.Name = interfaceType.Name;
                baseObject.ChildList.Add(baseInterface);
            }
            return baseObject;
        }
        private static ViewModelClass getBaseClass(Type type)
        {
            if (type.BaseType == null)
            {
                return null;
            }
            ViewModelClass cur = new ViewModelClass();
            cur.Name = type.BaseType.Name;
            ViewModelClass baseClass = getBaseClass(type.BaseType);
            if (baseClass != null)
            {
                cur.ChildList.Add(baseClass);
            }
            return cur;
        }
        public static List<ViewModelMethod> DealMethods(Type type)
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
        public static List<ViewModelEvent> DealEvents(Type type)
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
                ViewModelEvent vmEvent = new ViewModelEvent();
                vmEvent.Name = eventInfo.Name;
                vmEvent.Colon = " : "; 
                vmEvent.ReturnValue = Util.getParamString(eventInfo.EventHandlerType);

                MethodInfo addMethod = eventInfo.GetAddMethod();
                if (addMethod != null)
                {
                    ViewModelMethod vmAddMethod = new ViewModelMethod(addMethod);
                    vmEvent.ChildList.Add(vmAddMethod);
                }

                MethodInfo removeMethod = eventInfo.GetRemoveMethod();
                if (removeMethod != null)
                {
                    ViewModelMethod vmRemoveMethod = new ViewModelMethod(removeMethod);
                    vmEvent.ChildList.Add(vmRemoveMethod);
                }

                eventList.Add(vmEvent);
            }
            return eventList;
        }
        public static List<ViewModelProperty> DealProperties(Type type)
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
                ViewModelProperty vmProperty = new ViewModelProperty();
                vmProperty.Name = propertyInfo.Name;
                vmProperty.Colon = " : ";
                vmProperty.ReturnValue = Util.getParamString(propertyInfo.PropertyType);
                vmProperty.AccessRights = getAccessibility(propertyInfo);
                MethodInfo[] methods = propertyInfo.GetAccessors(true);
                foreach (MethodInfo method in methods)
                {
                    ViewModelMethod vmMethod = new ViewModelMethod(method);
                    vmProperty.ChildList.Add(vmMethod);
                }
                propertyList.Add(vmProperty);
            }
            return propertyList;
        }
        public static List<ViewModelField> DealFields(Type type)
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
                ViewModelField vmField = new ViewModelField();
                vmField.Name = fieldInfo.Name;
                vmField.Colon = " : ";
                vmField.ReturnValue = Util.getParamString(fieldInfo.FieldType);
                vmField.AccessRights = getAccessibility(fieldInfo);
                fieldList.Add(vmField);
            }
            return fieldList;
        }

        //========================================================================//
        public static void SortAll(List<ViewModelObject> vmList)
        {
            if (vmList == null)
            {
                return;
            }
            vmList.Sort();
            foreach (ViewModelObject vm in vmList)
            {
                SortAll(vm.ChildList);
                if (vm is ViewModelBaseClass BaseClass)
                {
                    BaseClass.MethodList.Sort();
                    BaseClass.EventList.Sort();
                    BaseClass.PropertyList.Sort();
                    BaseClass.FieldList.Sort();
                }
            }
        }

        //========================================================================//
        private static bool isPublic(Type t)
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
        private static bool isInternal(Type t)
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
        // only nested types can be declared "protected"
        private static bool isProtected(Type t)
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
        // only nested types can be declared "private"
        private static bool isPrivate(Type t)
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
        public static AccessRights getAccessibility(Type type)
        {
            if (isPublic(type))
            {
                return AccessRights.Public;
            }
            else if (isProtected(type))
            {
                return AccessRights.Protected;
            }
            else if (isPrivate(type))
            {
                return AccessRights.Private;
            }
            else if (isInternal(type))
            {
                return AccessRights.Internal;
            }
            return AccessRights.Public;
        }
        public static AccessRights getAccessibility(MethodInfo method)
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
        public static AccessRights getAccessibility(ConstructorInfo constructorInfo)
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
        private static AccessRights getAccessibility(PropertyInfo property)
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
        private static AccessRights getAccessibility(FieldInfo field)
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

        //========================================================================//
        public static string getParamString(Type type)
        {
            Type[] generic = type.GenericTypeArguments;
            if (generic.Length == 0)
            {
                return type.Name;
            }
            string genericString = type.Name.Substring(0, type.Name.Length - 2) + "<";
            foreach (Type genericType in generic)
            {
                genericString += getParamString(genericType) + ", ";
            }
            genericString = genericString.Substring(0, genericString.Length - 2);
            genericString += ">";
            return genericString;
        }
    }
}
