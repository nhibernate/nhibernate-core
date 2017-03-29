using System.Collections;
using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Metadata;
using NHibernate.Properties;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	public sealed class Printer
	{
		private readonly ISessionFactoryImplementor _factory;
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(Printer));

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity">an actual entity object, not a proxy!</param>
		/// <returns></returns>
		public string ToString(object entity)
		{
			IClassMetadata cm = _factory.GetClassMetadata(entity.GetType());
			if (cm == null)
			{
				return entity.GetType().FullName;
			}

			IDictionary<string, string> result = new Dictionary<string, string>();

			if (cm.HasIdentifierProperty)
			{
				result[cm.IdentifierPropertyName] =
					cm.IdentifierType.ToLoggableString(cm.GetIdentifier(entity), _factory);
			}

			IType[] types = cm.PropertyTypes;
			string[] names = cm.PropertyNames;
			object[] values = cm.GetPropertyValues(entity);

			for (int i = 0; i < types.Length; i++)
			{
				var value = values[i];
				if (Equals(LazyPropertyInitializer.UnfetchedProperty, value) || Equals(BackrefPropertyAccessor.Unknown, value))
				{
					result[names[i]] = value.ToString();
				}
				else
				{
					result[names[i]] = types[i].ToLoggableString(value, _factory);
				}
			}

			return cm.EntityName + CollectionPrinter.ToString(result);
		}

		public string ToString(IType[] types, object[] values)
		{
			List<string> list = new List<string>(types.Length);

			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] != null)
				{
					list.Add(types[i].ToLoggableString(values[i], _factory));
				}
			}
			return CollectionPrinter.ToString(list);
		}

		public string ToString(IDictionary<string, TypedValue> namedTypedValues)
		{
			IDictionary<string, string> result = new Dictionary<string, string>(namedTypedValues.Count);

			foreach (KeyValuePair<string, TypedValue> me in namedTypedValues)
			{
				TypedValue tv = me.Value;
				result[me.Key] = tv.Type.ToLoggableString(tv.Value, _factory);
			}

			return CollectionPrinter.ToString(result);
		}

		public void ToString(IEnumerator enumerator)
		{
			if (!log.IsDebugEnabled || !enumerator.MoveNext())
			{
				return;
			}

			log.Debug("listing entities:");
			int i = 0;

			do
			{
				if (i++ > 20)
				{
					log.Debug("more......");
					break;
				}
				log.Debug(ToString(enumerator.Current));
			} while (enumerator.MoveNext());
		}

		public Printer(ISessionFactoryImplementor factory)
		{
			_factory = factory;
		}
	}
}