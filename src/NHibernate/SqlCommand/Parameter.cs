using System;
using System.Data;

using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;


namespace NHibernate.SqlCommand
{
	/// <summary>
	/// A lightweight object to hold what later will be converted into an IDbParameter
	/// for an IDbCommand.
	/// </summary>
	public class Parameter: ICloneable
	{
		private DbType dbType;
		
		private string tableAlias;
		private string name;

		public System.Data.DbType DbType {
			get	{return dbType;}
			set {this.dbType = value;}
		}

		public string Name {
			get{ return name;}
			set{ this.name = value;}
		}


		public string TableAlias {
			get {return tableAlias;}
			set {this.tableAlias = value;}
		}

		/// <summary>
		/// Returns a string version of the Parameter that is in the correct
		/// format for the SQL in the CommandText.
		/// </summary>
		/// <param name="driver">The Driver that knows how to format the name.</param>
		/// <param name="name">The name to format for SQL.</param>
		/// <returns>A valid SQL string for this Parameter.</returns>
		public string GetSqlName(IDriver driver, string name) 
		{
			return driver.FormatNameForSql(name);
		}

		/// <summary>
		/// Returns a string version of the Parameter that is in the correct
		/// format for the IDbDataParameter.Name
		/// </summary>
		/// <param name="driver">The Driver that knows how to format the name.</param>
		/// <param name="name">The name to format for the IDbDataParameter.</param>
		/// <returns>A valid IDbDataParameter Name for this  Parameter.</returns>
		public string GetParameterName(IDriver driver, string name) 
		{
			return driver.FormatNameForParameter(name);
		}

		public virtual IDbDataParameter GetIDbDataParameter(IDbCommand command, IDriver driver, string name) 
		{
			IDbDataParameter param = command.CreateParameter();
			param.DbType = dbType;
			param.ParameterName = GetParameterName(driver, name);

			return param;
		}

		/// <summary>
		/// Generates an Array of Parameters for the columns that make up the IType
		/// </summary>
		/// <param name="columnNames">The names of the Columns that compose the IType</param>
		/// <param name="type">The IType to turn into Parameters</param>
		/// <returns>An Array of IParameter objects</returns>
		public static Parameter[] GenerateParameters(ISessionFactoryImplementor factory, string[] columnNames, IType type) {
			return Parameter.GenerateParameters(factory, null, columnNames, type);
		}


		/// <summary>
		/// Generates an Array of Parameters for the columns that make up the IType
		/// </summary>
		/// <param name="factory">The SessionFactory to use to get the DbTypes.</param>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="columnNames">The names of the Columns that compose the IType</param>
		/// <param name="type">The IType to turn into Parameters</param>
		/// <returns>An Array of IParameter objects</returns>
		public static Parameter[] GenerateParameters(ISessionFactoryImplementor factory, string tableAlias, string[] columnNames, IType type) {
			SqlType[] sqlTypes = type.SqlTypes(factory);

			Parameter[] parameters = new Parameter[sqlTypes.Length];

			for(int i = 0; i < sqlTypes.Length; i++) {
				if(sqlTypes[i].LengthDefined) {
					ParameterLength param = new ParameterLength();
					param.Length = sqlTypes[i].Length;
					parameters[i] = param;
				}
				else if(sqlTypes[i].PrecisionDefined) {
					ParameterPrecisionScale param = new ParameterPrecisionScale();
					param.Precision = sqlTypes[i].Precision;
					param.Scale = sqlTypes[i].Scale;
					parameters[i] = param;
				}
				else {
					parameters[i] = new Parameter();
				}

				parameters[i].Name = columnNames[i];
				parameters[i].DbType = sqlTypes[i].DbType;
				parameters[i].TableAlias = tableAlias;
			}


			return parameters;
		}

		#region object Members
		
		public override bool Equals(object obj) 
		{
			Parameter rhs;
			
			// Step1: Perform an equals test
			if(obj==this) return true;

			// Step	2: Instance of check
			rhs = obj as Parameter;
			if(rhs==null) return false;

			//Step 3: Check each important field
			
			// these 2 fields will not be null so compare them...
			if(this.DbType.Equals(rhs.DbType)==false || this.Name.Equals(rhs.Name)==false) return false;

			// becareful with TableAlias being null
			if(this.TableAlias==null && rhs.TableAlias==null) 
			{
				return true;
			}
			else if (this.TableAlias==null && rhs.TableAlias!=null) 
			{
				return false;
			}
			else 
			{
				return this.TableAlias.Equals(rhs.TableAlias);
			}
		}

		public override int GetHashCode()
		{
			int hashCode;

			unchecked 
			{
				hashCode = dbType.GetHashCode() + name.GetHashCode();
				if(tableAlias!=null) 
				{
					hashCode += tableAlias.GetHashCode();
				}

				return hashCode;
			}
		}

		public override string ToString() 
		{
			return (tableAlias==null || tableAlias==String.Empty)? 
				":" + name : 
				":" + tableAlias + "." + name;
		}

		#endregion

		#region ICloneable Members

		public Parameter Clone() 
		{
			
			Parameter paramClone = (Parameter)this.MemberwiseClone(); 

			return paramClone;
		}

		object ICloneable.Clone() {
			return Clone();
		}

		#endregion
	}

}
