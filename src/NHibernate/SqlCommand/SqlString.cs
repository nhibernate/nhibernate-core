using System;
using System.Text;



namespace NHibernate.SqlCommand {

	/// <summary>
	/// This is a non-modifiable Sql statement that is ready to be prepared 
	/// and sent to the Database for execution.
	/// 
	/// If you need to modify this object pass it to a <c>SqlStringBuilder</c> and
	/// get a new object back from it.
	/// </summary>
	public class SqlString : ICloneable {

		object[] sqlParts;
		
		public SqlString(string sqlPart) : this(new object[] {sqlPart}) {
		}

		public SqlString(object[] sqlParts)	{
			this.sqlParts = sqlParts;
		}

		public object[] SqlParts {
			get { return sqlParts;}
		}

		// this method treats this as immutable
		public SqlString Append(SqlString sqlString)
		{
		    object[] temp = new object[sqlString.SqlParts.Length + sqlParts.Length];
			Array.Copy(sqlParts, 0, temp, 0, sqlParts.Length);
			Array.Copy(sqlString.SqlParts, 0, temp, sqlParts.Length, sqlString.SqlParts.Length);

			return new SqlString(temp);
		}

		/// <summary>
		/// Returns the SqlString in a string where it looks like
		/// SELECT col1, col2 FROM table WHERE col1 = :param1
		/// 
		/// The ":" is used as the indicator of a parameter because at this point
		/// we are not using the specific Provider so we don't know how that provider
		/// wants our parameters formatted.
		/// </summary>
		/// <returns>A Provider nuetral version of the CommandText</returns>
		public override string ToString() {
			StringBuilder builder = new StringBuilder(sqlParts.Length * 15);
			foreach(object part in sqlParts) {
				builder.Append(part.ToString());
			}

			return builder.ToString();
		}
		
		
		#region ICloneable Members

		public SqlString Clone() {
			object[] clonedParts = new object[sqlParts.Length];
			Parameter param;

			for(int i = 0; i < sqlParts.Length; i++) {
				
				param = sqlParts[i] as Parameter;
				if(param!=null) {
					clonedParts[i] = param.Clone();
				}
				else {
					clonedParts[i] = (string)sqlParts[i];
				}
			}

			return new SqlString(clonedParts);
		}

		object ICloneable.Clone() {
			return Clone();
		}

		#endregion
	}
}
