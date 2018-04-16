using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// POJO for FooComponent
	/// </summary>
	/// <remark>
	/// This class is autogenerated
	/// </remark>
	[Serializable]
	public class FooComponent
	{
		#region Fields

		/// <summary>
		/// Holder for name
		/// </summary>
		private String _name;

		/// <summary>
		/// Holder for count
		/// </summary>
		private Int32 _count;

		private DateTime[] _importantDates;

		private FooComponent _subcomponent;
		private Fee _fee = new Fee();
		private GlarchProxy _glarch;
		private FooProxy _parent;
		private Baz _baz;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor for class FooComponent
		/// </summary>
		public FooComponent()
		{
		}

		/// <summary>
		/// Constructor for class FooComponent
		/// </summary>
		/// <param name="name">Initial name value</param>
		/// <param name="count">Initial count value</param>
		public FooComponent(String name, Int32 count)
		{
			_name = name;
			_count = count;
		}

		public FooComponent(String name, int count, DateTime[] dates, FooComponent subcomponent)
		{
			_name = name;
			_count = count;
			_importantDates = dates;
			_subcomponent = subcomponent;
		}

		public FooComponent(String name, int count, DateTime[] dates, FooComponent subcomponent, Fee fee)
		{
			_name = name;
			_count = count;
			_importantDates = dates;
			_subcomponent = subcomponent;
			_fee = fee;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get/set for name
		/// </summary>
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Get/set for count
		/// </summary>
		public Int32 Count
		{
			get { return _count; }
			set { _count = value; }
		}

		public DateTime[] ImportantDates
		{
			get { return _importantDates; }
			set { _importantDates = value; }
		}

		public FooComponent Subcomponent
		{
			get { return _subcomponent; }
			set { _subcomponent = value; }
		}

		private String NullString
		{
			get { return null; }
			set
			{
				if (value != null)
					throw new ArgumentException("null component property");
			}
		}

		public Fee Fee
		{
			get { return _fee; }
			set { this._fee = value; }
		}

		public GlarchProxy Glarch
		{
			get { return _glarch; }
			set { _glarch = value; }
		}

		public FooProxy Parent
		{
			get { return _parent; }
			set
			{
				if (_parent != null && value == null)
					throw new ArgumentNullException("null parent set");
				_parent = value;
			}
		}

		public Baz Baz
		{
			get { return _baz; }
			set { _baz = value; }
		}

		#endregion

		#region System.Object Members

		public override bool Equals(object obj)
		{
			FooComponent fc = (FooComponent) obj;
			return Count == fc.Count;
		}

		public override int GetHashCode()
		{
			return Count;
		}

		public override string ToString()
		{
			String result = "FooComponent: " + Name + "=" + Count;
			result += "; dates=[";
			if (_importantDates != null)
			{
				for (int i = 0; i < _importantDates.Length; i++)
				{
					result += (i == 0 ? "" : ", ") + _importantDates[i];
				}
			}
			result += "]";
			if (Subcomponent != null)
			{
				result += " (" + Subcomponent + ")";
			}
			return result;
		}

		#endregion
	}
}
