using System;

namespace NHibernate.SqlCommand 
{	
	/// <summary>
	/// Extension to the Parameter class that supports Parameters with
	/// a Precision and a Scale
	/// </summary>
	/// <remarks>
	/// This should only be used when the property needs to be mapped with
	/// a <c>type="Decimal(20,4)"</c> because for some reason the default parameter
	/// generation of <c>decimal(19,5)</c> (MsSql specific) is not good enough.
	/// </remarks>
	[Serializable]
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

