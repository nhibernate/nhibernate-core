using System;
using System.Collections.Generic;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection
{
	[Serializable]
	public class ContractVariation
	{
		private int id;
		private Contract contract;
		private string text;
		private ISet<Info> infos = new HashSet<Info>();

		public ContractVariation()
		{
		}
		
		public ContractVariation(Contract contract)
		{
			this.contract = contract;
			this.contract.Variations.Add(this);
		}
		
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Contract Contract
		{
			get { return contract; }
			set { contract = value; }
		}
		
		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}
		
		public virtual ISet<Info> Infos
		{
			get { return infos; }
			set { infos = value; }
		}
	}
}
