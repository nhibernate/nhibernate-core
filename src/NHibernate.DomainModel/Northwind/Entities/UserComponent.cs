namespace NHibernate.DomainModel.Northwind.Entities
{
    public class UserComponent
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public UserComponent2 OtherComponent { get; set; }

		public string Property3 => $"{Property1}{Property2}";
    }

    public class UserComponent2
    {
        public string OtherProperty1 { get; set; }

		public string OtherProperty2 => OtherProperty1;
    }
}
