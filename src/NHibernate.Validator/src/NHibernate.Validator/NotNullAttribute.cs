namespace NHibernate.Validator
{
    using System;

	/// <summary>
	/// Not null constraint
	/// </summary>
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    [ValidatorClass(typeof(NotNullValidator))]
    public class NotNullAttribute : Attribute
    {
        private string message = "{validator.notEmpty}";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}