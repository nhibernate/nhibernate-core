using System;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Extension to the Parameter class that supports Parameters with
	/// a Length.
	/// </summary>
	/// <remarks>
	/// This should only be used when the property needs to be mapped with
	/// a <c>type="String(200)"</c> because for some reason the default parameter
	/// generation of <c>nvarchar(4000)</c> (MsSql specific) is not good enough.
	/// </remarks>
	[Serializable]
	public class ParameterLength : Parameter
	{
		private int length;

		/// <summary></summary>
		public int Length
		{
			get { return length; }
			set { length = value; }
		}

		#region System.Object Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals( object obj )
		{
			if( base.Equals( obj ) )
			{
				ParameterLength rhs;

				// Step	2: Instance of check
				rhs = obj as ParameterLength;
				if( rhs == null )
				{
					return false;
				}

				//Step 3: Check each important field
				return this.Length.Equals( rhs.Length );
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
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