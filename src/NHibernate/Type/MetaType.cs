using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	[Serializable]
	public partial class MetaType : AbstractType
	{
		private readonly IDictionary<object, string> _values;
		private readonly IDictionary<string, object> _keys;
		private readonly IType _baseType;
		private readonly ILiteralType _baseLiteralType;

		public MetaType(IDictionary<object, string> values, IType baseType)
		{
			_baseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
			_baseLiteralType = baseType as ILiteralType;
			_values = values;

			if (_values == null)
			{
				if (baseType.ReturnedClass != typeof(string))
					throw new ArgumentException(
						$"Meta type base type {baseType} does not yield string but {baseType.ReturnedClass}, while no " +
						"meta-value mapping has been provided",
						nameof(baseType));
				if (_baseLiteralType == null)
					_baseLiteralType = NHibernateUtil.String;
			}
			else
			{
				_keys = new Dictionary<string, object>();
				foreach (var me in values)
				{
					_keys[me.Value] = me.Key;
				}
			}
		}

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			return _baseType.SqlTypes(mapping);
		}

		public override int GetColumnSpan(IMapping mapping)
		{
			return _baseType.GetColumnSpan(mapping);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(string); }
		}

		public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return GetValueForKey(_baseType.NullSafeGet(rs, names, session, owner));
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return GetValueForKey(_baseType.NullSafeGet(rs, name, session, owner));
		}

		private object GetValueForKey(object key)
		{
			// "_values?[key]" is valid code provided "_values[key]" can never yield null. It is the case because we
			// use a dictionary interface which throws in case of missing key, and because a key associated to a null
			// value would cause the building of the _keys dictionaries to fail.
			return key == null ? null : _values?[key] ?? key;
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			if (settable[0]) NullSafeSet(st, value, index, session);
		}

		public override void NullSafeSet(DbCommand st, object value, int index, ISessionImplementor session)
		{
			// "_keys?[(string) value]" is valid code provided "_keys[(string) value]" can never yield null. It is the
			// case because we use a dictionary interface which throws in case of missing key, and because it is not
			// possible to have a value associated to a null key since generic dictionaries do not support null keys.
			var key = value == null ? null : _keys?[(string) value] ?? value;

			_baseType.NullSafeSet(st, key, index, session);
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (string) value;
		}

		public override string Name
		{
			get { return _baseType.Name; } //TODO!
		}

		public override object DeepCopy(object value, ISessionFactoryImplementor factory)
		{
			return value;
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return checkable[0] && IsDirty(old, current, session);
		}

		public override object Replace(object original, object current, ISessionImplementor session, object owner, System.Collections.IDictionary copiedAlready)
		{
			return original;
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			return _baseType.ToColumnNullness(value, mapping);
		}

		internal string GetMetaValue(string className, Dialect.Dialect dialect)
		{
			var raw = _keys?[className] ?? className;
			if (_baseLiteralType != null)
			{
				return _baseLiteralType.ObjectToSQLString(raw, dialect);
			}

			return raw?.ToString();
		}
	}
}
