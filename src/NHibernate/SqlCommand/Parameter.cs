using System;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A placeholder for an ADO.NET parameter in an <see cref="SqlString" />.
	/// </summary>
	[Serializable]
	public class Parameter
	{
        /// <summary>
        /// We need to know what the position of the parameter was in a query
        /// before we rearranged the query.
        /// This is used only by dialects that rearrange the query, unfortunately, 
        /// the MS SQL 2005 dialect have to re shuffle the query (and ruin positional parameter
        /// support) because the SQL 2005 and 2008 SQL dialects have a completely broken
        /// support for paging, which is just a tad less important than SELECT.
        /// See  	 NH-1528
        /// </summary>
	    public int? OriginalPositionInQuery;

	    /// <summary>
	    /// Used as a placeholder when parsing HQL or SQL queries.
	    /// </summary>
	    public static Parameter Placeholder
	    {
            get { return new Parameter(); }
	    }

		private Parameter()
		{
		}

		/// <summary>
		/// Generates an array of parameters for the given <see cref="SqlType">SqlTypes</see>.
		/// </summary>
		/// <param name="count">The number of parameters to generate.</param>
		/// <returns>An array of <see cref="Parameter"/> objects</returns>
		public static Parameter[] GenerateParameters(int count)
		{
			Parameter[] result = new Parameter[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = Placeholder;
			}
			return result;
		}

		/// <summary>
		/// Determines whether this instance and the specified object 
		/// are of the same type and have the same values.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns>
		/// <see langword="true" /> if the object equals the current instance.
		/// </returns>
		public override bool Equals(object obj)
		{
			// All parameters are equal, this check that
            // the other one is not null and a parameter
			return obj is Parameter;
		}

		/// <summary>
		/// Gets a hash code for the parameter.
		/// </summary>
		/// <returns>
		/// An <see cref="Int32"/> value for the hash code.
		/// </returns>
		public override int GetHashCode()
		{
			// Just an arbitrary value.
			return 1337;
		}

		public override string ToString()
		{
			return StringHelper.SqlParameter;
		}

        public static bool operator ==(Parameter a, Parameter b)
        {
            return Equals(a, b);
        }

        public static bool operator ==(object a, Parameter b)
        {
            return Equals(a, b);
        }

        public static bool operator ==(Parameter a, object b)
        {
            return Equals(a, b);
        }

	    public static bool operator !=(Parameter a, object b)
	    {
	        return !(a == b);
	    }

	    public static bool operator !=(object a, Parameter b)
	    {
	        return !(a == b);
	    }

	    public static bool operator !=(Parameter a, Parameter b)
	    {
	        return !(a == b);
	    }
	}
}
