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
		/// Used as a placeholder when parsing HQL or SQL queries.
		/// </summary>
		public static readonly Parameter Placeholder = new Parameter();

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
			// All parameters are equal
			return obj == this || obj is Parameter;
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
	}
}
