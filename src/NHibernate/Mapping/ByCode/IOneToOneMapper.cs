using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Util;

namespace NHibernate.Mapping.ByCode
{
	public interface IOneToOneMapper : IEntityPropertyMapper
	{
		void Cascade(Cascade cascadeStyle);
		void Lazy(LazyRelation lazyRelation);
		void Constrained(bool value);
		void PropertyReference(MemberInfo propertyInTheOtherSide);
		void Formula(string formula);
		void ForeignKey(string foreignKeyName);
		void Class(System.Type clazz);

		//6.0 TODO: Uncomment
		//void Fetch(FetchKind fetchMode);
	}

	public interface IOneToOneMapper<T> : IOneToOneMapper
	{
		void PropertyReference<TProperty>(Expression<Func<T, TProperty>> reference);
	}

	// 6.0 TODO: move method into IOneToOneMapper
	public static class OneToOneMapperExtensions
	{
		/// <summary>
		/// Maps many formulas.
		/// </summary>
		/// <param name="mapper">The mapper.</param>
		/// <param name="formulas">The formulas to map.</param>
		public static void Formulas(this IOneToOneMapper mapper, params string[] formulas)
		{
			var o2oMapper = ReflectHelper.CastOrThrow<OneToOneMapper>(mapper, "Setting many formula");
			o2oMapper.Formulas(formulas);
		}

		public static void Fetch(this IOneToOneMapper mapper, FetchKind fetchMode)
		{
			var o2oMapper = ReflectHelper.CastOrThrow<OneToOneMapper>(mapper, "Setting fetch");
			o2oMapper.Fetch(fetchMode);
		}
	}
}
