using System;

using NHibernate.Id;

namespace NHibernate.Engine
{
	/// <summary>
	/// A strategy for determining if a version value is an version of
	/// a new transient instance or a previously persistent transient instance.
	/// The strategy is determined by the <c>Unsaved-Value</c> attribute in the mapping file.
	/// </summary>
	public class VersionValue
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(VersionValue));

		private readonly object value;

		/// <summary></summary>
		protected VersionValue()
		{
			value = null;
		}

		/// <summary>
		/// Assume the transient instance is newly instantiated if its version is null or
		/// equal to <c>Value</c>
		/// </summary>
		/// <param name="value"></param>
		public VersionValue(object value)
		{
			this.value = value;
		}

		/// <summary>
		/// Does the given identifier belong to a new instance
		/// </summary>
		public virtual bool? IsUnsaved(object version)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("unsaved-value: " + value);
			}
			return version == null || version.Equals(value);
		}

		public virtual object GetDefaultValue(object currentValue)
		{
			return value;
		}

		/// <summary>
		/// Assume the transient instance is newly instantiated if the version
		/// is null, otherwise assume it is a detached instance.
		/// </summary>
		public static VersionValue VersionSaveNull = new VersionSaveNullClass();

		private class VersionSaveNullClass : VersionValue
		{
			public override bool? IsUnsaved(object version)
			{
				log.Debug("version unsaved-value strategy NULL");
				return version == null;
			}

			public override object GetDefaultValue(object currentValue)
			{
				return null;
			}
		}

		/// <summary>
		/// Assume the transient instance is newly instantiated if the version
		/// is null, otherwise defer to the identifier unsaved-value.
		/// </summary>
		public static VersionValue VersionUndefined = new VersionUndefinedClass();

		private class VersionUndefinedClass : VersionValue
		{
			public override bool? IsUnsaved(object version)
			{
				log.Debug("version unsaved-value strategy UNDEFINED");
				if (version == null)
					return true;
				else
					return null;
			}

			public override object GetDefaultValue(object currentValue)
			{
				return currentValue;
			}
		}

		/// <summary>
		/// Assume the transient instance is newly instantiated if the identifier
		/// is null.
		/// </summary>
		public static VersionValue VersionNegative = new VersionNegativeClass();

		private class VersionNegativeClass : VersionValue
		{
			public override bool? IsUnsaved(object version)
			{
				log.Debug("version unsaved-value strategy NEGATIVE");
				if (version is short || version is int || version is long)
				{
					return Convert.ToInt64(version) < 0L;
				}
				else
				{
					throw new MappingException("unsaved-value strategy NEGATIVE may only be used with short, int and long types");
				}
			}

			public override object GetDefaultValue(object currentValue)
			{
				return IdentifierGeneratorFactory.CreateNumber(-1L, currentValue.GetType());
			}
		}
	}
}
