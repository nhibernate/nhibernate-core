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

		public static IGeneratorDef Foreign<TEntity>(Expression<Func<TEntity, object>> property)
		{
			return new ForeignGeneratorDef(TypeExtensions.DecodeMemberAccessExpression(property));
		}

		public static IGeneratorDef Foreign(MemberInfo property)
		{
			return new ForeignGeneratorDef(property);
		}
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