using System;

namespace NHibernate.Test.NHSpecificTest.NH1556
{
	public class Claim
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private Guid id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
		private DateTime lastFilled;
		private ProductIdentifier productIdentifier;
		private Patient patient;

		protected Claim()
		{
		}

		public Claim(Patient patient, DateTime lastFilled, ProductIdentifier productIdentifier)
		{
			this.patient = patient;
			this.lastFilled = lastFilled;
			this.productIdentifier = productIdentifier;
		}

		public virtual Guid Id
		{
			get { return id; }
		}

		public virtual DateTime LastFilled
		{
			get { return lastFilled; }
		}

		public virtual ProductIdentifier ProductIdentifier
		{
			get { return productIdentifier; }
		}

		public virtual Patient Patient
		{
			get { return patient; }
		}
	}
}
