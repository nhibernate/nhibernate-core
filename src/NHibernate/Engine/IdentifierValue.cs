

namespace NHibernate.Engine
{
	/// <summary>
	/// A strategy for determining if an identifier value is an identifier of a new 
	/// transient instance or a previously persistent transient instance. The strategy
	/// is determined by the <c>Unsaved-Value</c> attribute in the mapping file.
	/// </summary>
	public class IdentifierValue
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(IdentifierValue));

		private readonly object value;

		/// <summary></summary>
		protected IdentifierValue()
		{
			value = null;
		}

		/// <summary>
		/// Assume the transient instance is newly instantiated if its identifier is null or
		/// equal to <c>Value</c>
		/// </summary>
		/// <param name="value"></param>
		public IdentifierValue(object value)
		{
			this.value = value;
		}

		/// <summary>
		/// Does the given identifier belong to a new instance
		/// </summary>
		public virtual bool? IsUnsaved(object id)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("unsaved-value: " + value);
			}
			return id == null || id.Equals(value);
		}

		public virtual object GetDefaultValue(object currentValue)
		{
			return value;
		}

		/// <summary>
		/// Always assume the transient instance is newly instantiated
		/// </summary>
		public static readonly IdentifierValue SaveAny = new SaveAnyClass();

		private class SaveAnyClass : IdentifierValue
		{
			public override bool? IsUnsaved(object id)
			{
				log.Debug("unsaved-value strategy ANY");
				return true;
			}

			public override object GetDefaultValue(object currentValue)
			{
				return currentValue;
			}
		}

		/// <summary>
		/// Never assume that transient instance is newly instantiated
		/// </summary>
		public static readonly IdentifierValue SaveNone = new SaveNoneClass();

		private class SaveNoneClass : IdentifierValue
		{
			public override bool? IsUnsaved(object id)
			{
				log.Debug("unsaved-value strategy NONE");
				return false;
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
		public static readonly IdentifierValue SaveNull = new SaveNullClass();

		private class SaveNullClass : IdentifierValue
		{
			public override bool? IsUnsaved(object id)
			{
				log.Debug("unsaved-value strategy NULL");
				return id == null;
			}

			public override object GetDefaultValue(object currentValue)
			{
				return null;
			}
		}

		/// <summary> Assume nothing.</summary>
		public static readonly IdentifierValue Undefined = new UndefinedClass();

		public class UndefinedClass : IdentifierValue
		{
			public override bool? IsUnsaved(object id)
			{
				log.Debug("id unsaved-value strategy UNDEFINED");
				return null;
			}
			public override object GetDefaultValue(object currentValue)
			{
				return null;
			}
		}
	}
}
