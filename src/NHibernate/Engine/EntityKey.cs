using System;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary>
	/// A globally unique identifier of an instance, consisting of the user-visible identifier
	/// and the identifier space (eg. tablename)
	/// </summary>
	[Serializable]
	public sealed class EntityKey
	{
		private readonly object identifier;
		private readonly object identifierSpace;
		private readonly System.Type clazz;
		private readonly IType identifierType;
		private readonly bool isBatchLoadable;
		private readonly ISessionFactoryImplementor factory;
		private readonly int hashCode;

		private EntityKey(
			object id,
			object identifierSpace,
			System.Type clazz,
			IType identifierType,
			bool isBatchLoadable,
			ISessionFactoryImplementor factory)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			if (!identifierType.ReturnedClass.IsAssignableFrom(id.GetType()))
			{
				throw new ArgumentException("identifier type mismatch", "id");
			}

			this.identifier = id;
			this.identifierSpace = identifierSpace;
			this.clazz = clazz;
			this.identifierType = identifierType;
			this.isBatchLoadable = isBatchLoadable;
			this.factory = factory;
			this.hashCode = GenerateHashCode();
		}

		/// <summary>
		/// Construct a unique identifier for an entity class instance
		/// </summary>
		/// <param name="id"></param>
		/// <param name="p"></param>
		public EntityKey(object id, IEntityPersister p)
			: this(id, p.IdentifierSpace, p.MappedClass, p.IdentifierType, p.IsBatchLoadable, p.Factory)
		{
		}

		/// <summary>
		/// The user-visible identifier
		/// </summary>
		public object Identifier
		{
			get { return identifier; }
		}

		/// <summary>
		/// 
		/// </summary>
		public System.Type MappedClass
		{
			get { return clazz; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsBatchLoadable
		{
			get { return isBatchLoadable; }
		}

		public override bool Equals(object other)
		{
			EntityKey otherKey = other as EntityKey;
			if (otherKey == null)
			{
				return false;
			}
			return
				otherKey.identifierSpace.Equals(this.identifierSpace) && identifierType.Equals(otherKey.Identifier, this.identifier);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		private int GenerateHashCode()
		{
			unchecked
			{
				int result = 17;
				result = 37 * result + identifierSpace.GetHashCode();
				result = 37 * result + identifierType.GetHashCode(identifier, factory);
				return result;
			}
		}

		/// <summary></summary>
		public override string ToString()
		{
			return identifier.ToString();
		}
	}
}