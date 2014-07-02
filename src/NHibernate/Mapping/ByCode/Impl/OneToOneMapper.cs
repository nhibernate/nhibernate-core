using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class OneToOneMapper : IOneToOneMapper
	{
		private readonly IAccessorPropertyMapper _entityPropertyMapper;
		private readonly MemberInfo _member;
		private readonly HbmOneToOne _oneToOne;

		public OneToOneMapper(MemberInfo member, HbmOneToOne oneToOne)
			: this(member, member == null ? (IAccessorPropertyMapper)new NoMemberPropertyMapper() : new AccessorPropertyMapper(member.DeclaringType, member.Name, x => oneToOne.access = x), oneToOne) { }

		public OneToOneMapper(MemberInfo member, IAccessorPropertyMapper accessorMapper, HbmOneToOne oneToOne)
		{
			_member = member;
			_oneToOne = oneToOne;
			if (member == null)
			{
				_oneToOne.access = "none";
			}
			_entityPropertyMapper = member == null ? new NoMemberPropertyMapper() : accessorMapper;
		}

		#region Implementation of IOneToOneMapper

		public void Cascade(Cascade cascadeStyle)
		{
			_oneToOne.cascade = (cascadeStyle.Exclude(ByCode.Cascade.DeleteOrphans)).ToCascadeString();
		}

		#endregion

		#region Implementation of IAccessorPropertyMapper

		public void Access(Accessor accessor)
		{
			_entityPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			_entityPropertyMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock)
		{
			// not supported by HbmOneToOne
		}

		#endregion

		#region IOneToOneMapper Members

		public void Lazy(LazyRelation lazyRelation)
		{
			_oneToOne.lazy = lazyRelation.ToHbm();
			_oneToOne.lazySpecified = _oneToOne.lazy != HbmLaziness.Proxy;
		}

		public void Constrained(bool value)
		{
			_oneToOne.constrained = value;
		}

		public void PropertyReference(MemberInfo propertyInTheOtherSide)
		{
			if (propertyInTheOtherSide == null)
			{
				_oneToOne.propertyref = null;
				return;
			}

			var declaringType = propertyInTheOtherSide.DeclaringType;
			if (_member != null && !declaringType.IsAssignableFrom(_member.GetPropertyOrFieldType()))
			{
				throw new ArgumentOutOfRangeException("propertyInTheOtherSide",
													  string.Format("Expected a member of {0} found the member {1} of {2}",
																	_member.GetPropertyOrFieldType(),
																	propertyInTheOtherSide,
																	declaringType));
			}

			_oneToOne.propertyref = propertyInTheOtherSide.Name;
		}

		public void Formula(string formula)
		{
			if (formula == null)
			{
				_oneToOne.formula = null;
				_oneToOne.formula1 = null;
				return;
			}

			string[] formulaLines = formula.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			if (formulaLines.Length > 1)
			{
				_oneToOne.formula = new[] {new HbmFormula {Text = formulaLines}};
				_oneToOne.formula1 = null;
			}
			else
			{
				_oneToOne.formula1 = formula;
				_oneToOne.formula = null;
			}
		}

		public void ForeignKey(string foreignKeyName)
		{
			_oneToOne.foreignkey = foreignKeyName;
		}

		#endregion
	}

	public class OneToOneMapper<T> : OneToOneMapper, IOneToOneMapper<T>
	{
		public OneToOneMapper(MemberInfo member, HbmOneToOne oneToOne) 
			: base(member, oneToOne)
		{
		}

		public OneToOneMapper(MemberInfo member, IAccessorPropertyMapper accessorMapper, HbmOneToOne oneToOne)
			: base(member, accessorMapper, oneToOne)
		{
		}

		public void PropertyReference<TProperty>(Expression<Func<T, TProperty>> reference)
		{
			PropertyReference(TypeExtensions.DecodeMemberAccessExpression(reference));
		}
	}
}