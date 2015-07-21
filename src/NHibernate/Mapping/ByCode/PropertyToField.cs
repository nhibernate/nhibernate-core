using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Properties;

namespace NHibernate.Mapping.ByCode
{
	public class PropertyToField
	{
		private static readonly Dictionary<string, IFieldNamingStrategy> FieldNamingStrategies = new Dictionary<string, IFieldNamingStrategy>
			{
				{"camelcase", new CamelCaseStrategy()},
				{"camelcase-underscore", new CamelCaseUnderscoreStrategy()},
				{"lowercase", new LowerCaseStrategy()},
				{"lowercase-underscore", new LowerCaseUnderscoreStrategy()},
				{"pascalcase-underscore", new PascalCaseUnderscoreStrategy()},
				{"pascalcase-m-underscore", new PascalCaseMUnderscoreStrategy()},
			};

		/// <summary>
		/// Dictionary containing the embedded strategies to find a field giving a property name.
		/// The key is the "partial-name" of the strategy used in XML mapping.
		/// The value is an instance of the strategy.
		/// </summary>
		public static IDictionary<string, IFieldNamingStrategy> DefaultStrategies
		{
			get
			{
				// please leave it as no read-only; the user may need to add his strategies or remove existing if he no want his people use it.
				return FieldNamingStrategies;
			}
		}

		public static FieldInfo GetBackFieldInfo(PropertyInfo subject)
		{
			const BindingFlags defaultBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			return (from s in FieldNamingStrategies.Values
			        let field = subject.DeclaringType.GetField(s.GetFieldName(subject.Name), defaultBinding)
			        where field != null
			        select field).FirstOrDefault();
		}
	}
}