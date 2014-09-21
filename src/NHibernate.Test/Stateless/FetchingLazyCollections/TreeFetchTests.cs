using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Stateless.FetchingLazyCollections
{
	public class TreeFetchTests : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.BeforeMapClass += (mi, t, cm) => cm.Id(im => im.Generator(Generators.HighLow));
			mapper.Class<TreeNode>(
				mc =>
				{
					mc.Id(x => x.Id);
					mc.Property(x => x.Content);
					mc.Set(x => x.Children, cam =>
					{
						cam.Key(km => km.Column("parentId"));
						cam.Cascade(Mapping.ByCode.Cascade.All);
					}, rel => rel.OneToMany());
				});
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mappings;
		}

		[Test]
		public void FetchMultipleHierarchies()
		{
			using (ISession s = sessions.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var root = new TreeNode {Content = "Root"};
				var child1 = new TreeNode {Content = "Child1"};
				root.Children.Add(child1);
				root.Children.Add(new TreeNode {Content = "Child2"});
				child1.Children.Add(new TreeNode {Content = "Child1Child1"});
				child1.Children.Add(new TreeNode {Content = "Child1Child2"});
				s.Save(root);
				tx.Commit();
			}

			using (IStatelessSession s = sessions.OpenStatelessSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				IList<TreeNode> rootNodes = s.Query<TreeNode>().Where(t => t.Content == "Root")
				                             .FetchMany(f => f.Children)
				                             .ThenFetchMany(f => f.Children).ToList();
				Assert.That(rootNodes.Count, Is.EqualTo(1));
				Assert.That(rootNodes.First().Children.Count, Is.EqualTo(2));

				tx.Commit();
			}

			using (ISession s = sessions.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from TreeNode");
				tx.Commit();
			}
		}
	}
}