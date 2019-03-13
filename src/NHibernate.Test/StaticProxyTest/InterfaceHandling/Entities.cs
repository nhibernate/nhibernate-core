using System;

namespace NHibernate.Test.StaticProxyTest.InterfaceHandling
{
	public class EntitySimple : IEntity
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }
	}

	public class EntityMultiInterfaces : IEntity, IEntityId
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

	public class EntityMixExplicitImplicitInterface : IEntity, IEntityId
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
