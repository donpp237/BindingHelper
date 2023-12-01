using System;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;

namespace BindingHelper
{
    public class Property : IDisposable
    {
        private object m_instance;
        private Dictionary<string, object> m_properties;

        public Property(object instance)
        {
            m_instance = instance;
            m_properties = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            if (m_properties != null)
            {
                m_properties.Clear();
                m_properties = null;
            }
            
            if (m_instance != null)
                m_instance = null;
        }

        public void SetProperty(object value, string memberName)
        {
            string propertyName = GetPropertyName(memberName);

            object savedValue;
            if (m_properties.TryGetValue(propertyName, out savedValue) == false)
                SetProperty(propertyName, value);

            if (savedValue == value)
                return;

            PropertyInfo property = GetProperty(memberName);
            if (property == null)
                return;

            SetProperty(propertyName, value);
            property.SetValue(m_instance, value);
        }

        public T GetProperty<T>(string memberName)
        {
            string propertyName = GetPropertyName(memberName);

            object value;
            if (m_properties.TryGetValue(propertyName, out value) == false)
            {
                value = default(T);
                m_properties.Add(propertyName, value);
            }

            return (T)value;
        }

        private void SetProperty(string propertyName, object value)
        {
            if (m_properties.ContainsKey(propertyName) == true)
                m_properties[propertyName] = value;
            else
                m_properties.Add(propertyName, value);
        }

        private PropertyInfo GetProperty(string memberName)
        {
            if(m_instance == null)
                return null;

            Type classType = m_instance.GetType();
            if (classType == null)
                return null;

            return classType.GetProperty(memberName);
        }

        private string GetPropertyName(string memberName) 
        {
            if (m_instance == null)
                return "";

            Type classType = m_instance.GetType();
            if (classType == null)
                return "";

            return $"{classType}.{memberName}";
        }
    }
}
