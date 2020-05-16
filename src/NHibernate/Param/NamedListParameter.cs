using NHibernate.Type;

namespace NHibernate.Param
{
	internal class NamedListParameter : NamedParameter
	{
		public NamedListParameter(string name, object value, IType elementType) : base(name, value, elementType)
		{
		}

		internal NamedListParameter(string name, object value, IType type, int index) : base(name, value, type, index)
		{
		}

		public override bool IsCollection => true;
	}
}
