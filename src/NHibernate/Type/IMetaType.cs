namespace NHibernate.Type
{
	interface IMetaType
	{
		string GetMetaValue(string className, Dialect.Dialect dialect);
	}
}