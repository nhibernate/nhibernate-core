using System;
using NHibernate.Engine;

namespace NHibernate.Type
{
	[Serializable]
	internal class SubqueryComponentType : ComponentType
	{
		internal SubqueryComponentType(IType[] propertyTypes)
			: base(
				propertyTypes,
				new string[propertyTypes.Length],
				new bool[propertyTypes.Length],
				propertyTypes.Length,
				new CascadeStyle[propertyTypes.Length],
				new FetchMode?[propertyTypes.Length],
				false,
				false,
				null,
				EntityMode.Poco)
		{
		}
	}
}
