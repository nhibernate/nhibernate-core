namespace NHibernate.DomainModel
{
	public class Super 
	{
		protected string name;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				this.name = value;
			}
		}
	}
}