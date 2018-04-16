using System;
using NHibernate.Proxy;

namespace NHibernate.Test.NHSpecificTest.NH1789
{
	///<summary>
	/// The base for all objects in the model
	///</summary>
	[Serializable]
	public abstract partial class DomainObject : IDomainObject
	{
		protected long _id;

		#region IDomainObject Members

		/// <summary>
		/// Database ID. This ID doesn't have any business meaning.
		/// </summary>
		public virtual long ID
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// Transient objects are not associated with an 
		/// item already in storage.  For instance, a 
		/// Customer is transient if its ID is 0.
		/// </summary>
		public virtual bool IsTransient()
		{
			return ID.Equals(0);
		}

		///<summary>
		/// This is a string which uniquely identifies an instance in the case that the ids 
		/// are both transient
		///</summary>
		public abstract string BusinessKey { get; }

		/// <summary>
		/// Returns the concrete type of the object, not the proxy one.
		/// </summary>
		/// <returns></returns>
		public virtual System.Type GetConcreteType()
		{
			return NHibernateProxyHelper.GuessClass(this);
		}

		#endregion

		///<summary>
		/// The equals method works with the persistence framework too
		///</summary>
		///<param name="obj"></param>
		///<returns></returns>
		public override bool Equals(object obj)
		{
			Console.Out.WriteLine("Calling Equals");

			var compareTo = obj as IDomainObject;

			if (compareTo == null)
			{
				return false;
			}

			if (compareTo.GetConcreteType() != GetConcreteType())
			{
				return false;
			}

			if (BothNonDefaultIds(compareTo))
			{
				return ID.CompareTo(compareTo.ID).Equals(0);
			}

			if ((IsTransient() || compareTo.IsTransient()) && HasSameBusinessSignatureAs(compareTo))
			{
				return true;
			}

			return false;
		}

		private bool HasSameBusinessSignatureAs(IDomainObject compareTo)
		{
			return BusinessKey.Equals(compareTo.BusinessKey);
		}

		private bool BothNonDefaultIds(IDomainObject compareTo)
		{
			return !ID.Equals(0) && !compareTo.ID.Equals(0);
		}

		/// <summary>
		/// Must be implemented to compare two objects
		/// </summary>
		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		/// <summary>
		/// Turn a proxy object into a "real" object. If the <paramref name="proxy"/> you give in parameter is not a INHibernateProxy, it will returns the same object without any change.
		/// </summary>
		/// <typeparam name="T">Type in which the unproxied object should be returned</typeparam>
		/// <param name="proxy">Proxy object</param>
		/// <returns>Unproxied object</returns>
		public static T UnProxy<T>(object proxy)
		{
			//If the object is not a proxy, just cast it and returns it
			if (!(proxy is INHibernateProxy))
			{
				return (T) proxy;
			}

			//Otherwise, use the NHibernate methods to get the implementation, and cast it
			var p = (INHibernateProxy) proxy;
			return (T) p.HibernateLazyInitializer.GetImplementation();
		}

		/// <summary>
		/// Turn a proxy object into a "real" object. If the <paramref name="proxy"/> you give in parameter is not a INHibernateProxy, it will returns the same object without any change.
		/// </summary>
		/// <param name="proxy">Proxy object</param>
		/// <returns>Unproxied object</returns>
		public static object UnProxy(object proxy)
		{
			return UnProxy<object>(proxy);
		}
	}
}