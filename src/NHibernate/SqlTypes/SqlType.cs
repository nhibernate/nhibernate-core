using System;
using System.Data;

namespace NHibernate.SqlTypes 
{	
	/// <summary>
	/// This is the base class that adds information to the <see cref="DbType" /> 
	/// for the <see cref="Driver.IDriver"/> and <see cref="Dialect.Dialect"/>
	/// to use.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The <see cref="Driver.IDriver"/> uses the SqlType to get enough
	/// information to create an <see cref="IDbDataParameter"/>.  
	/// </p>
	/// <p>
	/// The <see cref="Dialect.Dialect"/> use the SqlType to convert the <see cref="DbType"/>
	/// to the appropriate sql type for SchemaExport.
	/// </p>
	/// </remarks>
	[Serializable]
	public abstract class SqlType
	{
		private DbType _dbType;
		private int _length;
		private byte _precision;
		private byte _scale;

		private bool _lengthDefined = false;
		private bool _precisionDefined = false;
		
		protected SqlType(DbType dbType)
		{
			_dbType = dbType;
		}

		protected SqlType(DbType dbType, int length)
		{
			_dbType = dbType;
			_length = length;
			_lengthDefined = true;
		}

		protected SqlType(DbType dbType, byte precision, byte scale) 
		{
			_dbType = dbType;
			_precision = precision;
			_scale = scale;
			_precisionDefined = true;
		}

		public DbType DbType 
		{
			get { return _dbType;}
		}

		public int Length 
		{
			get { return _length;}
		}

		public byte Precision 
		{
			get { return _precision;}
		}

		public byte Scale 
		{
			get { return _scale;}
		}

		public bool LengthDefined 
		{
			get { return _lengthDefined;}
		}

		public bool PrecisionDefined 
		{
			get { return _precisionDefined;}
		}

		#region System.Object Members

		public override int GetHashCode() 
		{
			int hashCode = 0;

			if( LengthDefined ) 
			{
				hashCode = ( DbType.GetHashCode()/2 ) + ( Length.GetHashCode()/2 );
			}
			else if( PrecisionDefined) 
			{
				hashCode = ( DbType.GetHashCode()/3 ) + ( Precision.GetHashCode()/3 ) + ( Scale.GetHashCode()/3 );
			}
			else 
			{
				hashCode = DbType.GetHashCode();
			}

			return hashCode;
		}

		public override bool Equals(object obj) 
		{
			bool equals = false;
			SqlType rhsSqlType;
			
			// Step1: Perform an equals test
			if( obj==this ) return true;

			// Step	2: Instance of check
			rhsSqlType = obj as SqlType;
			if( rhsSqlType==null ) return false;

			//Step 3: Check each important field

			if( LengthDefined )
			{
				equals = ( DbType.Equals( rhsSqlType.DbType ) ) 
					&& ( Length==rhsSqlType.Length );
			}
			else if( PrecisionDefined ) 
			{
				equals = ( DbType.Equals( rhsSqlType.DbType ) )
					&& ( Precision==rhsSqlType.Precision )
					&& ( Scale==rhsSqlType.Scale );
			}
			else 
			{
				equals = ( DbType.Equals( rhsSqlType.DbType ) );
			}

			return equals;

		}

		#endregion

	}
}
