using System;

using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Expression
{
    /// <summary>
    /// Superclass for comparisons between two properties (with SQL binary operators)
    /// </summary>
    public abstract class PropertyExpression : Expression
    {
        private string propertyName;
        private string otherPropertyName;

        private static TypedValue[] NoTypedValues = new TypedValue[0];

        protected PropertyExpression(string propertyName, string otherPropertyName)
        {
            this.propertyName = propertyName;
            this.otherPropertyName = otherPropertyName;
        }

        public override SqlString ToSqlString(ISessionFactoryImplementor factory, System.Type persistentClass, string alias)
        {
            SqlStringBuilder sqlBuilder = new SqlStringBuilder();

            string[] columnNames = GetColumns(factory, persistentClass, propertyName, alias);
            string[] otherColumnNames = GetColumns(factory, persistentClass, otherPropertyName, alias);

            bool andNeeded = false;

            for (int i = 0; i < columnNames.Length; i++)
            {
                if (andNeeded)
                    sqlBuilder.Add(" AND ");
                andNeeded = true;
                
                sqlBuilder.Add(columnNames[i]).Add(Op).Add(otherColumnNames[i]);
            }            

            return sqlBuilder.ToSqlString();

            //TODO: get SQL rendering out of this package!
        }

        public override TypedValue[] GetTypedValues(ISessionFactoryImplementor sessionFactory, System.Type persistentClass)
        {
            return NoTypedValues;
        }

        public override string ToString()
        {
            return propertyName + Op + otherPropertyName;
        }

        protected abstract string Op
        {
            get;
        }
    }
}