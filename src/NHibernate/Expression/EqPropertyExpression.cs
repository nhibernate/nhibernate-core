using System;

namespace NHibernate.Expression
{
    public class EqPropertyExpression : PropertyExpression
    {
        public EqPropertyExpression(string propertyName, string otherPropertyName)
            : base(propertyName, otherPropertyName)
        {
        }

        protected override string Op
        {
            get { return " = "; }
        }
    }
}