namespace NHibernate.Test.NHSpecificTest.NH2907
{
	public class Group
	{
		public Group()
		{
			Name = string.Empty;
		}

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var casted = obj as Group;
			if (casted == null)
				return false;
			return casted.Id==Id && casted.Name.Equals(Name);
		}
	}
}
