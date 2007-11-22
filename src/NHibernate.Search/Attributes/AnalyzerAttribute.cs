using System;

namespace NHibernate.Search.Attributes
{
    /// <summary>
    /// Defines an analyzer for a given entity, method or field 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class AnalyzerAttribute : Attribute
    {
        private readonly System.Type type;

        public AnalyzerAttribute(System.Type value)
        {
            type = value;
        }

        public System.Type Type
        {
            get { return type; }
        }
    }
}