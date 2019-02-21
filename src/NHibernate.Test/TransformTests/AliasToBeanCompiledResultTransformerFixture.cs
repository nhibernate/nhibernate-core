using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.TransformTests
{
	[TestFixture]
	public class AliasToBeanCompiledResultTransformerFixture : AliasToBeanFixtureBase
	{
		protected override IResultTransformer GetTransformer<T>()
		{
			return Transformers.AliasToBeanCompiled<T>();
		}

		public class PersonDTO
		{
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public int Age { get; set; }
			public decimal SomeAmount { get; set; }
			public double SomeOtherAmount { get; set; }
		}

		[Test, Explicit]
		public void PerfomanceComparisonWithAliasToBean()
		{
			PersonDTO p = null;
			Dictionary<string, object> testData = new Dictionary<string, object>
			{
				{nameof(p.FirstName), "First" },
				{nameof(p.LastName), "Last" },
				{nameof(p.SomeAmount).ToLowerInvariant(), decimal.MaxValue },
				{nameof(p.Age).ToLowerInvariant(), 45 },
				{nameof(p.SomeOtherAmount), 1.5 },
			};
			var tuple = testData.Values.ToArray();
			var aliases = testData.Keys.ToArray();

			//cold run just in case
			Transformers.AliasToBean<PersonDTO>().TransformTuple(tuple, aliases);
			Transformers.AliasToBeanCompiled<PersonDTO>().TransformTuple(tuple, aliases);

			const int numberOfIterations = 50000;
			List<object> list = new List<object>(numberOfIterations);

			using (Timer.Start("AliasToBean"))
			{
				var t = Transformers.AliasToBean<PersonDTO>();
				for (int i = 0; i < numberOfIterations; i++)
				{
					list.Add(t.TransformTuple(tuple, aliases));
				}
			}

			list.Clear();
			list.Capacity = numberOfIterations;

			using (Timer.Start("AliasToBeanCompiled"))
			{
				var t = Transformers.AliasToBeanCompiled<PersonDTO>();
				for (int i = 0; i < numberOfIterations; i++)
				{
					list.Add(t.TransformTuple(tuple, aliases));
				}
			}
		}
	}
}
