using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for GeneratorParameterAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
	public class GeneratorParameterAttribute : Attribute
	{
		#region Member Variables

		private string m_Name;
		private string m_Value;
		
		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public GeneratorParameterAttribute( string name, string value )
		{
			m_Name = name;
			m_Value = value;			
		}
		
		/// <summary>
		/// Gets and sets the Name.
		/// </summary>
		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}
    
		/// <summary>
		/// Gets and sets the Value.
		/// </summary>
		public string Value
		{
			get { return m_Value; }
			set { m_Value = value; }
		}
    
	}
}
