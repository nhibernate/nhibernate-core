using System;

namespace NHibernate.Expression
{
    public class LtPropertyExpression : PropertyExpression
    {
        public LtPropertyExpression(string propertyName, string otherPropertyName)
            : base(propertyName, otherPropertyName)
        {
        }

        protected override string Op
        {
            get { return " < "; }
        }
    }
}