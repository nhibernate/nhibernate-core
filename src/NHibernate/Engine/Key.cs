using System;

using NHibernate.Collection;
using NHibernate.Persister;

namespace NHibernate.Engine 
{
	/// <summary>
	/// A globally unique identifier of an instance, consisting of the user-visible identifier
	/// and the identifier space (eg. tablename)
	/// </summary>
	[Serializable]
	public sealed class Key 
	{
		private object id;
		private object identifierSpace;

		private Key(object id, object identifierSpace) 
		{
			if (id==null) throw new ArgumentException("null identifier", "id");
			this.id = id;
			this.identifierSpace = identifierSpace;
		}

		/// <summary>
		/// Construct a unique identifier for an entity class instace
		/// </summary>
		/// <param name="id"></param>
		/// <param name="p"></param>
		public Key(object id, IClassPersister p) : this ( id, p.IdentifierSpace ) { }

		/// <summary>
		/// The user-visible identifier
		/// </summary>
		public object Identifier 
		{
			get { return id; }
		}

		public override bool Equals(object other) 
		{
			Key otherKey = other as Key;
			if(otherKey==null) 
			{
				return false;
			}
			return otherKey.identifierSpace.Equals(this.identifierSpace) && otherKey.id.Equals(this.id);
		}

		public override int GetHashCode() 
		{
			return id.GetHashCode();
		}

		public override string ToString() 
		{ 
			return id.ToString(); 
		} 

	}
}
