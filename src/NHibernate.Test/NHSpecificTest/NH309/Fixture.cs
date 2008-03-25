using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH309
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
			get
			{
				return new string[]
					{
						"NHSpecificTest.NH309.Node.hbm.xml",
						"NHSpecificTest.NH309.Menu.hbm.xml"
					};
			}
		}

		[Test]
		public void RemoveNodeFromNodesCollection()
		{
			Node rootNode = new Node();
			rootNode.Id = 1;
			rootNode.Name = "Root node";

			Node childNode1 = new Node();
			childNode1.Id = 2;
			childNode1.Name = "Child Node 1";
			childNode1.ParentNode = rootNode;

			Node childNode2 = new Node();
			childNode2.Id = 3;
			childNode2.Name = "Child Node 2";
			childNode2.ParentNode = rootNode;

			Menu menu = new Menu();
			menu.Id = 1;
			menu.Name = "Menu";
			menu.Nodes.Add(rootNode);
			menu.Nodes.Add(childNode1);
			menu.Nodes.Add(childNode2);

			ISession s = OpenSession();
			s.Save(rootNode);
			s.Save(childNode1);
			s.Save(childNode2);
			s.Save(menu);
			s.Flush();
			s.Close();

			s = OpenSession();
			rootNode = (Node) s.Load(typeof(Node), 1);
			Node nodeToBeRemoved = (Node) s.Load(typeof(Node), 3); // childNode 2 with Id 3
			Menu menu2 = (Menu) s.Load(typeof(Menu), 1);

			int nodePostion = menu2.Nodes.IndexOf(nodeToBeRemoved);
			Assert.AreEqual(2, nodePostion, "Test IndexOf");
			menu2.Nodes.Remove(nodeToBeRemoved);
			Assert.AreEqual(2, menu2.Nodes.Count, "Test count after removal");
			Assert.AreEqual(rootNode, menu2.Nodes[0], "Test identity first node in menu");


			s.Delete("from Node");
			s.Delete("from Menu");
			s.Flush();
			s.Close();
		}
	}
}