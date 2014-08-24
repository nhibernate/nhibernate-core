namespace NHibernate.Test.Events.Collections
{
	public class ChildValue: IChild
	{
		private string name;
		public ChildValue() {}
		public ChildValue(string name)
		{
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;
			ChildValue that = obj as ChildValue;
			if (that == null)
				return false;
			return name == that.name;
		}

		public override int GetHashCode()
		{
			return name == null ? base.GetHashCode() : name.GetHashCode();
		}
	}
}