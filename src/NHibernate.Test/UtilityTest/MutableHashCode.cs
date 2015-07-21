using System;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Provides an object whose HashCode is based on a Mutable field.  Not a good
	/// practice but perfect for testing IdentityMap because it simulates an object
	/// being loaded with its default constructor (no params) and then having the 
	/// fields initialized.  If the class overrides GetHashCode() then it will be inconsistent
	/// between the construction and field population by NHibernate.
	/// </summary>
	[Serializable]
	public class MutableHashCode
	{
		private int hashCodeField;

		public MutableHashCode()
		{
		}

		public MutableHashCode(int hashCodeField)
		{
			this.hashCodeField = hashCodeField;
		}

		public int HashCodeField
		{
			get { return hashCodeField; }
			set { hashCodeField = value; }
		}

		public override int GetHashCode()
		{
			return hashCodeField.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			// I am not putting all of the proper comparisons in here
			// because this is just simple test code.
			return hashCodeField.Equals(((MutableHashCode) obj).HashCodeField);
		}
	}
}