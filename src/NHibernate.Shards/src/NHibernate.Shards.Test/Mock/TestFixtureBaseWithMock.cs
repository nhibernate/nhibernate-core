using NUnit.Framework;
using Rhino.Mocks;

namespace NHibernate.Shards.Test.Mock
{
	[TestFixture]
	public class TestFixtureBaseWithMock
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
		}

		#endregion

		protected MockRepository mocks;

		protected MockRepository Mocks
		{
			get
			{
				return mocks;
			}
		}

		protected T Mock<T>()
		{
			return Mocks.CreateMock<T>();
		}

		protected T Stub<T>()
		{
			return MockRepository.GenerateStub<T>();
		}

		protected T Stub<T>(params object[] parameters)
		{
			return MockRepository.GenerateStub<T>(parameters);
		}
	}
}