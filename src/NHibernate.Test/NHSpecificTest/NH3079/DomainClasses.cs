

using System;
using System.Collections.Generic;
namespace NHibernate.Test.NHSpecificTest.NH3079
{
	public class PersonCpId
	{
		private Int32? idA;
		public Int32? IdA
		{
			get { return idA; }
			set { idA = value; }
		}

		private Int32? idB;
		public Int32? IdB
		{
			get { return idB; }
			set { idB = value; }
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			PersonCpId objCpId = (PersonCpId)obj;
			return this.IdA == objCpId.IdA && this.IdB == this.IdB;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return  this.IdA.Value + this.IdB.Value;
		}
	}
	public class Person
	{
		private PersonCpId cpId;
		public virtual PersonCpId CpId
		{
			get { return cpId; }
			set { cpId = value; }
		}

		private String name;
		public virtual String Name
		{
			get { return name; }
			set { name = value; }
		}

		private ICollection<Employment> employmentList;
		public virtual ICollection<Employment> EmploymentList
		{
			get { return employmentList; }
			set { employmentList = value; }
		}
	}

	public class EmployerCpId
	{
		private Int32? idA;
		public virtual Int32? IdA
		{
			get { return idA; }
			set { idA = value; }
		}

		private Int32? idB;
		public virtual Int32? IdB
		{
			get { return idB; }
			set { idB = value; }
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			EmployerCpId objCpId = (EmployerCpId)obj;
			return this.IdA == objCpId.IdA && this.IdB == this.IdB;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return this.IdA.Value + this.IdB.Value;
		}
	}

	public class Employer
	{
		private EmployerCpId cpId;
		public virtual EmployerCpId CpId
		{
			get { return cpId; }
			set { cpId = value; }
		}

		private String name;
		public virtual String Name
		{
			get { return name; }
			set { name = value; }
		}

		private ICollection<Employment> employmentList;
		public virtual ICollection<Employment> EmploymentList
		{
			get { return employmentList; }
			set { employmentList = value; }
		}
	}

	public class EmploymentCpId
	{
		private Int32? id;
		public virtual Int32? Id
		{
			get { return id; }
			set { id = value; }
		}

		private Person personObj;
		public virtual Person PersonObj
		{
			get { return personObj; }
			set { personObj = value; }
		}

		private Employer employerObj;
		public virtual Employer EmployerObj
		{
			get { return employerObj; }
			set { employerObj = value; }
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			EmploymentCpId objCpId = (EmploymentCpId)obj;
			return this.Id == objCpId.Id && this.PersonObj.CpId == this.PersonObj.CpId && this.EmployerObj.CpId == this.EmployerObj.CpId;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return this.Id.Value + this.PersonObj.CpId.GetHashCode() + this.EmployerObj.CpId.GetHashCode();
		}
	}

	public class Employment
	{
		private EmploymentCpId cpId;
		public virtual EmploymentCpId CpId
		{
			get { return cpId; }
			set { cpId = value; }
		}

		private String name;
		public virtual String Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	public class PersonNoComponent
	{
		private Int32? idA;
		public virtual Int32? IdA
		{
			get { return idA; }
			set { idA = value; }
		}

		private Int32? idB;
		public virtual Int32? IdB
		{
			get { return idB; }
			set { idB = value; }
		}

		private String name;
		public virtual String Name
		{
			get { return name; }
			set { name = value; }
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			PersonCpId objCpId = (PersonCpId)obj;
			return this.IdA == objCpId.IdA && this.IdB == this.IdB;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return this.IdA.Value + this.IdB.Value;
		}
	}
}