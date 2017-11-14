using System;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public class Person : IEquatable<Person>
	{
		#region Fields

		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private int _id;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
		// Used by reflection
#pragma warning disable CS0169 // The field is never used
		private int _version;
#pragma warning restore CS0169 // The field is never used

		private string _name;
		private IMilestoneCollection<int, decimal> _feeMatrix = new MilestoneCollection<int, decimal>();
		private IMilestoneCollection<DateTime, Address> _historyOfAddresses = new MilestoneCollection<DateTime, Address>();

		#endregion

		#region Constructors

		private Person()
		{
		}

		public Person(string name)
		{
			_name = name;
		}

		#endregion

		#region Properties

		public int Id
		{
			get { return _id; }
		}
		
		public string Name
		{
			get { return _name; }
		}

		public Address CurrentAddress
		{
			get { return _historyOfAddresses.FindValueFor(DateTime.Now); }
		}

		#endregion

		#region Methods

		public void AddPercentageToFeeMatrix(int value, decimal percentage)
		{
			_feeMatrix[value] = percentage;
		}

		public decimal FindFeePercentageForValue(int value)
		{
			return _feeMatrix.FindValueFor(value);
		}

		public void RegisterChangeOfAddress(DateTime movingDate, Address newAddress)
		{
			_historyOfAddresses[movingDate] = newAddress;
		}

		#endregion

		#region Object Overrides

		public override bool Equals(object obj)
		{
			return Equals(obj as Person);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override string ToString()
		{
			return "Person: " + Name;
		}

		#endregion

		#region IEquatable<Person> Members

		public bool Equals(Person other)
		{
			return other == null ? false : IsEqualTo(other);
		}

		#endregion

		private bool IsEqualTo(Person other)
		{
			return ReferenceEquals(this, other) || Name.Equals(other.Name);
		}
	}
}