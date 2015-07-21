using System;
using System.Collections.Generic;

namespace NHibernate.Test.Immutable
{
	[Serializable]
	public class Party
	{
		private long id;
		private long version;
		private Contract contract;
		private string name;
		private ISet<Info> infos = new HashSet<Info>();
			
		public Party()
		{
		}
		
		public Party(string name)
		{
			this.name = name;
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
		
		public virtual Contract Contract
		{
			get { return contract; }
			set { contract = value; }
		}
		
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public virtual ISet<Info> Infos
		{
			get { return infos; }
			set { infos = value; }
		}
	}
}
