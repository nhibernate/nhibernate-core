using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for TypeNode.
	/// </summary>
	internal class TypeNode
	{
		#region Member Variables

		private System.Type m_Type;
		private TypeNodeCollection m_Nodes;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public TypeNode( System.Type type )
		{
			m_Type = type;
			m_Nodes = new TypeNodeCollection();
		}
		
		/// <summary>
		/// Gets and sets the Nodes.
		/// </summary>
		public TypeNodeCollection Nodes
		{
			get { return m_Nodes; }
		}
		
		/// <summary>
		/// Gets and sets the Type.
		/// </summary>
		public System.Type Type
		{
			get { return m_Type; }
			set { m_Type = value; }
		}

	}
}
