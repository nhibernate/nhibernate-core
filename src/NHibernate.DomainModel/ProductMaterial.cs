
using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	public class EntityVersioned<TKey>
		where TKey: struct, IComparable<TKey>
	{
		public static IEqualityComparer<TKey> KeyComparer { get; protected internal set; } = EqualityComparer<TKey>.Default;

		public virtual TKey Id { get; set; }

		public virtual int IntegrityVersion { get; set; }

		/// <summary>
		///		No business key, any 2 instances with same Ids considered equal
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;

			if (!(obj is EntityVersioned<TKey> thatEntity))
				return false;

			var thisType = GetTypeUnproxied();
			var thatType = thatEntity.GetTypeUnproxied();

			if (thisType != thatType)
				return false;

			return KeyComparer.Equals(Id, thatEntity.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public virtual bool IsTransient => IntegrityVersion <= 0;

		protected virtual System.Type GetTypeUnproxied()
		{
			return GetTypeUnproxied(this);
		}

		protected virtual bool IsPersistentWithSameIdAs(EntityVersioned<TKey> compareTo)
		{
			if (compareTo == null) throw new ArgumentNullException(nameof(compareTo));

			return !IsTransient && !compareTo.IsTransient && KeyComparer.Equals(Id, compareTo.Id);
		}

		public static System.Type GetTypeUnproxied(EntityVersioned<TKey> obj)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			var result = obj.GetType();

			while (IsProxyOrAccessor(result))
				result = result.BaseType;

			return result;
		}

		public static bool IsProxyOrAccessor(System.Type type) => type.Assembly.IsDynamic
		                                                   || typeof(Intercept.IFieldInterceptorAccessor).IsAssignableFrom(type)
		                                                   || typeof(Proxy.INHibernateProxy).IsAssignableFrom(type);
	}

	public class EntityVersionedWithName<TKey> : EntityVersioned<TKey>
		where TKey: struct, IComparable<TKey>
	{
		public virtual string Name { get; set; }
	}

	public class Material : EntityVersionedWithName<int>
	{

		public virtual MaterialDefinition MaterialDefinition { get; set; }

		public virtual ProductDefinition ProductDefinition { get; set; }
	}

	public class MaterialDefinition : EntityVersionedWithName<int>
	{
	}

	public class ProductDefinition : EntityVersionedWithName<int>
	{
		public virtual MaterialDefinition MaterialDefinition { get; set; }
	}
}
