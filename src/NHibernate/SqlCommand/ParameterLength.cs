using System;
using System.Data;

using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.SqlCommand {
	
	/// <summary>
	/// Extension to the Parameter class that supports Parameters with
	/// a Length
	/// </summary>
	public class ParameterLength : Parameter {
		private int length;

		public int Length {
			get {return length;}
			set {length = value;}
		}

		public override IDbDataParameter GetIDbDataParameter(IDbCommand command, IConnectionProvider provider) {
			IDbDataParameter param = base.GetIDbDataParameter (command, provider);
			param.Size = length;

			return param;
		}

		public override bool Equals(object obj) {
			if(base.Equals(obj)) {
				ParameterLength rhs;
			
				// Step	2: Instance of check
				rhs = obj as ParameterLength;
				if(rhs==null) return false;

				//Step 3: Check each important field
				return this.Length.Equals(rhs.Length);
			}
			else {
				return false;
			}
		}

		// override to prevent compiler warning CS0659
		// TODO: this may need a different impl
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}


	}
}
