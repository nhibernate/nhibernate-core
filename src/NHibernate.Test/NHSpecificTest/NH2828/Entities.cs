using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2828
{
	public class Company
	{
		public virtual Guid Id { get; protected set; }

		/// <summary>
		/// 	The addresses.
		/// </summary>
		private readonly ISet<Address> addresses;

		/// <summary>
		/// The bank accounts.
		/// </summary>
		private readonly ISet<BankAccount> bankAccounts;

		/// <summary>
		/// 	Gets or sets Name.
		/// </summary>
		public virtual string Name { get; set; }

		public Company(){
			this.addresses = new HashSet<Address>();
			this.bankAccounts = new HashSet<BankAccount>();
		}

		/// <summary>
		/// 	Gets or sets Addresses.
		/// </summary>
		public virtual IEnumerable<Address> Addresses
		{
			get
			{
				return this.addresses;
			}
		}

		/// <summary>
		/// 	Gets or sets BankAccounts.
		/// </summary>
		public virtual IEnumerable<BankAccount> BankAccounts
		{
			get
			{
				return this.bankAccounts;
			}
		}
		public virtual bool AddBank(BankAccount bankAccount)
		{
			if (bankAccount == null)
			{
				return false;
			}

			if (this.bankAccounts.Add(bankAccount))
			{
				bankAccount.Company = this;
				return true;
			}
			return false;
		}

		public virtual bool AddAddress(Address address)
		{
			if (address == null)
			{
				return false;
			}

			if (this.addresses.Add(address))
			{
				address.AddCompany(this);
				return true;
			}
			return false;
		}

		public virtual bool RemoveAddress(Address address)
		{
			if (address == null)
			{
				return false;
			}
			if (this.addresses.Remove(address))
			{
				address.RemoveCompany();
				return true;
			}
			return false;
		}

	}

	public class Address
	{
		public virtual Guid Id { get; protected set; }

		public virtual string Name { get; set; }

		public virtual Company Company { get; set; }

		public virtual bool AddCompany(Company company)
		{
			if (company == null)
			{
				return false;
			}

			this.Company = company;
			if (company.AddAddress(this)) return true;
			return false;
		}

		public virtual void RemoveCompany()
		{
			this.Company = null;
		}

	}

	public class BankAccount
	{
		public virtual Guid Id { get; protected set; }
		
		public virtual string Name { get; set; }

		public virtual Company Company { get; set; }

	}
		
}