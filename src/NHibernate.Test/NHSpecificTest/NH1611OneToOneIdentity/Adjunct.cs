namespace NHibernate.Test.NHSpecificTest.NH1611OneToOneIdentity
{
	public class Adjunct
	{
		private int id;
		private string adjunctDescription;

		public Adjunct()
		{

		}

		virtual public int ID
		{
			get { return id; }
			set { id = value; }
		}

		virtual public string AdjunctDescription
		{
			get { return adjunctDescription; }
			set { adjunctDescription = value; }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Adjunct)) return false;
			return Equals((Adjunct) obj);
		}

		public virtual bool Equals(Adjunct obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.id == id;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (id*397);
			}
		}
	}
}