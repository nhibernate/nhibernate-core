using System;

namespace NHibernate.Test.StaticProxyTest.InterfaceHandling
{
	public class EntitySimple : IEntity
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }
	}

	public class EntityMultiInterfaces : IEntity, IEntity2
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }
	}

	public class EntityExplicitInterface : IEntity
	{
		private Guid id;
		private string name;

		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		Guid IEntity.Id
		{
			get => id;
			set => id = value;
		}

		string IEntity.Name
		{
			get => name;
			set => name = value;
		}
	}

	//Proxy contains IEntity.Id and IEntity2.Id
	public interface IMultiIdProxy : IEntity, IEntity2
	{
	}

	public class EntityMultiIdProxy : IMultiIdProxy
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class EntityMixExplicitImplicitInterface : IEntity, IEntity2
	{
		private Guid id;
		private string name;

		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		Guid IEntity.Id
		{
			get => id;
			set => id = value;
		}

		string IEntity.Name
		{
			get => name;
			set => name = value;
		}
	}

	public class EntityWithSuperClassInterfaceLookup
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IEntity EntityLookup { get; set; }
	}
}
