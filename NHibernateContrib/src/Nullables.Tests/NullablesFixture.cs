using System;
using System.Collections;
using System.Reflection;
using System.Text;
using NHibernate.Type;
using Nullables.NHibernate;
using NUnit.Framework;

namespace Nullables.Tests
{
	[TestFixture]
	public class NullablesFixture
	{
		[Test]
		public void AllITypesAreSerializable()
		{
			Assembly nhibernate = typeof(NullableInt32Type).Assembly;
			System.Type[] allTypes = nhibernate.GetTypes();
			
			ArrayList shouldBeSerializable = new ArrayList();
			
			foreach( System.Type type in allTypes )
			{
				if( type.IsClass && typeof( IType ).IsAssignableFrom( type ) )
				{
					if (!type.IsSerializable)
					{
						shouldBeSerializable.Add(type);
					}
				}
			}

			if (shouldBeSerializable.Count > 0)
			{
				StringBuilder message = new StringBuilder();
				foreach (System.Type type in shouldBeSerializable)
				{
					message.Append('\t').Append(type).Append('\n');
				}
				Assert.Fail("These types should be serializable:\n{0}", message.ToString());
			}
		}
	}
}
