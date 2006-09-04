using System;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// An immutable Parameter that later will be converted into an IDbParameter
	/// for an IDbCommand.
	/// </summary>
	[Serializable]
	public class Parameter : ICloneable
	{
		/// <summary>
		/// A parameter with <c>null</c> <see cref="SqlType" />. Used as a placeholder when
		/// parsing HQL or SQL queries.
		/// </summary>
		public static readonly Parameter Placeholder = new Parameter(null);

		//private readonly string _name;
		private readonly SqlType _sqlType;

		///// <summary>
		///// Initializes a new instance of <see cref="Parameter"/> class.
		///// </summary>
		///// <param name="name">The name of the parameter.</param>
		//public Parameter(string name)
		//    : this(name, null, null)
		//{
		//}

		/// <summary>
		/// Initializes a new instance of <see cref="Parameter"/> class.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="sqlType">The <see cref="SqlType"/> to create the parameter for.</param>
		public Parameter(SqlType sqlType)
		{
			_sqlType = sqlType;
		}

		///// <summary>
		///// Initializes a new instance of <see cref="Parameter"/> class.
		///// </summary>
		///// <param name="name">The name of the parameter.</param>
		///// <param name="tableAlias">The Alias to use for the table.</param>
		///// <param name="sqlType">The <see cref="SqlType"/> to create the parameter for.</param>
		//public Parameter(string name, string tableAlias, SqlType sqlType)
		//{
		//    if (tableAlias != null && tableAlias.Length > 0)
		//    {
		//        _name = tableAlias + StringHelper.Dot + name;
		//    }
		//    else
		//    {
		//        _name = name;
		//    }
		//    _sqlType = sqlType;
		//}

		///// <summary>
		///// Gets the name of the Parameter.
		///// </summary>
		///// <value>The name of the Parameter.</value>
		///// <remarks>
		///// The Parameter name is not used by anything except to compare equality
		///// and to generate a debug string.
		///// </remarks>
		//public string Name
		//{
		//    get { return _name; }
		//}

		/// <summary>
		/// Gets the <see cref="SqlType"/> that defines the specifics of 
		/// the IDbDataParameter.
		/// </summary>
		/// <value>
		/// The <see cref="SqlType"/> that defines the specifics of 
		/// the IDbDataParameter.
		/// </value>
		public SqlType SqlType
		{
			get { return _sqlType; }
		}

		/// <summary>
		/// Generates an array of parameters for the columns that make up the type.
		/// </summary>
		/// <param name="columnNames">The names of the columns that compose the type</param>
		/// <param name="type">The type to turn into parameters</param>
		/// <returns>An array of <see cref="Parameter"/> objects</returns>
		public static Parameter[] GenerateParameters(IMapping mapping, IType type)
		{
			return Parameter.GenerateParameters(type.SqlTypes(mapping));
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
				//if (_name != null)
				//{
				//    hashCode += _name.GetHashCode();
				//}

				return hashCode;
			}
		}

		public override string ToString()
		{
			return "?";
			//":" + _name;
		}

		#endregion

		#region ICloneable Members

		public Parameter Clone()
		{
			Parameter paramClone = (Parameter) this.MemberwiseClone();

			return paramClone;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion
	}
}