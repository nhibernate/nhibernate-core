using System;

namespace NHibernate.DomainModel
{

 [Serializable]
 public class CompositeElement : IComparable
 {

	#region Fields
	private String _foo;
	private String _bar;
	
	#endregion

	#region Constructors
	/// <summary>
	/// Default constructor for class CompositeElement
	/// </summary>
	public CompositeElement()
	{
	}
	
	/// <summary>
	/// Constructor for class CompositeElement
	/// </summary>
	/// <param name="foo">Initial foo value</param>
	/// <param name="bar">Initial bar value</param>
	public CompositeElement(String foo, String bar)
	{
		this._foo = foo;
		this._bar = bar;
	}
	
	#endregion
	
	#region Properties
	/// <summary>
	/// Get/set for foo
	/// </summary>
	public String Foo
	{
		get { return _foo; }
		set { _foo = value; }
	}
	
	/// <summary>
	/// Get/set for bar
	/// </summary>
	public String Bar
	{
		get { return _bar; }
		set { _bar = value; }
	}
	
	#endregion

	 
	#region IComparable Members

	 public int CompareTo(object obj)
	 {
			 return ( (CompositeElement) obj ).Foo.CompareTo(Foo);
	 }

	 #endregion
 }
}