using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class Patient
	{
		private IList<PatientRecord> patientRecords;
		private bool active;
		private Physician physician;

		protected Patient() { }
		public Patient(IEnumerable<PatientRecord> patientRecords, bool active, Physician physician)
		{
			this.active = active;
			this.physician = physician;
			this.patientRecords = new List<PatientRecord>(patientRecords);
			foreach (var record in this.patientRecords)
			{
				record.Patient = this;
			}
		}

		public virtual long Id { get; set; }

		public virtual bool Active
		{
			get { return active; }
			set { active = value; }
		}

		public virtual IList<PatientRecord> PatientRecords
		{
			get { return patientRecords; }
			set { patientRecords = value; }
		}

		public virtual Physician Physician
		{
			get { return physician; }
			set { physician = value; }
		}
	}

	public class Physician
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class PatientRecord
	{
		public virtual long Id { get; set; }
		public virtual PatientName Name { get; set; }
		public virtual DateTime BirthDate { get; set; }
		public virtual Gender Gender { get; set; }
		public virtual PatientAddress Address { get; set; }
		public virtual Patient Patient { get; set; }
	}

	public enum Gender
	{
		Unknown,
		Male,
		Female
	}

	public class PatientName
	{
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
	}

    public class PatientAddress
    {
        public virtual string AddressLine1 { get; set; }
        public virtual string AddressLine2 { get; set; }
        public virtual string City { get; set; }
        public virtual State State { get; set; }
        public virtual string ZipCode { get; set; }
    }
	public class State
	{
		public virtual long Id { get; set; }
		public virtual string Abbreviation { get; set; }
		public virtual string FullName { get; set; }
	}
}
