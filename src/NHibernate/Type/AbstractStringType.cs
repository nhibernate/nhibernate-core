using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	[Serializable]
	public abstract class AbstractStringType : ImmutableType, IDiscriminatorType, IParameterizedType
	{
		private static StringComparer _defaultComparer = StringComparer.Ordinal;
		private StringComparer _comparer;

		/// <summary>
		/// The comparer culture parameter name. Value should be <c>Current</c>, <c>Invariant</c>,
		/// <c>Ordinal</c> or any valid culture name.
		/// </summary>
		/// <remarks>Default comparison is ordinal.</remarks>
		public const string ComparerCultureParameterName = "ComparerCulture";

		/// <summary>
		/// The case sensitivity parameter name. Value should be a boolean, <c>true</c> meaning
		/// case insensitive.
		/// </summary>
		/// <remarks>Default comparison is case sensitive.</remarks>
		public const string IgnoreCaseParameterName = "IgnoreCase";

		/// <summary>
		/// The default string comparer for determining string equality and calculating hash codes.
		/// Default is <c>StringComparer.Ordinal</c>.
		/// </summary>
		public static StringComparer DefaultComparer
		{
			get => _defaultComparer;
			set => _defaultComparer = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		/// The string comparer of this instance of string type, for determining string equality and
		/// calculating hash codes. Set <see langword="null" /> to use <see cref="DefaultComparer"/>.
		/// </summary>
		protected StringComparer Comparer
		{
			get => _comparer ?? _defaultComparer;
			set => _comparer = value;
		}

		public AbstractStringType(SqlType sqlType)
			: base(sqlType)
		{
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			var parameter = cmd.Parameters[index];

			//Allow the driver to adjust the parameter for the value
			session.Factory.ConnectionProvider.Driver.AdjustParameterForValue(parameter, SqlType, value);

			// set the parameter value before the size check, since ODBC changes the size automatically
			parameter.Value = value;

			if (parameter.Size > 0 && ((string) value).Length > parameter.Size)
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
			return Comparer.Equals(x, y);
		}

		public override int GetHashCode(object x)
		{
			return Comparer.GetHashCode(x);
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
			return (string) val;
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
			return "'" + (string) value + "'";
		}

		#endregion

		#region IParameterizedType Members

		public void SetParameterValues(IDictionary<string, string> parameters)
		{
			if (parameters == null)
				return;

			var ignoreCase = false;
			var hasCultureNameParameter = parameters.TryGetValue(ComparerCultureParameterName, out var cultureName);
			var hasIgnoreCaseParameter =
				parameters.TryGetValue(IgnoreCaseParameterName, out var ignoreCaseString) &&
				bool.TryParse(ignoreCaseString, out ignoreCase);

			if (hasCultureNameParameter || hasIgnoreCaseParameter)
			{
				Comparer = GetComparer(cultureName, ignoreCase);
			}
		}

		private static StringComparer GetComparer(string cultureName, bool ignoreCase)
		{
			if (cultureName == null ||
				string.Equals(cultureName, "ordinal", StringComparison.OrdinalIgnoreCase))
			{
				return ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
			}

			return StringComparer.Create(GetCultureInfo(cultureName), ignoreCase);
		}

		private static CultureInfo GetCultureInfo(string cultureName)
		{
			if (string.Equals(cultureName, "invariant", StringComparison.OrdinalIgnoreCase))
			{
				return CultureInfo.InvariantCulture;
			}

			if (string.Equals(cultureName, "current", StringComparison.OrdinalIgnoreCase))
			{
				return CultureInfo.CurrentCulture;
			}

			return CultureInfo.GetCultureInfo(cultureName);
		}

		#endregion

		#region override of System.Object Members

		/// <summary>
		/// Determines whether the specified <see cref="Object"/> is equal to this
		/// <see cref="AbstractStringType"/>.
		/// </summary>
		/// <param name="obj">The <see cref="Object"/> to compare with this <c>AbstractStringType</c>.</param>
		/// <returns><see langword="true"/> if the SqlType, Name and Comparer properties are the same.</returns>
		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
				return false;

			return Equals(Comparer, ((AbstractStringType) obj).Comparer);
		}

		/// <summary>
		/// Serves as a hash function for the <see cref="AbstractStringType"/>, 
		/// suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code that is based on the <see cref="NullableType.SqlType"/>'s
		/// hash code, the <see cref="AbstractType.Name"/>'s hash code and the <see cref="Comparer"/> hash
		/// code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode() * 37 ^ Comparer.GetHashCode();
		}

		#endregion
	}
}
