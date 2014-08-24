using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH2324
{
	public class CompositeUserType : ICompositeUserType
	{
		#region ICompositeUserType Members

		/// <summary>
		/// Compare two instances of the class mapped by this type for persistence
		/// "equality", ie. equality of persistent state.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public new bool Equals(object x, object y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}
			else if (x == null || y == null)
			{
				return false;
			}
			else
			{
				return x.Equals(y);
			}
		}

		/// <summary>
		/// Get the "property names" that may be used in a query.
		/// </summary>
		/// <value></value>
		public string[] PropertyNames
		{
			get
			{
				return new[] { "DataA", "DataB" };
			}
		}

		/// <summary>
		/// Get the corresponding "property types"
		/// </summary>
		/// <value></value>
		public IType[] PropertyTypes
		{
			get
			{
				return new IType[]
							{
								NHibernateUtil.DateTime,
								NHibernateUtil.DateTime
							};
			}
		}

		/// <summary>
		/// Get the value of a property
		/// </summary>
		/// <param name="component">an instance of class mapped by this "type"</param>
		/// <param name="property"></param>
		/// <returns>the property value</returns>
		public object GetPropertyValue(object component, int property)
		{
			var p = (CompositeData) component;
			if (property == 0)
			{
				return p.DataA;
			}
			else
			{
				return p.DataB;
			}
		}

		/// <summary>
		/// Set the value of a property
		/// </summary>
		/// <param name="component">an instance of class mapped by this "type"</param>
		/// <param name="property"></param>
		/// <param name="value">the value to set</param>
		public void SetPropertyValue(object component, int property, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// The class returned by NullSafeGet().
		/// </summary>
		/// <value></value>
		public System.Type ReturnedClass
		{
			get { return typeof (CompositeData); }
		}

		/// <summary>
		/// Get a hashcode for the instance, consistent with persistence "equality"
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		/// <summary>
		/// Retrieve an instance of the mapped class from a IDataReader. Implementors
		/// should handle possibility of null values.
		/// </summary>
		/// <param name="dr">IDataReader</param>
		/// <param name="names">the column names</param>
		/// <param name="session"></param>
		/// <param name="owner">the containing entity</param>
		/// <returns></returns>
		public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
		{
			var data = new CompositeData();
			data.DataA = (DateTime) NHibernateUtil.DateTime.NullSafeGet(dr, new[] {names[0]}, session, owner);
			data.DataB = (DateTime) NHibernateUtil.DateTime.NullSafeGet(dr, new[] {names[1]}, session, owner);

			return data;
		}

		/// <summary>
		/// Write an instance of the mapped class to a prepared statement.
		/// Implementors should handle possibility of null values.
		/// A multi-column type should be written to parameters starting from index.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="settable"></param>
		/// <param name="session"></param>
		public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (value == null)
			{
				if (settable[0]) ((IDataParameter) cmd.Parameters[index++]).Value = DBNull.Value;
				if (settable[1]) ((IDataParameter) cmd.Parameters[index]).Value = DBNull.Value;
			}
			else
			{
				var data = (CompositeData) value;
				if (settable[0]) NHibernateUtil.DateTime.Set(cmd, data.DataA, index++);
				if (settable[1]) NHibernateUtil.DateTime.Set(cmd, data.DataB, index);
			}
		}

		/// <summary>
		/// Return a deep copy of the persistent state, stopping at entities and at collections.
		/// </summary>
		/// <param name="value">generally a collection element or entity field</param>
		/// <returns></returns>
		public object DeepCopy(object value)
		{
			return value;
		}

		/// <summary>
		/// Are objects of this type mutable?
		/// </summary>
		/// <value></value>
		public bool IsMutable
		{
			get { return true; }
		}

		/// <summary>
		/// Transform the object into its cacheable representation.
		/// At the very least this method should perform a deep copy.
		/// That may not be enough for some implementations, method should perform a deep copy. That may not be enough for some implementations, however; for example, associations must be cached as identifier values. (optional operation)
		/// </summary>
		/// <param name="value">the object to be cached</param>
		/// <param name="session"></param>
		/// <returns></returns>
		public object Disassemble(object value, ISessionImplementor session)
		{
			return DeepCopy(value);
		}

		/// <summary>
		/// Reconstruct an object from the cacheable representation.
		/// At the very least this method should perform a deep copy. (optional operation)
		/// </summary>
		/// <param name="cached">the object to be cached</param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			return DeepCopy(cached);
		}

		/// <summary>
		/// During merge, replace the existing (target) value in the entity we are merging to
		/// with a new (original) value from the detached entity we are merging. For immutable
		/// objects, or null values, it is safe to simply return the first parameter. For
		/// mutable objects, it is safe to return a copy of the first parameter. However, since
		/// composite user types often define component values, it might make sense to recursively
		/// replace component values in the target object.
		/// </summary>
		/// <param name="original"></param>
		/// <param name="target"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			return original;
		}

		#endregion
	}
}