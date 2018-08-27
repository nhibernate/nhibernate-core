using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	[Serializable]
	public abstract class AbstractStringType: ImmutableType, IDiscriminatorType, IParameterizedType
	{
		/// <summary>
		/// The comparer culture parameter name. Value should be <c>Current</c>, <c>Invariant</c>,
		/// <c>Ordinal</c> or any valid culture name.
		/// </summary>
		/// <remarks>Default comparison is ordinal.</remarks>
		public const string ComparerCultureParameterName = "ComparerCulture";

		/// <summary>
		/// The case sensitivity parameter name. Value should be a boolean, <c>false</c> meaning
		/// case insensitive.
		/// </summary>
		/// <remarks>Default comparison is case sensitive.</remarks>
		public const string CaseSensitiveParameterName = "CaseSensitive";

		/// <summary>
		/// The default string comparer for string equality and hashcodes. <see langword="null" /> for
		/// using .Net default equality and hashcode computation.
		/// </summary>
		public static StringComparer DefaultComparer { get; set; }

		/// <summary>
		/// The string comparer of this instance of string type, for string equality and hashcodes.
		/// <see langword="null" /> for using <see cref="DefaultComparer"/>.
		/// </summary>
		protected StringComparer Comparer { get; set; }

		public AbstractStringType(SqlType sqlType)
			: base(sqlType)
		{
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var parameter = cmd.Parameters[index];

			// set the parameter value before the size check, since ODBC changes the size automatically
			parameter.Value = value;

			if (parameter.Size > 0 && ((string)value).Length > parameter.Size)
				throw new HibernateException("The length of the string value exceeds the length configured in the mapping/parameter.");
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return Convert.ToString(rs[index]);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Convert.ToString(rs[name]);
		}

		public override bool IsEqual(object x, object y)
		{
			return (Comparer ?? DefaultComparer)?.Equals(x, y) ?? base.IsEqual(x, y);
		}

		public override int GetHashCode(object x)
		{
			return (Comparer ?? DefaultComparer)?.GetHashCode(x) ?? base.GetHashCode(x);
		}

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val)
		{
			return (string)val;
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return xml;
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(string); }
		}

		#region IIdentifierType Members

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			return xml;
		}

		#endregion

		#region ILiteralType Members

		public string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + (string)value + "'";
		}

		#endregion

		#region IParameterizedType Members

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			if (parameters == null)
				return;

			bool? caseSensitive = null;
			if (parameters.TryGetValue(CaseSensitiveParameterName, out var value))
			{
				caseSensitive = !StringComparer.OrdinalIgnoreCase.Equals("false", value);
			}

			parameters.TryGetValue(ComparerCultureParameterName, out var cultureName);
			switch (cultureName?.ToLowerInvariant())
			{
				case null:
					if (caseSensitive.HasValue)
					{
						Comparer = caseSensitive.Value ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
					}
					// else use default comparer
					break;
				case "current":
					Comparer = caseSensitive == false ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
					break;
				case "invariant":
					Comparer = caseSensitive == false ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
					break;
				case "ordinal":
					Comparer = caseSensitive == false ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
					break;
				default:
					var culture = CultureInfo.GetCultureInfo(cultureName);
					Comparer = StringComparer.Create(culture, caseSensitive == false);
					break;
			}
		}

		#endregion
	}
}
