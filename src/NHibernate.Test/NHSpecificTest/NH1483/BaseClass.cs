using System;

namespace NHibernate.Test.NHSpecificTest.NH1483
{
	public abstract class BaseClass
	{
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private Guid _id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

		public Guid Id
		{
			get { return _id; }
		}
	}
}