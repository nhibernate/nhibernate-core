using log4net;

namespace NHibernate.Test.NHSpecificTest.NH1810
{
	public class Parent
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Fixture));

		int id;
		IChildren children = new Children();
		MedicalRecord medicalRecord = new MedicalRecord();
		string address;
		int visits;

		public virtual int Id
		{
			get { return id; }
		}

		public virtual IChildren Children
		{
			get { return children; }
		}

		public virtual MedicalRecord MedicalRecord
		{
			get { return medicalRecord; }
		}

		public virtual string Address
		{
			get { return address; }
			set { address = value; }
		}

		public virtual int Visits
		{
			get { return visits; }
			set { visits = value; }
		}

		public virtual bool Equals(Parent other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.MedicalRecord, MedicalRecord) && Equals(other.Address, Address) && other.Visits == Visits;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Parent)) return false;
			return Equals((Parent) obj);
		}

		int? hashCode;

		public override int GetHashCode()
		{
			Log.Debug("Parent.GetHashCode()");

			if (!hashCode.HasValue)
				unchecked
				{
					int result = (MedicalRecord != null ? MedicalRecord.GetHashCode() : 0);
					result = (result*397) ^ (Address != null ? Address.GetHashCode() : 0);
					result = (result*397) ^ Visits;
					hashCode = result;
				}

			return hashCode.Value;
		}
	}
}
