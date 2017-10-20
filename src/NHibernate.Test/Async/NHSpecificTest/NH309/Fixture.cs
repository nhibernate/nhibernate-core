﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH309
{
	using System.Threading.Tasks;
	/// <summary>
	/// Summary description for Fixture.
	/// </summary>
	[TestFixture]
	public class FixtureAsync : TestCase
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
		public async Task RemoveNodeFromNodesCollectionAsync()
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
			await (s.SaveAsync(rootNode));
			await (s.SaveAsync(childNode1));
			await (s.SaveAsync(childNode2));
			await (s.SaveAsync(menu));
			await (s.FlushAsync());
			s.Close();

			s = OpenSession();
			rootNode = (Node) await (s.LoadAsync(typeof(Node), 1));
			Node nodeToBeRemoved = (Node) await (s.LoadAsync(typeof(Node), 3)); // childNode 2 with Id 3
			Menu menu2 = (Menu) await (s.LoadAsync(typeof(Menu), 1));

			int nodePostion = menu2.Nodes.IndexOf(nodeToBeRemoved);
			Assert.AreEqual(2, nodePostion, "Test IndexOf");
			menu2.Nodes.Remove(nodeToBeRemoved);
			Assert.AreEqual(2, menu2.Nodes.Count, "Test count after removal");
			Assert.AreEqual(rootNode, menu2.Nodes[0], "Test identity first node in menu");


			await (s.DeleteAsync("from Node"));
			await (s.DeleteAsync("from Menu"));
			await (s.FlushAsync());
			s.Close();
		}
	}
}