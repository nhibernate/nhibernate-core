namespace NHibernate.DomainModel
{
	public class Super 
	{
		protected string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}