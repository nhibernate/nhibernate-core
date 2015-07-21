using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2202
{
	public class Employee
	{
		private ISet<EmployeeAddress> _addresses;

		public virtual int NationalId { get; set; }
		public virtual int EmployeeId { get; set; }

		public virtual ISet<EmployeeAddress> Addresses
		{
			get
			{
				return _addresses ?? (_addresses = new HashSet<EmployeeAddress>());
			}
			set 
			{
				_addresses = value; 
			}
		}

		public virtual bool Equals(Employee other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.NationalId == NationalId && other.EmployeeId == EmployeeId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Employee)) return false;
			return Equals((Employee) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (NationalId*397) ^ EmployeeId;
			}
		}
	}

	public class EmployeeAddress
	{
		public virtual Employee Employee { get; set; }
		public virtual string Type { get; set; }

		public virtual bool Equals(EmployeeAddress other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Employee, Employee) && Equals(other.Type, Type);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (EmployeeAddress)) return false;
			return Equals((EmployeeAddress) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Employee != null ? Employee.GetHashCode() : 0)*397) ^ (Type != null ? Type.GetHashCode() : 0);
			}
		}
	}
}