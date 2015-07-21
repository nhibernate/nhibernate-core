using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.Join
{
	public class EmployeeWithCompositeKey
	{
		private EmployeeWithCompositeKey() { }

		public EmployeeWithCompositeKey(int companyId, int empNumber)
		{
			this.Pk = new EmployeePk(companyId, empNumber);
		}

		private EmployeePk _Pk;
		public virtual EmployeePk Pk
		{
			get { return _Pk; }
			set { _Pk = value; }
		}

		private DateTime? _StartDate;
		public virtual DateTime? StartDate
		{
			get { return _StartDate; }
			set { _StartDate = value; }
		}

		private string _FirstName;
		public virtual string FirstName
		{
			get { return _FirstName; }
			set { _FirstName = value; }
		}

		private string _Surname;
		public virtual string Surname
		{
			get { return _Surname; }
			set { _Surname = value; }
		}

		private string _OtherNames;
		public virtual string OtherNames
		{
			get { return _OtherNames; }
			set { _OtherNames = value; }
		}

		private string _Title;
		public virtual string Title
		{
			get { return _Title; }
			set { _Title = value; }
		}

		public override bool Equals(object obj)
		{
			if (this.Pk == null) return false;
			EmployeeWithCompositeKey other = obj as EmployeeWithCompositeKey;

			if (other == null) return false;
			if (other.Pk == null) return false;

			return this.Pk.Equals(other.Pk);
		}

		public override int GetHashCode()
		{
			return this.Pk == null ? 0 : this.Pk.GetHashCode();
		}
	}

	public class EmployeePk
	{
		public EmployeePk() { }

		public EmployeePk(int companyId, int empNumber)
		{
			this._CompanyId = companyId;
			this._EmpNumber = empNumber;
		}

		private int _CompanyId;
		public virtual int CompanyId
		{
			get { return _CompanyId; }
			set { _CompanyId = value; }
		}

		private int _EmpNumber;
		public virtual int EmpNumber
		{
			get { return _EmpNumber; }
			set { _EmpNumber = value; }
		}

		public override bool Equals(object obj)
		{
			EmployeePk other = obj as EmployeePk;
			if (other == null) 
				return false;
			else
				return (this.CompanyId == other.CompanyId && string.Equals(this.EmpNumber, other.EmpNumber));
		}

		public override int GetHashCode()
		{
			return this.CompanyId.GetHashCode();
		}
	}
}
