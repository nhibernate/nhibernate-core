using System;
using System.Collections;

namespace NHibernate.Test.ConnectionTest
{
	public abstract class ConnectionManagementTestCase : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"ConnectionTest.Silly.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected virtual void Prepare()
		{
		}

		protected virtual void Done()
		{
		}

		protected abstract ISession GetSessionUnderTest();

		protected virtual void Release(ISession session)
		{
			if (session != null && session.IsOpen)
			{
				try
				{
					session.Close();
				}
				catch
				{
					// Ignore
				}
			}
		}
	}
}