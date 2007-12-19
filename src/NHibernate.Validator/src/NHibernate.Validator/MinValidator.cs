namespace NHibernate.Validator
{
    using System;
    using Mapping;

    public class MinValidator : IValidator<MinAttribute>, IPropertyConstraint
    {
        private long min;

        #region IPropertyConstraint Members

        public void apply(Property property)
        {
            throw new NotImplementedException();
            //Column col = (Column)property.getColumnIterator().next();
            //col.setCheckConstraint(col.getName() + ">=" + min);
        }

        #endregion

        #region IValidator<MinAttribute> Members

        public bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is string)
            {
                try
                {
                    return Convert.ToDecimal(value) >= Convert.ToDecimal(min);
                    //return decimal.Parse((String) value).CompareTo(decimal.Parse(min.ToString())) >= 0;
                }
                catch(FormatException nfe)
                {
                    return false;
                }
            }
            else if (value is decimal)
            {
                return Convert.ToDecimal(value) >= Convert.ToDecimal(min);
            }
            else if (value is Int32)
            {
                return Convert.ToInt32(value) >= Convert.ToInt32(min);
            }
            else if (value is Int64)
            {
                return Convert.ToInt64(value) >= Convert.ToInt64(min);
            }

            return false;
        }

        public void Initialize(Attribute parameters)
        {
            min = ((MinAttribute) parameters).Value;
        }

        #endregion
    }
}