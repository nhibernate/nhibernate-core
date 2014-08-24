using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public class Address : IEquatable<Address>
	{
		#region Fields

#pragma warning disable 169
		private int _id;
		private int _version;
#pragma warning restore 169

		private string _number;
		private string _postcode;

		#endregion

		#region Ctors

		public Address(string number, string postcode)
		{
			_number = number;
			_postcode = postcode;
		}

		private Address()
		{
		}

		#endregion

		#region Properties

		public string Number
		{
			get { return _number; }
		}

		public string Postcode
		{
			get { return _postcode; }
		}

		#endregion

		#region Object Overrides

		public override bool Equals(object obj)
		{
			return Equals(obj as Address);
		}

		public override int GetHashCode()
		{
			return Postcode.GetHashCode() + 29*Number.GetHashCode();
		}

		#endregion

		#region IEquatable<Address> Members

		public bool Equals(Address other)
		{
			return other == null ? false : IsEqualTo(other);
		}

		#endregion

		private bool IsEqualTo(Address other)
		{
			return ReferenceEquals(this, other) || (Postcode.Equals(other.Postcode) && Number.Equals(other.Number));
		}
	}
}