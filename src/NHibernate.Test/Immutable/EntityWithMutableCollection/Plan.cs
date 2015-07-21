using System;
using System.Collections.Generic;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection
{
	[Serializable]
	public class Plan
	{
		private long id;
		private long version;
		private string description;
		private ISet<Contract> contracts;
		private ISet<Info> infos;
		private Owner owner;
		
		public Plan() : this(null)
		{
		}
		
		public Plan(string description)
		{
			this.description = description;
			this.contracts = new HashSet<Contract>();
			this.infos = new HashSet<Info>();
		}
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
		
		public virtual ISet<Contract> Contracts
		{
			get { return contracts; }
			set { contracts = value; }
		}
		
		public virtual ISet<Info> Infos
		{
			get { return infos; }
			set { infos = value; }
		}
		
		public virtual Owner Owner
		{
			get { return owner; }
			set { owner = value; }
		}
		
		public virtual void AddContract(Contract contract)
		{
			if (!contracts.Add(contract))
			{
				return;
			}
			if (contract.Parent != null)
			{
				AddContract(contract.Parent);
			}
			contract.Plans.Add(this);
			foreach(Contract subContract in contract.Subcontracts)
			{
				AddContract(subContract);
			}
		}
	
		public virtual void RemoveContract(Contract contract)
		{
			if (contract.Parent != null)
			{
				contract.Parent.Subcontracts.Remove(contract);
				contract.Parent = null;			
			}
			RemoveSubcontracts(contract);
			contract.Plans.Remove(this);
			contracts.Remove(contract);
		}
	
		public virtual void RemoveSubcontracts(Contract contract)
		{
			foreach(Contract subContract in contract.Subcontracts)
			{
				RemoveSubcontracts(subContract);
				subContract.Plans.Remove(this);
				contracts.Remove(subContract);
			}
		}
	}
}
