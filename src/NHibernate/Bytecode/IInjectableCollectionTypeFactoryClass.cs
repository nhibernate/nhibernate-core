namespace NHibernate.Bytecode
{
	public interface IInjectableCollectionTypeFactoryClass
	{
		void SetCollectionTypeFactoryClass(string typeAssemblyQualifiedName);
		void SetCollectionTypeFactoryClass(System.Type type);
	}
}