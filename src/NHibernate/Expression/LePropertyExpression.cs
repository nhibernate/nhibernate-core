using System;

namespace NHibernate.Expression
{
    public class LePropertyExpression : PropertyExpression
    {
        public LePropertyExpression(string propertyName, string otherPropertyName) 
            : base(propertyName, otherPropertyName)
        {
        }

        protected override string Op
        {
            get { return " <= "; }
        }
    }
}