using System;
using System.Collections;

using NHibernate.DomainModel.NHSpecific;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	/// <summary>
	/// Summary description for NodeFixture.
	/// </summary>
	[TestFixture]
	public class NodeFixture : TestCase 
	{

		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "NHSpecific.Node.hbm.xml"}, true );
		}


		[Test]
		public void InsertNodes() 
		{
			
			Node startNode = new Node("start");
			Node levelOneNode = new Node("1");
			Node levelTwoFirstNode = new Node("2-1");
			Node levelTwoSecondNode = new Node("2-2");
			Node levelThreeNode = new Node("3");
			Node endNode = new Node("end");

			startNode.AddDestinationNode(levelOneNode);
			levelOneNode.AddDestinationNode(levelTwoFirstNode);
			levelOneNode.AddDestinationNode(levelTwoSecondNode);
			levelTwoFirstNode.AddDestinationNode(levelThreeNode);
			levelTwoSecondNode.AddDestinationNode(levelThreeNode);
			levelThreeNode.AddDestinationNode(endNode);

			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();

			s.Save(startNode);
			s.Save(levelOneNode);
			s.Save(levelTwoFirstNode);
			s.Save(levelTwoSecondNode);
			s.Save(levelThreeNode);
			s.Save(endNode);

			s.Flush();

			t.Commit();
			s.Close();

			// verify these nodes were actually saved and can be queried correctly.
			ISession s2 = sessions.OpenSession();
			ITransaction t2 = s2.BeginTransaction();
			
			Node startNode2 = (Node)s2.CreateCriteria(typeof(Node))
								.Add(Expression.Expression.Eq("Id", "start"))
								.List()[0];
								
			Assert.AreEqual(1, startNode2.DestinationNodes.Count, "Start Node goes to 1 Node");
			Assert.AreEqual(0, startNode2.PreviousNodes.Count, "Start Node has no previous Nodes");

			Assert.IsTrue(startNode2.DestinationNodes.Contains(levelOneNode), "The DestinationNodes contain the LevelOneNode");

			Node levelOneNode2 = null;
			Node levelTwoFirstNode2 = new Node("2-1");
			Node levelTwoSecondNode2 = new Node("2-2");

			// only one node
			foreach( Node node in startNode2.DestinationNodes ) 
			{
				// replace the levelOneNode from previous session with the one from this Session.
				levelOneNode2 = node;
			}

			Assert.AreEqual(2, levelOneNode2.DestinationNodes.Count, "Level One Node goes to 2 Nodes");
			Assert.AreEqual(1, levelOneNode2.PreviousNodes.Count, "The Start Node lead into Level 1");

			Assert.IsTrue(levelOneNode2.DestinationNodes.Contains(levelTwoFirstNode2), "Level one goes to TwoFirst");
			Assert.IsTrue(levelOneNode2.DestinationNodes.Contains(levelTwoSecondNode2), "Level one goes to TwoSecond");

			Assert.IsTrue(levelOneNode2.PreviousNodes.Contains(startNode2), "Level One can be reached through Start Node");

			t2.Commit();
			s2.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();

			levelThreeNode = (Node)s.Load( typeof(Node), "3" );
			endNode = (Node)s.Load( typeof(Node), "end" );

			Node levelFourOneNode = new Node("4-1");
			Node levelFourTwoNode = new Node("4-2");

			levelThreeNode.RemoveDestinationNode(endNode);
			levelThreeNode.AddDestinationNode(levelFourOneNode);
			levelThreeNode.AddDestinationNode(levelFourTwoNode);

			levelFourOneNode.AddDestinationNode(endNode);
			levelFourTwoNode.AddDestinationNode(endNode);

			s.Save(levelFourOneNode);
			s.Save(levelFourTwoNode);
			
			t.Commit();
			s.Close();

			s = sessions.OpenSession();
			t = s.BeginTransaction();

			levelThreeNode = (Node)s.Load( typeof(Node), "3" );
			endNode = (Node)s.Load( typeof(Node), "end" );

			Assert.AreEqual( 2, levelThreeNode.DestinationNodes.Count, "should be attached to the 2 level 4 nodes" );
			foreach( Node node in levelThreeNode.DestinationNodes ) 
			{
				Assert.IsFalse( node.Equals(endNode), "one of the Dest Nodes in levelThreeNode should not be the end node");
			}

			Assert.AreEqual( 2, endNode.PreviousNodes.Count, "end node should have two nodes leading into it" );

			foreach( Node node in endNode.PreviousNodes ) 
			{
				Assert.IsFalse( node.Equals(levelThreeNode) , "one of the Prev Nodes in should not be the level 3 node, only level 4 nodes" );
			}

			t.Commit();
			s.Close();

		}


	}
}
