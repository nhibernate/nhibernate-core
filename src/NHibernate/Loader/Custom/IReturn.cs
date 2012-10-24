using NHibernate.Type;

namespace NHibernate.Loader.Custom
{
	/// <summary> Represents a return in a custom query. </summary>
	public interface IReturn
	{
		string Alias { get; }
		IType Type { get; }
	}
}