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
			OnSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			mocks.VerifyAll();
			OnTearDown();
		}

		#endregion

		/// <summary>
		/// Overridable for subclasses in order to free/clean resources
		/// </summary>
		protected virtual void OnTearDown()
		{
		}

		/// <summary>
		/// Overridable for subclasses in order to set up classes.
		/// </summary>
		protected virtual void OnSetUp()
		{
		}

		protected MockRepository mocks;

		protected MockRepository Mocks
		{
			get { return mocks; }
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