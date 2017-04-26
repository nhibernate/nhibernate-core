using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public static class Generators
	{
		static Generators()
		{
			Native = new NativeGeneratorDef();
			HighLow = new HighLowGeneratorDef();
			Guid = new GuidGeneratorDef();
			GuidComb = new GuidCombGeneratorDef();
			Sequence = new SequenceGeneratorDef();
			Identity = new IdentityGeneratorDef();
			Assigned = new AssignedGeneratorDef();
			EnhancedSequence = new EnhancedSequenceGeneratorDef();
			EnhancedTable = new EnhancedTableGeneratorDef();
			Counter = new CounterGeneratorDef();
			Increment = new IncrementGeneratorDef();
			NativeGuid = new NativeGuidGeneratorDef();
			Select = new SelectGeneratorDef();
			SequenceHiLo = new SequenceHiLoGeneratorDef();
			SequenceIdentity = new SequenceIdentityGeneratorDef();
			Table = new TableGeneratorDef();
			TriggerIdentity = new TriggerIdentityGeneratorDef();
			UUIDString = new UUIDStringGeneratorDef();
		}

		public static IGeneratorDef Assigned { get; private set; }
		public static IGeneratorDef Native { get; private set; }
		public static IGeneratorDef HighLow { get; private set; }
		public static IGeneratorDef Guid { get; private set; }
		public static IGeneratorDef GuidComb { get; private set; }
		public static IGeneratorDef Sequence { get; private set; }
		public static IGeneratorDef Identity { get; private set; }
		public static IGeneratorDef EnhancedSequence { get; private set; }
		public static IGeneratorDef EnhancedTable { get; private set; }
		public static IGeneratorDef Counter { get; private set; }
		public static IGeneratorDef Increment { get; private set; }
		public static IGeneratorDef NativeGuid { get; private set; }
		public static IGeneratorDef Select { get; private set; }
		public static IGeneratorDef SequenceHiLo { get; private set; }
		public static IGeneratorDef Table { get; private set; }
		public static IGeneratorDef TriggerIdentity { get; private set; }
		public static IGeneratorDef SequenceIdentity { get; private set; }

		public static IGeneratorDef UUIDHex()
		{
			return new UUIDHexGeneratorDef(); 
		}

		public static IGeneratorDef UUIDHex(string format)
		{
			return new UUIDHexGeneratorDef(format); 
		}

		public static IGeneratorDef UUIDHex(string format, string separator)
		{
			return new UUIDHexGeneratorDef(format, separator); 
		}

		public static IGeneratorDef UUIDString { get; private set; }

		public static IGeneratorDef Foreign<TEntity>(Expression<Func<TEntity, object>> property)
		{
			return new ForeignGeneratorDef(TypeExtensions.DecodeMemberAccessExpression(property));
		}

		public static IGeneratorDef Foreign(MemberInfo property)
		{
			return new ForeignGeneratorDef(property);
		}
	}

	public class UUIDStringGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "uuid.string"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(string); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class UUIDHexGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		private readonly object param;

		public UUIDHexGeneratorDef()
		{
		}

		public UUIDHexGeneratorDef(string format)
		{
			if (format == null)
				throw new ArgumentNullException("format");
			param = new { format = format };
		}

		public UUIDHexGeneratorDef(string format, string separator)
		{
			if (format == null)
				throw new ArgumentNullException("format");
			if (separator == null)
				throw new ArgumentNullException("separator");
			param = new { format = format, separator = separator };
		}

		public string Class
		{
			get { return "uuid.hex"; }
		}

		public object Params
		{
			get { return param; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(string); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class TriggerIdentityGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "trigger-identity"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class TableHiLoGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "table-hilo"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class TableGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "table"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class SequenceIdentityGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "sequence-identity"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class SequenceHiLoGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "seqhilo"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class SelectGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "select"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return null; }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class NativeGuidGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "guid.native"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(Guid); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}

		#endregion
	}

	public class IncrementGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "increment"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(long); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class CounterGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "counter"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(short); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class AssignedGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "assigned"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return null; }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class ForeignGeneratorDef : IGeneratorDef
	{
		private readonly object param;

		public ForeignGeneratorDef(MemberInfo foreignProperty)
		{
			if (foreignProperty == null)
			{
				throw new ArgumentNullException("foreignProperty");
			}
			param = new {property = foreignProperty.Name};
		}

		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "foreign"; }
		}

		public object Params
		{
			get { return param; }
		}

		public System.Type DefaultReturnType
		{
			get { return null; }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return false; }
		}

		#endregion
	}

	public class NativeGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "native"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}

		#endregion
	}

	public class HighLowGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "hilo"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}
		#endregion
	}

	public class GuidGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "guid"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(Guid); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}
		#endregion
	}

	public class GuidCombGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "guid.comb"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(Guid); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}
		#endregion
	}

	public class SequenceGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "sequence"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}
		#endregion
	}

	public class IdentityGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "identity"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}
		#endregion
	}

	public class EnhancedSequenceGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "enhanced-sequence"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}
		#endregion
	}

	public class EnhancedTableGeneratorDef : IGeneratorDef
	{
		#region Implementation of IGeneratorDef

		public string Class
		{
			get { return "enhanced-table"; }
		}

		public object Params
		{
			get { return null; }
		}

		public System.Type DefaultReturnType
		{
			get { return typeof(int); }
		}

		public bool SupportedAsCollectionElementId
		{
			get { return true; }
		}
		#endregion
	}
}
