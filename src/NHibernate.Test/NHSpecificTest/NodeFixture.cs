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
			foreach(Node node in startNode2.DestinationNodes.Keys) 
			{
				// replace the levelOneNode from previous session with the one from this Session.
				levelOneNode2 = node;
			}

			Assert.AreEqual(2, levelOneNode2.DestinationNodes.Count, "Level One Node goes to 2 Nodes");
			Assert.AreEqual(1, levelOneNode2.PreviousNodes.Count, "The Start Node lead into Level 1");

			Assert.IsTrue(levelOneNode2.DestinationNodes.Contains(levelTwoFirstNode2), "Level one goes to TwoFirst");
			Assert.IsTrue(levelOneNode2.DestinationNodes.Contains(levelTwoSecondNode2), "Level one goes to TwoSecond");

			Assert.IsTrue(levelOneNode2.PreviousNodes.Contains(startNode2), "Level One can be reached through Start Node");
			
//			// TODO: get rid of this HACK that was used to find out what was causing the problem
//			// lets test out my theory that the problem is the "END" node being loaded during flush by just loading
//			// it before the flush.  If it loads before the flush I don't think there will be any problems.  See
//			// http://jira.nhibernate.org:8080/browse/NH-20 for what I think is happening...
//			foreach(Node node2 in levelOneNode2.DestinationNodes.Keys) 
//			{
//				System.Diagnostics.Debug.WriteLine("touching node2's destinations = " + node2.DestinationNodes.Count);
//				foreach(Node node3 in node2.DestinationNodes.Keys) 
//				{
//					System.Diagnostics.Debug.WriteLine("touching node3's destinations - " + node3.DestinationNodes.Count);
//				}
//			}

			//foreach

			t2.Commit();
			s2.Close();

		}



	
	}
}
