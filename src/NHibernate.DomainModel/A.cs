
using System;

namespace NHibernate.DomainModel
{

 /// <summary>
 /// POCO for A
 /// </summary>
 [Serializable]
 public class A
 {

	#region Fields
	/// <summary>
	/// Holder for id
	/// </summary>
	private Int64 _id;
	
	/// <summary>
	/// Holder for name
	/// </summary>
	private String _name;
	
	#endregion

	#region Constructors
	/// <summary>
	/// Default constructor for class A
	/// </summary>
	public A()
	{
	}
	
	/// <summary>
	/// Constructor for class A
	/// </summary>
	/// <param name="name">Initial name value</param>
	public A(String name)
	{
		this._name = name;
	}
	
	#endregion
	
	#region Properties
	/// <summary>
	/// Get/set for id
	/// </summary>
	public Int64 Id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}
	
	/// <summary>
	/// Get/set for name
	/// </summary>
	public String Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}
	
	#endregion
 }
}