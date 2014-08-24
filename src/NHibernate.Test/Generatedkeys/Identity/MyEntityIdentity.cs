namespace NHibernate.Test.Generatedkeys.Identity
{
	public class MyEntityIdentity
	{
		private int id;
		private string name;

		public virtual int Id
		{
			get { return id; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}