namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Base class for fixtures testing individual types, created to avoid
	/// code duplication in derived classes. It assumes that the fixture
	/// uses mapping file named "(TypeName)Class.hbm.xml" placed in TypesTest
	/// directory in NHibernate.Test assembly.
	/// </summary>
	public abstract class TypeFixtureBase : TestCase
	{
		protected abstract string TypeName { get; }

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { $"TypesTest.{TypeName}Class.hbm.xml" };

		protected override void OnTearDown()
		{
			using var s = OpenSession();
			using var t = s.BeginTransaction();
			s.CreateQuery($"delete {TypeName}Class").ExecuteUpdate();
			t.Commit();
		}
	}
}
