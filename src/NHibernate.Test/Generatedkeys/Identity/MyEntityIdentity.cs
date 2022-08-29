namespace NHibernate.Test.Generatedkeys.Identity
{
	public class MyEntityIdentity
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private int id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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
