namespace NHibernate.Validator
{
    using System;

    public class ValidatorClassAttribute : Attribute
    {
        private Type value;

        public ValidatorClassAttribute()
        {
        }

        public ValidatorClassAttribute(Type value)
        {
            this.value = value;
        }

        public Type Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}