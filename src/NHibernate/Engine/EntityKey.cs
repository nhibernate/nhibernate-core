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
		private readonly bool isBatchLoadable;

		private EntityKey( object id, IType identifierType, object identifierSpace, System.Type clazz, bool isBatchLoadable )
		{
			if( id == null )
			{
				throw new ArgumentNullException( "id" );
			}

			if( !identifierType.ReturnedClass.IsAssignableFrom( id.GetType() ) )
			{
				throw new ArgumentException( "identifier type mismatch", "id" );
			}

			this.identifier = id;
			this.identifierSpace = identifierSpace;
			this.clazz = clazz;
			this.isBatchLoadable = isBatchLoadable;
		}

		/// <summary>
		/// Construct a unique identifier for an entity class instace
		/// </summary>
		/// <param name="id"></param>
		/// <param name="p"></param>
		public EntityKey( object id, IEntityPersister p ) : this( id, p.IdentifierType, p.IdentifierSpace, p.MappedClass, p.IsBatchLoadable )
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public override bool Equals( object other )
		{
			EntityKey otherKey = other as EntityKey;
			if( otherKey == null )
			{
				return false;
			}
			return otherKey.identifierSpace.Equals( this.identifierSpace ) && otherKey.Identifier.Equals( this.identifier );
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			int result = 17;
			result = 37 * result + identifierSpace.GetHashCode();
			result = 37 * result + identifier.GetHashCode();
			return result;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return identifier.ToString();
		}

	}
}