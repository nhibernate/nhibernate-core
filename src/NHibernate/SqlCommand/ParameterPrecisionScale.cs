using System;
using System.Data;

using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.SqlCommand 
{	
	/// <summary>
	/// Extension to the Parameter class that supports Parameters with
	/// a Precision and a Scale
	/// </summary>
	public class ParameterPrecisionScale : Parameter 
	{
		private byte precision;
		private byte scale;

		public byte Precision 
		{
			get {return precision;}
			set {precision = value;}
		}

		public byte Scale 
		{
			get {return scale;}
			set {scale = value;}
		}

		public override IDbDataParameter GetIDbDataParameter(IDbCommand command, IDriver driver, string name) 
		{
			IDbDataParameter param = base.GetIDbDataParameter (command, driver, name);
			param.Precision = precision;
			param.Scale = scale;

			return param;
		}

		#region System.Object Members
		
		public override bool Equals(object obj) 
		{
			if(base.Equals(obj)) 
			{
				ParameterPrecisionScale rhs;
			
				// Step	2: Instance of check
				rhs = obj as ParameterPrecisionScale;
				if(rhs==null) return false;

				//Step 3: Check each important field
				return this.Precision==rhs.Precision
					&& this.Scale==rhs.Scale;
			}
			else 
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			unchecked 
			{
				return base.GetHashCode() + precision.GetHashCode() + scale.GetHashCode();
			}
		}

		#endregion

	}
}

