using System;
using System.Data;

using NHibernate.Connection;
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
		
		public string GetSqlName(IConnectionProvider provider){
			
			return provider.Driver.FormatNameForSql(tableAlias, name);
		}

		public string GetParameterName(IConnectionProvider provider){
			return provider.Driver.FormatNameForParameter(tableAlias, name);
		}

		public virtual IDbDataParameter GetIDbDataParameter(IDbCommand command, IConnectionProvider provider) {
			IDbDataParameter param = command.CreateParameter();
			param.DbType = dbType;
			param.ParameterName = GetParameterName(provider);

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

		public override bool Equals(object obj) {
			Parameter rhs;
			
			// Step1: Perform an equals test
			if(obj==this) return true;

			// Step	2: Instance of check
			rhs = obj as Parameter;
			if(rhs==null) return false;

			//Step 3: Check each important field
			return this.DbType.Equals(rhs.DbType) 
					&& this.Name.Equals(rhs.Name)
					&& this.TableAlias.Equals(rhs.TableAlias);
			
		}

		public override string ToString() {
			return (tableAlias==null || tableAlias==String.Empty)? 
				":" + name : 
				":" + tableAlias + "." + name;
		}

		
		#region ICloneable Members

		public Parameter Clone() {
			
			Parameter paramClone = (Parameter)this.MemberwiseClone(); 

			return paramClone;
		}

		object ICloneable.Clone() {
			return Clone();
		}

		#endregion
	}

}
