using System;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for TypeKey.
	/// </summary>
	internal class TypeKey : IComparable
	{
		#region Member Variables
		
		private System.Type m_Type;
		private System.Type m_ExtendsType;

		#endregion

		/// <summary>
		/// Class constructor.
		/// </summary>
		public TypeKey( System.Type type )
		{
			m_Type = type;
			m_ExtendsType = FindExtendsType( type );			
		}
		
		/// <summary>
		/// Searchs the class hieracrhy for the first class with an attribute that we are looking for.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private System.Type FindExtendsType( System.Type type )
		{
			//check for joined subclass
			JoinedSubclassAttribute joinedSubclassAttr = (JoinedSubclassAttribute)Attribute.GetCustomAttribute( type, typeof(JoinedSubclassAttribute), false );
			if( joinedSubclassAttr != null )
			{
				if( joinedSubclassAttr.Extends != null )
				{
					return joinedSubclassAttr.Extends;
				}
				else
				{
					return type.BaseType;
				}
			}
			else //check for subclass
			{
				SubclassAttribute subclassAttr = (SubclassAttribute)Attribute.GetCustomAttribute( type, typeof(SubclassAttribute), false );
				if( subclassAttr != null )
				{
					if( subclassAttr.Extends != null )
					{
						return subclassAttr.Extends;
					}
					else
					{
						return type.BaseType;
					}
				}
				else //look to the base class
				{
					if( type.BaseType != null )
						return FindExtendsType( type.BaseType );
					else
						return null;
				}
			}
		}
		
		/// <summary>
		/// Gets and sets the Type.
		/// </summary>
		public System.Type Type
		{
			get { return m_Type; }
		}
		
		/// <summary>
		/// Gets and sets the ExtendsType.
		/// </summary>
		public System.Type ExtendsType
		{
			get { return m_ExtendsType; }
		}

		#region IComparable Members
		/// <summary>
		/// Compares the two objects.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo( object obj )
		{
			if( obj is TypeKey )
			{
				TypeKey other = (TypeKey)obj;
				if( this.Type == other.Type )
				{
					return 0;
				}
				else if( this.ExtendsType == other.Type )
				{
					return 1;
				}
				else if( other.ExtendsType == this.Type )
				{
					return -1;
				}
				else
				{
					return -1;
				}
			}
			else
			{
				throw new ArgumentException();
			}
		}
		#endregion
	}
}
