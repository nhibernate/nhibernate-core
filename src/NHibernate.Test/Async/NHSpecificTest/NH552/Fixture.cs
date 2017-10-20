﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH552
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH552"; }
		}

		[Test]
		public async Task DeleteAndResaveAsync()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				Question q = new Question();
				q.Id = 1;
				await (session.SaveAsync(q));
				await (session.DeleteAsync(q));
				await (session.SaveAsync(q));

				Answer a = new Answer();
				a.Id = 1;
				a.Question = q;
				await (session.SaveAsync(a));
				await (tx.CommitAsync());
			}

			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				await (session.DeleteAsync("from Answer"));
				await (session.DeleteAsync("from Question"));
				await (tx.CommitAsync());
			}
		}
	}
}