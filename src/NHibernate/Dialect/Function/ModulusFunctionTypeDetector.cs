using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	internal class ModulusFunctionTypeDetector
	{
		// The supported DbTypes with their priorities in order to detect which is the
		// returned type when mixing them
		private readonly Lazy<Dictionary<DbType, KeyValuePair<int, IType>>> _supportedDbTypesLazy;
		private readonly bool _supportDecimals;
		private readonly bool _supportFloatingNumbers;

		public ModulusFunctionTypeDetector(bool supportDecimals, bool supportFloatingNumbers)
		{
			_supportDecimals = supportDecimals;
			_supportFloatingNumbers = supportFloatingNumbers;
			_supportedDbTypesLazy = new Lazy<Dictionary<DbType, KeyValuePair<int, IType>>>(GetSupportedTypes);
		}

		public ModulusFunctionTypeDetector(bool supportDecimals) : this(supportDecimals, false)
		{
		}

		protected virtual Dictionary<DbType, KeyValuePair<int, IType>> GetSupportedTypes()
		{
			var types = new Dictionary<DbType, KeyValuePair<int, IType>>()
			{
					{DbType.Int16, new KeyValuePair<int, IType>(1, NHibernateUtil.Int16)},
					{DbType.Int32, new KeyValuePair<int, IType>(2, NHibernateUtil.Int32)},
					{DbType.Int64, new KeyValuePair<int, IType>(3, NHibernateUtil.Int64)},
			};

			if (_supportDecimals)
			{
				types.Add(DbType.Currency, new KeyValuePair<int, IType>(4, NHibernateUtil.Decimal));
				types.Add(DbType.Decimal, new KeyValuePair<int, IType>(4, NHibernateUtil.Decimal));
			}

			if (_supportFloatingNumbers)
			{
				types.Add(DbType.Single, new KeyValuePair<int, IType>(5, NHibernateUtil.Single));
				types.Add(DbType.Double, new KeyValuePair<int, IType>(6, NHibernateUtil.Double));
			}

			return types;
		}

		public IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			KeyValuePair<int, IType> currentReturnType = default;
			int totalArguments = 0;
			foreach (var argumentType in argumentTypes)
			{
				if (argumentType == null)
				{
					return null;
				}

				SqlType[] sqlTypes;
				try
				{
					sqlTypes = argumentType.SqlTypes(mapping);
				}
				catch (MappingException me)
				{
					if (throwOnError)
					{
						throw new QueryException(me);
					}

					return null;
				}

				if (sqlTypes.Length != 1)
				{
					return ThrowOrReturnDefault("Multi-column type can not be in mod()", throwOnError);
				}

				if (!_supportedDbTypesLazy.Value.TryGetValue(sqlTypes[0].DbType, out var returnType))
				{
					return ThrowOrReturnDefault($"DbType {sqlTypes[0].DbType} is not supported for mod()", throwOnError);
				}

				if (returnType.Key > currentReturnType.Key)
				{
					currentReturnType = returnType;
				}

				totalArguments++;
			}

			return totalArguments == 2
				? currentReturnType.Value
				: ThrowOrReturnDefault("Invalid number of arguments for mod()", throwOnError);
		}

		private IType ThrowOrReturnDefault(string error, bool throwOnError)
		{
			if (throwOnError)
			{
				throw new QueryException(error);
			}

			return null;
		}
	}
}
