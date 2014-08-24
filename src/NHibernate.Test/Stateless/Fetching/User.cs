
namespace NHibernate.Test.Stateless.Fetching
{
	public class User
	{
		private long? id;
		private string name;

		public User()
		{
		}

		public User(string name)
		{
			this.name = name;
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

	}
}
