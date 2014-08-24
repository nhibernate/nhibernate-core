using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1217
{
	[TestFixture,Ignore("Not fixed yet")]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1217"; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from System.Object o");
				tx.Commit();
			}
		}

		/// <summary>
		/// +--------+          +--------+ 1   from   * +--------+
		///	|        | 1      * |        |--------------|        |
		///	|  Root  |----------|  Node  | 1    to    * |  Edge  |
		///	|        |          |        |--------------|        |
		///	+--------+          +--------+              +--------+ 
		/// </summary>
		[Test]
		public void NoExceptionMustBeThrown()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Root r = new Root();
					r.Name = "a root";

					INode n1 = r.AddNode("Node1");
					INode n2 = r.AddNode("Node2");

					IEdge e1 = new Edge();
					e1.Label = "Edge 1";
					e1.FromNode = n1;
					e1.ToNode = n2;
					
					n1.FromEdges.Add(e1);
					n2.ToEdges.Add(e1);
					
					s.Save(r);

					s.Flush();
					tx.Commit();
				}
			}
		}
	}
}