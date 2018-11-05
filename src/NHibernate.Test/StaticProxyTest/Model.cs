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
}
