namespace NHibernate.Validator
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    [ValidatorClass(typeof(NotEmptyValidator))]
    public class NotEmptyAttribute : Attribute
    {
        private string message = "{validator.notEmpty}";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}