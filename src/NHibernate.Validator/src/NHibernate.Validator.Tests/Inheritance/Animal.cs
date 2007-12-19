namespace NHibernate.Validator.Tests.Inheritance
{
    public class Animal : IName
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}