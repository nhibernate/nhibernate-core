using System;
using System.Collections.Generic;

namespace NHibernate.Test.Immutable
{
	[Serializable]
	public class ContractVariation
	{
		private long version;
		private Contract contract;
		private string text;
		private ISet<Info> infos = new HashSet<Info>();

		public ContractVariation()
		{
		}
		
		public ContractVariation(int version, Contract contract)
		{
			this.version = version;
			this.contract = contract;
			this.contract.Variations.Add(this);
		}
		
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
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
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				hashCode += 1000000007 * version.GetHashCode();
				if (contract != null)
					hashCode += 1000000009 * contract.GetHashCode();
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			ContractVariation other = obj as ContractVariation;
			if (other == null)
				return false;
			return (this.version == other.version) && (this.contract.Id == other.contract.Id);
		}
	}
}
