using NHibernate.Mapping.ByCode.Impl;

namespace NHibernate.Mapping.ByCode
{
	public interface IConformistHoldersProvider
	{
		ICustomizersHolder CustomizersHolder { get; }
		IModelExplicitDeclarationsHolder ExplicitDeclarationsHolder { get; }
	}
}