using log4net;

namespace NHibernate.Test.NHSpecificTest.NH1810
{
	public class Disease
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Fixture));

		int id;
		string name;
		int duration;
		MedicalRecord medicalRecord;

		public virtual int Id
		{
			get { return id; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual int Duration
		{
			get { return duration; }
			set { duration = value; }
		}

		public virtual MedicalRecord MedicalRecord
		{
			get { return medicalRecord; }
			set { medicalRecord = value; }
		}

		public virtual bool Equals(Disease other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Name, Name) && other.Duration == Duration && Equals(other.MedicalRecord, MedicalRecord);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Disease)) return false;
			return Equals((Disease) obj);
		}

		int? hashCode;

		public override int GetHashCode()
		{
			Log.Debug("Disease.GetHashCode()");

			if (!hashCode.HasValue)
				unchecked
				{
					int result = (Name != null ? Name.GetHashCode() : 0);
					result = (result*397) ^ Duration;
					result = (result*397) ^ (MedicalRecord != null ? MedicalRecord.GetHashCode() : 0);
					hashCode = result;
				}

			return hashCode.Value;
		}
	}
}
