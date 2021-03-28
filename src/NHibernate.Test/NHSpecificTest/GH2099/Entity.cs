using System;

namespace NHibernate.Test.NHSpecificTest.GH2099
{
	abstract class PersistentObject
	{
		public virtual Guid Id { get; set; }
		public virtual bool PDO_Deleted { get; set; }
	}

	class WorkflowInstance : PersistentObject
	{
		public virtual bool IsWaiting { get; set; }
	}
	
	class Level3 : WorkflowInstance
	{
		public virtual bool IsActive { get; set; }
	}
	
	class WorkflowInstance2 : PersistentObject
	{
		public virtual bool IsActive { get; set; }
		public virtual bool IsWaiting { get; set; }
	}
}
