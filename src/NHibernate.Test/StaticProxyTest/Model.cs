using System;

namespace NHibernate.Test.StaticProxyTest
{
	public interface IEntity
	{
		Guid Id { get; set; }
		string Name { get; set; }
		string Text { get; set; }
	}

	public interface ILazyTextEntity
	{
		Guid Id { get; set; }
		string Name { get; set; }
		string Text { get; set; }
	}

	[Serializable]
	public class SimpleEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Text { get; set; }
	}

	[Serializable]
	public class InterfacedEntity : IEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Text { get; set; }
	}

	[Serializable]
	public class LazyTextEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Text { get; set; }
	}

	[Serializable]
	public class InterfacedLazyTextEntity : ILazyTextEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Text { get; set; }
	}

	[Serializable]
	public class OverridingEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Text { get; set; }

		public override bool Equals(object obj)
		{
			return (obj as OverridingEntity)?.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}

	[Serializable]
	public class OverridingEntityWithField
	{
		private Guid _id;
		public virtual Guid Id
		{
			get => _id;
			set => _id = value;
		}

		public virtual string Name { get; set; }
		public virtual string Text { get; set; }

		public override bool Equals(object obj)
		{
			return (obj as OverridingEntityWithField)?.Id == _id;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}
	}

	[Serializable]
	public class OverridingEntityWithFieldChild : OverridingEntityWithField
	{
	}
}
