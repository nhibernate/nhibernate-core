using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH350
{
	public class SecurityDomain
	{
		private long m_ID = -1L;
		private string m_Name;

		private SecurityDomain m_ParentDomain;
		private IList m_ChildDomains;

		public SecurityDomain()
		{
		}

		public long Id
		{
			get { return m_ID; }
			set { m_ID = value; }
		}

		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		public SecurityDomain ParentDomain
		{
			get { return m_ParentDomain; }
			set { m_ParentDomain = value; }
		}

		public IList ChildDomains
		{
			get
			{
				if (m_ChildDomains == null)
				{
					m_ChildDomains = new ArrayList();
				}
				return m_ChildDomains;
			}
			set {m_ChildDomains = value;}
		}
	} 
}
