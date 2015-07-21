using System.Collections.Generic;
using log4net;

namespace NHibernate.Test.NHSpecificTest.NH1810
{
	public class MedicalRecord
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Fixture));

		int id;
		ISet<Disease> diseases = new HashSet<Disease>();
		string reference;

		public virtual int Id
		{
			get { return id; }
		}

		public virtual ISet<Disease> Diseases
		{
			get { return diseases; }
		}

		public virtual string Reference
		{
			get { return reference; }
			set { reference = value; }
		}

		public virtual bool Equals(MedicalRecord other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Reference, Reference);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (MedicalRecord)) return false;
			return Equals((MedicalRecord) obj);
		}

		int? hashCode;

		public override int GetHashCode()
		{
			Log.Debug("MedicalRecord.GetHashCode()");

			if (!hashCode.HasValue)
				hashCode = (Reference != null ? Reference.GetHashCode() : 0);

			return hashCode.Value;
		}
	}
}
