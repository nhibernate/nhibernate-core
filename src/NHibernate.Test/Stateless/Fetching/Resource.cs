
namespace NHibernate.Test.Stateless.Fetching
{
	public class Resource
	{
		private long? id;
		private string name;
		private User owner;

		public Resource()
		{
		}

		public Resource(string name, User owner)
		{
			this.name = name;
			this.owner = owner;
		}

		public virtual long? Id
		{
			get
			{
				return id;
			}

			set
			{
				this.id = value;
			}
		}

		public virtual string Name
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

		public virtual User Owner
		{
			get
			{
				return owner;
			}

			set
			{
				this.owner = value;
			}
		}
	}
}
