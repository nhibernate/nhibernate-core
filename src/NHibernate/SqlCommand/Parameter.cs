using System;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An immutable Parameter that later will be converted into an IDbParameter
	/// for an IDbCommand.
	/// </summary>
	[Serializable]
	public class Parameter
	{
		/// <summary>
		/// A parameter with <c>null</c> <see cref="SqlType" />. Used as a placeholder when
		/// parsing HQL or SQL queries.
		/// </summary>
		public static readonly Parameter Placeholder = new Parameter(null);

		private readonly SqlType _sqlType;

		/// <summary>
		/// Initializes a new instance of <see cref="Parameter"/> class.
		/// </summary>
		/// <param name="sqlType">The <see cref="SqlType"/> to create the parameter for.</param>
		public Parameter(SqlType sqlType)
		{
			_sqlType = sqlType;
		}

		public SqlType SqlType
		{
			get { return _sqlType; }
		}

		/// <summary>
		/// Generates an array of parameters for the columns that make up the type.
		/// </summary>
		/// <param name="type">The type to turn into parameters</param>
		/// <returns>An array of <see cref="Parameter"/> objects</returns>
		public static Parameter[] GenerateParameters(IMapping mapping, IType type)
		{
			return GenerateParameters(type.SqlTypes(mapping));
		}

		/// <summary>
		/// Generates an array of parameters for the given <see cref="SqlType">SqlTypes</see>.
		/// </summary>
		/// <param name="sqlTypes">Array of <see cref="SqlType" /> giving the database type and
		/// size of each generated parameter.</param>
		/// <returns>An array of <see cref="Parameter"/> objects</returns>
		public static Parameter[] GenerateParameters(SqlType[] sqlTypes)
		{
			Parameter[] parameters = new Parameter[sqlTypes.Length];
			for (int i = 0; i < sqlTypes.Length; i++)
			{
				parameters[i] = new Parameter(sqlTypes[i]);
			}
			return parameters;
		}

		#region Object Members

		/// <summary>
		/// Determines wether this instance and the specified object 
		/// are the same Type and have the same values.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns>
		/// <c>true</c> if the object is a Parameter (not a subclass) and has the
		/// same values.
		/// </returns>
		/// <remarks>
		/// Each subclass needs to implement their own <c>Equals</c>. 
		/// </remarks>
		public override bool Equals(object obj)
		{
			// Step1: Perform an equals test
			if (obj == this)
			{
				return true;
			}

			// Step	2: Instance of check
			Parameter rhs = obj as Parameter;
			if (rhs == null)
			{
				return false;
			}

			//Step 3: Check each important field
			return object.Equals(this.SqlType, rhs.SqlType); // && this.Name.Equals(rhs.Name);
		}

		/// <summary>
		/// Gets a hash code based on the SqlType, Name, and TableAlias properties.
		/// </summary>
		/// <returns>
		/// An <see cref="Int32"/> value for the hash code.
		/// </returns>
		public override int GetHashCode()
		{
			int hashCode = 0;

			unchecked
			{
				if (_sqlType != null)
				{
					hashCode += _sqlType.GetHashCode();
				}

				return hashCode;
			}
		}

		public override string ToString()
		{
			return "?";
		}

		#endregion
	}
}