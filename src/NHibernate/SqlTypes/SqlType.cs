using System;
using System.Data;

namespace NHibernate.SqlTypes 
{	
	/// <summary>
	/// This is the base class that describes the DbType in further detail so we 
	/// can prepare the IDbCommands.  Certain SqlTypes have a default length or
	/// precision/scale that can get overriden in the hbm files.
	/// 
	/// It is expected that each Dialect will be the one responsible for converting these
	/// objects to the sql string when using SchemaExport.
	/// </summary>
	[Serializable]
	public abstract class SqlType
	{
		protected DbType dbType;
		protected int length;
		protected byte precision;
		protected byte scale;

		protected bool lengthDefined = false;
		protected bool precisionDefined = false;
		
		protected SqlType(DbType dbType)
		{
			this.dbType = dbType;
		}

		protected SqlType(DbType dbType, int length)
		{
			this.dbType = dbType;
			this.length = length;
			this.lengthDefined = true;
		}

		protected SqlType(DbType dbType, byte precision, byte scale) 
		{
			this.dbType = dbType;
			this.precision = precision;
			this.scale = scale;
			this.precisionDefined = true;
		}

		public DbType DbType 
		{
			get { return dbType;}
		}

		public int Length 
		{
			get { return length;}
		}

		public byte Precision 
		{
			get { return precision;}
		}

		public byte Scale 
		{
			get { return scale;}
		}

		public bool LengthDefined 
		{
			get { return lengthDefined;}
		}

		public bool PrecisionDefined 
		{
			get { return precisionDefined;}
		}

		#region overrides of Object methods

		public override int GetHashCode() 
		{
			int hashCode = 0;

			if(lengthDefined) 
			{
				hashCode = (dbType.GetHashCode()/2) + (length.GetHashCode()/2);
			}
			else if(precisionDefined) 
			{
				hashCode = (dbType.GetHashCode()/3) + (precision.GetHashCode()/3) + (scale.GetHashCode()/3);
			}
			else 
			{
				hashCode = dbType.GetHashCode();
			}

			return hashCode;
		}

		public override bool Equals(object obj) 
		{
			bool equals = false;
			SqlType rhsSqlType;
			
			// Step1: Perform an equals test
			if(obj==this) return true;

			// Step	2: Instance of check
			rhsSqlType = obj as SqlType;
			if(rhsSqlType==null) return false;

			//Step 3: Check each important field

			if(lengthDefined)
			{
				equals = (dbType.Equals(rhsSqlType.DbType)) 
					&& (length==rhsSqlType.Length);
			}
			else if (precisionDefined) 
			{
				equals = (dbType.Equals(rhsSqlType.DbType))
					&& (precision==rhsSqlType.Precision)
					&& (scale==rhsSqlType.Scale);
			}
			else 
			{
				equals = (dbType.Equals(rhsSqlType.DbType));
			}

			return equals;

		}

		#endregion

	}
}
