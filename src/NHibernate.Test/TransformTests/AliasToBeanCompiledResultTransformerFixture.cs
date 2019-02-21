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
	}
}
