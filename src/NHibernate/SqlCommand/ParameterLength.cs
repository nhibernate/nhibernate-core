using System;
using System.Data;

using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.SqlCommand 
{	
	/// <summary>
	/// Extension to the Parameter class that supports Parameters with
	/// a Length
	/// </summary>
	public class ParameterLength : Parameter
	{
		private int length;

		public int Length
		{
			get {return length;}
			set {length = value;}
		}

		public override IDbDataParameter GetIDbDataParameter(IDbCommand command, IDriver driver, string name) 
		{
			IDbDataParameter param = base.GetIDbDataParameter (command, driver, name);
			param.Size = length;

			return param;
		}
		
		#region System.Object Members
		
		public override bool Equals(object obj) 
		{
			if(base.Equals(obj)) 
			{
				ParameterLength rhs;
			
				// Step	2: Instance of check
				rhs = obj as ParameterLength;
				if(rhs==null) return false;

				//Step 3: Check each important field
				return this.Length.Equals(rhs.Length);
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
				return base.GetHashCode() + length.GetHashCode();
			}
		}

		#endregion

	}
}
