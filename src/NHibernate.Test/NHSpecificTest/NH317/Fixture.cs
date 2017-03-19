#if FEATURE_SERIALIZATION
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH317
{
	/// <summary>
	/// Summary description for Fixture.
	/// </summary>
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"NHSpecificTest.NH317.Node.hbm.xml"}; }
		}

		[Test]
		public void ProxySerialization()
		{
			Node node = new Node();
			node.Id = 1;
			node.Name = "Node 1";

			ISession s = sessions.OpenSession();
			s.Save(node);
			s.Flush();
			s.Close();

			s = OpenSession();
			Node nodeProxy = (Node) s.Load(typeof(Node), 1);
			// Test if it is really a proxy
			Assert.IsTrue(nodeProxy is INHibernateProxy);
			s.Close();

			// Serialize
			IFormatter formatter = new BinaryFormatter();
			Node deserializedNodeProxy;
			using (MemoryStream ms = new MemoryStream())
			{
				formatter.Serialize(ms, nodeProxy);

				// Deserialize
				ms.Seek(0, SeekOrigin.Begin);
				deserializedNodeProxy = (Node) formatter.Deserialize(ms);
			}

			// Deserialized proxy should implement the INHibernateProxy interface.
			Assert.IsTrue(deserializedNodeProxy is INHibernateProxy);

			s = OpenSession();
			s.Delete("from Node");
			s.Flush();
			s.Close();
		}
	}
}
#endif
