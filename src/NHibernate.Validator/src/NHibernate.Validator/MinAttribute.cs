namespace NHibernate.Validator
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [ValidatorClass(typeof(MinValidator))]
    public class MinAttribute : Attribute
    {
        private string message = "{validator.min}";
        private long value;

        public MinAttribute(long min)
        {
            value = min;
        }

        public long Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}