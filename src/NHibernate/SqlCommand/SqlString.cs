using System;
using System.Text;

namespace NHibernate.SqlCommand 
{

	/// <summary>
	/// This is a non-modifiable Sql statement that is ready to be prepared 
	/// and sent to the Database for execution.
	/// 
	/// If you need to modify this object pass it to a <c>SqlStringBuilder</c> and
	/// get a new object back from it.
	/// </summary>
	[Serializable]
	public class SqlString : ICloneable 
	{
		readonly object[] sqlParts;
		
		public SqlString(string sqlPart) : this(new object[] {sqlPart}) 
		{
		}

		public SqlString(object[] sqlParts)	
		{
			this.sqlParts = sqlParts;
		}

		public object[] SqlParts 
		{
			get { return sqlParts;}
		}

		/// <summary>
		/// Appends the SqlString parameter to the end of the current SqlString to create a 
		/// new SqlString object.
		/// </summary>
		/// <param name="rhs">The SqlString to append.</param>
		/// <returns>A new SqlString object.</returns>
		/// <remarks>
		/// A SqlString object is immutable so this returns a new SqlString.  If multiple Appends 
		/// are called it is better to use the SqlStringBuilder.
		/// </remarks>
		public SqlString Append(SqlString rhs)
		{
		    object[] temp = new object[rhs.SqlParts.Length + sqlParts.Length];
			Array.Copy(sqlParts, 0, temp, 0, sqlParts.Length);
			Array.Copy(rhs.SqlParts, 0, temp, sqlParts.Length, rhs.SqlParts.Length);

			return new SqlString(temp);
		}

		#region System.Object Members
		
		
		public override bool Equals(object obj)
		{
			SqlString rhs;
			
			// Step1: Perform an equals test
			if(obj==this) return true;

			// Step	2: Instance of check
			rhs = obj as SqlString;
			if(rhs==null) return false;

			//Step 3: Check each important field
			for(int i = 0; i < sqlParts.Length; i++) 
			{
				if( this.sqlParts[i].Equals(rhs.SqlParts[i]) == false ) return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;

			unchecked 
			{
				for(int i = 0; i < sqlParts.Length; i++) 
				{
					hashCode += sqlParts[i].GetHashCode();
				}
			}

			return hashCode;
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
		public override string ToString() 
		{
			StringBuilder builder = new StringBuilder(sqlParts.Length * 15);

			for(int i = 0; i < sqlParts.Length; i++) 
			{
				builder.Append(sqlParts[i].ToString());
			}

			return builder.ToString();
		}
		
		#endregion
		
		#region ICloneable Members

		public SqlString Clone() 
		{
			object[] clonedParts = new object[sqlParts.Length];
			Parameter param;

			for(int i = 0; i < sqlParts.Length; i++) 
			{				
				param = sqlParts[i] as Parameter;
				if(param!=null) 
				{
					clonedParts[i] = param.Clone();
				}
				else 
				{
					clonedParts[i] = (string)sqlParts[i];
				}
			}

			return new SqlString(clonedParts);
		}

		object ICloneable.Clone() 
		{
			return Clone();
		}

		#endregion
	}
}
