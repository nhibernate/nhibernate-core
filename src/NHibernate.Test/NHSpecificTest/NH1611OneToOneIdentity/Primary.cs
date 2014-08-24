namespace NHibernate.Test.NHSpecificTest.NH1611OneToOneIdentity
{
	public class Primary
	{
		private int id;
		private string description;
		private Adjunct adjunct;

		public Primary()
		{

		}

		virtual public int ID
		{
			get { return id; }
			set { id = value; }
		}


		virtual public string Description
		{
			get { return description; }
			set { description = value; }
		}
	
		virtual public Adjunct Adjunct
		{
			get { return adjunct; }
			set { adjunct = value; }
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Primary)) return false;
			return Equals((Primary) obj);
		}

		public virtual bool Equals(Primary obj)
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