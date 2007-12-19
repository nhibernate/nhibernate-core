namespace NHibernate.Validator
{
    using System;

    public class NotNullValidator : NotEmptyNotNullPropertyConstraint, IValidator<NotNullAttribute>
    {
        public bool IsValid(object value)
        {
            return value != null;
        }

        public void Initialize(Attribute parameters)
        {
        }

        //public void Initialize(NotNullAttribute parameters)
        //{
        //}
    }
}