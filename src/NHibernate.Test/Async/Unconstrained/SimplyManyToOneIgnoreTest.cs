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

namespace NHibernate.Test.Unconstrained
{
	using System.Threading.Tasks;
	// represent another possible ""normal"" use (N.B. H3 create the FK if you don't use <formula>)
	[TestFixture]
	public class SimplyManyToOneIgnoreTestAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] {"Unconstrained.Simply.hbm.xml"}; }
		}

		[Test]
		public async Task UnconstrainedAsync()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				SimplyB sb = new SimplyB(100);
				SimplyA sa = new SimplyA("ralph");
				sa.SimplyB = sb;
				await (s.SaveAsync(sb));
				await (s.SaveAsync(sa));
				await (t.CommitAsync());
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					SimplyB sb = (SimplyB) await (s.GetAsync(typeof(SimplyB), 100));
					Assert.IsNotNull(sb);
					await (s.DeleteAsync(sb));
					await (t.CommitAsync());
				}

				// Have to do this in a separate transaction, otherwise ISession.Get retrieves
				// the cached version of SimplyA with its B being not null.
				using (ITransaction t = s.BeginTransaction())
				{
					SimplyA sa = (SimplyA) await (s.GetAsync(typeof(SimplyA), "ralph"));
					Assert.IsNull(sa.SimplyB);
					await (s.DeleteAsync(sa));
					await (t.CommitAsync());
				}
			}
		}
	}
}