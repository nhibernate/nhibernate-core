using NHibernate.Type;

namespace NHibernate.Param
{
	internal class NamedListParameter : NamedParameter
	{
		public NamedListParameter(string name, object value, IType elementType) : base(name, value, elementType)
		{
		}

		public override bool IsCollection => true;
	}
}
