using NHibernate.Id;

namespace NHibernate.Mapping.ByCode
{
	public enum Accessor
	{
		Property,
		Field,
		NoSetter,
		ReadOnly,
		None,
		//NH-3720
		FieldCamelCase,
		FieldCamelCaseUnderscore,
		FieldCamelCaseMUnderscore,
		FieldLowerCase,
		FieldLowerCaseUnderscore,
		FieldPascalCaseUnderscore,
		FieldPascalCaseM,
		FieldPascalCaseMUnderscore
	}

	public interface IAccessorPropertyMapper
	{
		void Access(Accessor accessor);
		void Access(System.Type accessorType);
		void Access<T>() where T : IIdentifierGenerator, new();
	}
}