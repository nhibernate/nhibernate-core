namespace NHibernate.Mapping.ByCode
{
	public interface IGeneratorDef
	{
		string Class { get; }
		object Params { get; }
	}
}