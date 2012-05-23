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
		private Parameter() {}

		/// <summary>
		/// We need to know what the position of the parameter was in a query
		/// before we rearranged the query.
		/// This is the ADO parameter position that this SqlString parameter is
		/// bound to.  The SqlString can be safely rearranged once this is set.
		/// </summary>
		private int? parameterPosition;

		/// <summary>
		/// Used to determine the parameter's name (p0,p1 etc.)
		/// </summary>
		public int? ParameterPosition
		{
			get { return parameterPosition; }
			set { parameterPosition = value; }
		}

		/// <summary>
		/// Unique identifier of a parameter to be tracked back by its generator.
		/// </summary>
		/// <remarks>
		/// We have various query-systems. Each one, at the end, give us a <see cref="SqlString"/>.
		/// At the same time we have various bad-guys playing the game (hql function implementations, the dialect...).
		/// A bad guy can rearrange a <see cref="SqlString"/> and the query-system can easly lost organization/sequence of parameters.
		/// Using the <see cref="BackTrack"/> the query-system can easily find where are its parameters.
		/// </remarks>
		public object BackTrack { get; set; }

		/// <summary>
		/// Used as a placeholder when parsing HQL or SQL queries.
		/// </summary>
		public static Parameter Placeholder
		{
			get { return new Parameter(); }
		}

		/// <summary>
		/// Create a parameter with the specified position
		/// </summary>
		public static Parameter WithIndex(int position)
		{
			return new Parameter {ParameterPosition = position};
		}

		public Parameter Clone()
		{
			// Note: don't clone parameterPosition
			return new Parameter {BackTrack = BackTrack};
		}

		/// <summary>
		/// Generates an array of parameters.
		/// </summary>
		/// <param name="count">The number of parameters to generate.</param>
		/// <returns>An array of <see cref="Parameter"/> objects</returns>
		public static Parameter[] GenerateParameters(int count)
		{
			var result = new Parameter[count];
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
		/// An <see cref="int"/> value for the hash code.
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