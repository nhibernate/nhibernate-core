using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1349
{
	public class Services
	{
		private int id;
		private int companyCount;
		private string accountNumber;
		private string name;
		private string type;

		public Services()
		{ }

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string AccountNumber
		{
			get { return accountNumber; }
			set { accountNumber = value; }
		}
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		public virtual string Type
		{
			get { return type; }
			set { type = value; }
		}
		public virtual int CompanyCount
		{
			get { return companyCount; }
			set { companyCount = value; }
		}
		public override string ToString()
		{
			return (this.id + "] [" + this.accountNumber + "]  [" + this.name + "] [" + this.type + "]  [" + this.CompanyCount + "]");

		}
	}

}
