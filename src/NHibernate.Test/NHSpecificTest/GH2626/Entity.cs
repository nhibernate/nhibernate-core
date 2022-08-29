using System;

namespace NHibernate.Test.NHSpecificTest.GH2626
{
	abstract class CapabilityAssignment
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	class ApplicationUser
	{
		public virtual Guid Id { get; set; }
		public virtual string UserName { get; set; }
	}

	class RoleCapabilityAssignment : CapabilityAssignment
	{
		public virtual Guid RoleId { get; set; }
	}

	class UserCapabilityAssignment : CapabilityAssignment
	{
		public virtual Guid UserId { get; set; }
	}
}
