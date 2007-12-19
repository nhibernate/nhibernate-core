namespace NHibernate.Validator
{
    using System;
    using Validator;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [ValidatorClass(typeof(LengthValidator))]
    public class LengthAttribute : Attribute
    {
        private int max;
        private int min;
        
        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        private string message = "{validator.length}";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}