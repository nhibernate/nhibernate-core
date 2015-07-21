using log4net;

namespace NHibernate.Test.NHSpecificTest.NH1810
{
	public class Doctor
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Fixture));

		int id;
		MedicalRecord medicalRecord;
		int doctorNumber;

		public virtual int Id
		{
			get { return id; }
		}

		public virtual MedicalRecord MedicalRecord
		{
			get { return medicalRecord; }
			set { medicalRecord = value; }
		}

		public virtual int DoctorNumber
		{
			get { return doctorNumber; }
			set { doctorNumber = value; }
		}

		public virtual bool Equals(Doctor other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.doctorNumber == doctorNumber;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Doctor)) return false;
			return Equals((Doctor) obj);
		}

		int? hashCode;

		public override int GetHashCode()
		{
			Log.Debug("Doctor.GetHashCode()");

			if (!hashCode.HasValue)
				hashCode = doctorNumber.GetHashCode();
			
			return hashCode.Value;
		}
	}
}
