using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.Linq;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Baz : INamed, IComparable
	{
		#region Fields

		private NestingComponent _collectionComponent;
		private String _code;
		private FooComponent[] _components;
		private DateTime[] _timeArray;
		private string[] _stringArray;
		private int[] _intArray;
		private FooProxy[] _fooArray;
		private Int32 _count;
		private String _name;
		private Foo _foo;
		private IList<string> _stringList;
		private IList<Fee> _fees;
		private IList<string[]> _customs;
		private IList<FooComponent> _topComponents;
		private IDictionary<Foo, GlarchProxy> _fooToGlarch;
		private IDictionary<FooComponent, Foo> _fooComponentToFoo;
		private IDictionary<Foo, GlarchProxy> _glarchToFoo;
		private IDictionary<string, DateTime?> _stringDateMap;
		private IDictionary<char, GlarchProxy> _topGlarchez;
		private IDictionary<Baz, CompositeElement> _cachedMap;
		private IDictionary<string, Glarch> _stringGlarchMap;
		private IDictionary<object, object> _anyToAny;
		private IList<object> _manyToAny;
		private ISet<FooProxy> _fooSet;
		private ISet<string> _stringSet;
		private ISet<Bar> _topFoos;
		private ISet<BarProxy> _cascadingBars;
		private ISet<CompositeElement> _cached;
		private ISet<Sortable> _sortablez;
		private IList<string> _bag;
		private IList<Foo> _fooBag;
		private IList<Foo> _idFooBag;
		private IList<byte[]> _byteBag;
		private IList<Baz> _bazez;
		private IList<Part> _parts;

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor for class Baz
		/// </summary>
		public Baz()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the _collectionComponent
		/// </summary> 
		public NestingComponent CollectionComponent
		{
			get { return _collectionComponent; }
			set { _collectionComponent = value; }
		}

		/// <summary>
		/// Get/set for Code
		/// </summary>
		public String Code
		{
			get { return this._code; }
			set { this._code = value; }
		}

		/// <summary>
		/// Get/set for count
		/// </summary>
		public Int32 Count
		{
			get { return this._count; }
			set { this._count = value; }
		}

		/// <summary>
		/// Get/set for name
		/// </summary>
		public String Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		/// <summary>
		/// Get/set for Foo
		/// </summary>
		public Foo Foo
		{
			get { return this._foo; }
			set { this._foo = value; }
		}

		/// <summary>
		/// Get/set for stringList
		/// </summary>
		public IList<string> StringList
		{
			get { return this._stringList; }
			set { this._stringList = value; }
		}

		/// <summary>
		/// Get/set for fees
		/// </summary>
		public IList<Fee> Fees
		{
			get { return _fees; }
			set { _fees = value; }
		}

		/// <summary>
		/// Get/set for customs
		/// </summary>
		public IList<string[]> Customs
		{
			get { return this._customs; }
			set { this._customs = value; }
		}

		/// <summary>
		/// Get/set for topComponents
		/// </summary>
		public IList<FooComponent> TopComponents
		{
			get { return this._topComponents; }
			set { this._topComponents = value; }
		}

		/// <summary>
		/// Get/set for fooToGlarch
		/// </summary>
		public IDictionary<Foo, GlarchProxy> FooToGlarch
		{
			get { return this._fooToGlarch; }
			set { this._fooToGlarch = value; }
		}

		/// <summary>
		/// Get/set for fooComponentToFoo
		/// </summary>
		public IDictionary<FooComponent, Foo> FooComponentToFoo
		{
			get { return this._fooComponentToFoo; }
			set { this._fooComponentToFoo = value; }
		}

		/// <summary>
		/// Get/set for glarchToFoo
		/// </summary>
		public IDictionary<Foo, GlarchProxy> GlarchToFoo
		{
			get { return this._glarchToFoo; }
			set { this._glarchToFoo = value; }
		}

		/// <summary>
		/// Get/set for stringDateMap
		/// </summary>
		public IDictionary<string, DateTime?> StringDateMap
		{
			get { return this._stringDateMap; }
			set { this._stringDateMap = value; }
		}

		/// <summary>
		/// Get/set for topGlarchez
		/// </summary>
		public IDictionary<char, GlarchProxy> TopGlarchez
		{
			get { return this._topGlarchez; }
			set { this._topGlarchez = value; }
		}

		/// <summary>
		/// Get/set for cachedMap
		/// </summary>
		public IDictionary<Baz,CompositeElement> CachedMap
		{
			get { return this._cachedMap; }
			set { this._cachedMap = value; }
		}

		/// <summary>
		/// Get/set for stringGlarchMap
		/// </summary>
		public IDictionary<string, Glarch> StringGlarchMap
		{
			get { return this._stringGlarchMap; }
			set { this._stringGlarchMap = value; }
		}

		/// <summary>
		/// Get/set for anyToAny
		/// </summary>
		public IDictionary<object, object> AnyToAny
		{
			get { return this._anyToAny; }
			set { this._anyToAny = value; }
		}

		/// <summary>
		/// Get/set for manyToAny
		/// </summary>
		public IList<object> ManyToAny
		{
			get { return this._manyToAny; }
			set { this._manyToAny = value; }
		}

		/// <summary>
		/// Gets or sets the intArray
		/// </summary> 
		public int[] IntArray
		{
			get { return _intArray; }
			set { _intArray = value; }
		}

		/// <summary>
		/// Gets or sets the _components
		/// </summary> 
		public FooComponent[] Components
		{
			get { return _components; }
			set { _components = value; }
		}

		/// <summary>
		/// Gets or sets the timeArray
		/// </summary> 
		public DateTime[] TimeArray
		{
			get { return _timeArray; }
			set { _timeArray = value; }
		}

		/// <summary>
		/// Gets or sets the stringArray
		/// </summary> 
		public string[] StringArray
		{
			get { return _stringArray; }
			set { _stringArray = value; }
		}


		/// <summary>
		/// Gets or sets the fooArray
		/// </summary> 
		public FooProxy[] FooArray
		{
			get { return _fooArray; }
			set { _fooArray = value; }
		}


		/// <summary>
		/// Get/set for fooSet
		/// </summary>
		public ISet<FooProxy> FooSet
		{
			get { return this._fooSet; }
			set { this._fooSet = value; }
		}

		/// <summary>
		/// Get/set for stringSet
		/// </summary>
		public ISet<string> StringSet
		{
			get { return this._stringSet; }
			set { this._stringSet = value; }
		}

		/// <summary>
		/// Get/set for topFoos
		/// </summary>
		public ISet<Bar> TopFoos
		{
			get { return this._topFoos; }
			set { this._topFoos = value; }
		}

		/// <summary>
		/// Get/set for cascadingBars
		/// </summary>
		public ISet<BarProxy> CascadingBars
		{
			get { return this._cascadingBars; }
			set { this._cascadingBars = value; }
		}

		/// <summary>
		/// Get/set for cached
		/// </summary>
		public ISet<CompositeElement> Cached
		{
			get { return this._cached; }
			set { this._cached = value; }
		}

		/// <summary>
		/// Get/set for sortablez
		/// </summary>
		public ISet<Sortable> Sortablez
		{
			get { return this._sortablez; }
			set { this._sortablez = value; }
		}

		/// <summary>
		/// Get/set for bag
		/// </summary>
		public IList<string> Bag
		{
			get { return this._bag; }
			set { this._bag = value; }
		}

		/// <summary>
		/// Get/set for fooBag
		/// </summary>
		public IList<Foo> FooBag
		{
			get { return this._fooBag; }
			set { this._fooBag = value; }
		}

		/// <summary>
		/// Get/set for bazez
		/// </summary>
		public IList<Baz> Bazez
		{
			get { return this._bazez; }
			set { this._bazez = value; }
		}

		/// <summary>
		/// Get/set for idFooBag
		/// </summary>
		public IList<Foo> IdFooBag
		{
			get { return this._idFooBag; }
			set { this._idFooBag = value; }
		}

		/// <summary>
		/// Get/set for byteBag
		/// </summary>
		public IList<byte[]> ByteBag
		{
			get { return this._byteBag; }
			set { this._byteBag = value; }
		}

		/// <summary>
		/// Get/set for parts
		/// </summary>
		public IList<Part> Parts
		{
			get { return this._parts; }
			set { this._parts = value; }
		}

		#endregion

		public void SetDefaults()
		{
			DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

			StringSet = new HashSet<string> {"foo", "bar", "baz"};

			StringDateMap = new SortedList<string, DateTime?>();
			StringDateMap.Add("now", DateTime.Now);
			StringDateMap.Add("never", null); // value is persisted since NH-2199
			// according to SQL Server the big bag happened in 1753 ;)
			StringDateMap.Add("big bang", new DateTime(1753, 01, 01));
			//StringDateMap.Add( "millenium", new DateTime( 2000, 01, 01 ) );
			StringArray = StringSet.ToArray();
			StringList = new List<string>(StringArray);
			IntArray = new int[] {1, 3, 3, 7};
			FooArray = new Foo[0];
			
			Customs = new List<string[]>();
			Customs.Add(new String[] {"foo", "bar"});
			Customs.Add(new String[] {"A", "B"});
			Customs.Add(new String[] {"1", "2"});

			FooSet = new HashSet<FooProxy>();
			Components = new FooComponent[]
				{
					new FooComponent("foo", 42, null, null),
					new FooComponent("bar", 88, null, new FooComponent("sub", 69, null, null))
				};
			TimeArray = new DateTime[]
				{
					new DateTime(),
					new DateTime(),
					new DateTime(), // H2.1 has null here, but it's illegal on .NET
					new DateTime(0)
				};

			Count = 667;
			Name = "Bazza";
			TopComponents = new List<FooComponent>();
			TopComponents.Add(new FooComponent("foo", 11, new DateTime[] {today, new DateTime(2123, 1, 1)}, null));
			TopComponents.Add(
				new FooComponent("bar", 22, new DateTime[] {new DateTime(2007, 2, 3), new DateTime(1945, 6, 1)}, null));
			TopComponents.Add(null);
			Bag = new List<string>();
			Bag.Add("duplicate");
			Bag.Add("duplicate");
			Bag.Add("duplicate");
			Bag.Add("unique");

			Cached = new LinkedHashSet<CompositeElement>();

			CompositeElement ce = new CompositeElement();
			ce.Foo = "foo";
			ce.Bar = "bar";
			CompositeElement ce2 = new CompositeElement();
			ce2.Foo = "fooxxx";
			ce2.Bar = "barxxx";
			Cached.Add(ce);
			Cached.Add(ce2);
			CachedMap = new SortedList<Baz, CompositeElement>();
			CachedMap.Add(this, ce);
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			return ((Baz) obj).Code.CompareTo(Code);
		}

		#endregion
	}
}