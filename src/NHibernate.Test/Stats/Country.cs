namespace NHibernate.Test.Stats
{
	public class Country
	{
		private int id;
		private string name;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			Country that = obj as Country;
			if (that == null) return false;
			if (!name.Equals(that.name)) return false;

			return true;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}
}
