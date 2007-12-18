using System;

namespace NHibernate.Search.Attributes
{
    /// <summary>
    /// This annotation allows a user to apply an implementation
    /// class to a Lucene document to manipulate it in any way
    /// the usersees fit.
    /// </summary>
    /// <remarks>We allow multiple instances of this attribute rather than having a ClassBridgesAttribute as per Java</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ClassBridgeAttribute : Attribute
    {
        private string name = null;
        private Store store = Store.No;
        private System.Type analyzer;
        private Index index = Index.Tokenized;
        private float boost = 1.0F;
        private readonly System.Type impl;
        private readonly object[] parameters;

        public ClassBridgeAttribute(System.Type impl, params object[] parameters)
        {
            this.impl = impl;
            this.parameters = parameters;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Store Store
        {
            get { return store; }
            set { store = value; }
        }

        public Index Index
        {
            get { return index; }
            set { index = value; }
        }

        public System.Type Analyzer
        {
            get { return analyzer; }
            set { analyzer = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float Boost
        {
            get { return boost; }
            set { boost = value; }
        }

        public System.Type Impl
        {
            get { return impl; }
        }

        public object[] Parameters
        {
            get { return parameters; }
        }
    }
}