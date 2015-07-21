using System.Collections;
using NHibernate.Transform;

namespace NHibernate.Test.QueryTest
{
	public class ResultTransformerStub : IResultTransformer
	{
		private bool _wasTransformTupleCalled;
		private bool _wasTransformListCalled;

		public bool WasTransformTupleCalled
		{
			get { return _wasTransformTupleCalled; }
		}

		public bool WasTransformListCalled
		{
			get { return _wasTransformListCalled; }
		}

		public ResultTransformerStub()
		{
			_wasTransformTupleCalled = false;
			_wasTransformListCalled = false;
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			_wasTransformTupleCalled = true;
			return tuple;
		}

		public IList TransformList(IList collection)
		{
			_wasTransformListCalled = true;
			return collection;
		}
	}
}