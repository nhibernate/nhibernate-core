using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.TransformTests
{
	[TestFixture]
	public class AliasToBeanResultTransformerFixture : AliasToBeanFixtureBase
	{
		protected override IResultTransformer GetTransformer<T>()
		{
			return Transformers.AliasToBean<T>();
		}
	}
}
