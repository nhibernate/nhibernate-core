using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1879
{
	[TestFixture]
	public abstract class GH1879BaseFixture<T> : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Client>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});
			mapper.JoinedSubclass<CorporateClient>(rc =>
			{
				rc.Property(x => x.CorporateId);
			});
			mapper.Class<Project>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.EmailPref, m => m.Type<EnumType<EmailPref>>());
				rc.ManyToOne(x => x.Client, m => m.Column("ClientId"));
				rc.ManyToOne(x => x.BillingClient, m => m.Column("BillingClientId"));
				rc.ManyToOne(x => x.CorporateClient, m => m.Column("CorporateClientId"));
			});
			mapper.Class<Issue>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Project, m => m.Column("ProjectId"));
				rc.ManyToOne(x => x.Client, m => m.Column("ClientId"));
			});
			mapper.Class<Invoice>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.InvoiceNumber);
				rc.ManyToOne(x => x.Project, m => m.Column("ProjectId"));
				rc.ManyToOne(x => x.Issue, m => m.Column("IssueId"));
			});
			mapper.Class<Employee>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.ReviewAsPrimary);
				rc.Set(x => x.WorkIssues, 
				         m => 
				         { 
					         m.Table("EmployeesToWorkIssues"); 
					         m.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					         m.Key(k => k.Column(c => c.Name("EmployeeId")) );
				         }, 
				         rel => rel.ManyToMany(m => m.Column("IssueId")));
				rc.Set(x => x.ReviewIssues, 
				         m => 
				         { 
					         m.Table("EmployeesToReviewIssues");
					         m.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					         m.Key(k => k.Column(c => c.Name("EmployeeId")) );
				         }, 
				         rel => rel.ManyToMany(m => m.Column("IssueId")));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}
		}

		protected void AreEqual<TResult>(
		    Func<IQueryable<T>, IQueryable<TResult>> actualQuery, 
		    Func<IQueryable<T>, IQueryable<TResult>> expectedQuery)
		{
			using (var session = OpenSession())
			{
				IEnumerable<TResult> expectedResult = null;
				try
				{
					expectedResult = expectedQuery(session.Query<T>()).ToList();
				}
				catch
				{
					Assert.Ignore("Not currently supported query");
				}
				
				var testResult = actualQuery(session.Query<T>()).ToList();
				Assert.That(testResult, Is.EqualTo(expectedResult));
			}
		}
	}
}
