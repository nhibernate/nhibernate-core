using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace NHibernate
{
	[Serializable]
	public class IdentityEqualityComparer: IEqualityComparer
	{
		#region IEqualityComparer Members

		public int GetHashCode(object obj)
		{
			return RuntimeHelpers.GetHashCode(obj);
		}

		/// <summary>
		/// Performs a null safe comparison using "==" instead of Object.Equals()
		/// </summary>
		/// <param name="x">First object to compare.</param>
		/// <param name="y">Second object to compare.</param>
		/// <returns>
		/// true if x is the same instance as y or if both are null references; otherwise, false.
		///</returns>
		/// <remarks>
		/// This is Lazy collection safe since it uses <see cref="Object.ReferenceEquals"/>, 
		/// unlike <c>Object.Equals()</c> which currently causes NHibernate to load up the collection.
		/// This behaivior of Collections is likely to change because Java's collections override Equals() and 
		/// .net's collections don't. So in .net there is no need to override Equals() and 
		/// GetHashCode() on the NHibernate Collection implementations.
		/// </remarks>
		public new bool Equals(object x, object y)
		{
			return ReferenceEquals(x, y);

		}
		#endregion
	}
}
