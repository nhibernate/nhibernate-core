//$Id$
using System;
using StringHelper = NHibernate.Util.StringHelper;

namespace NHibernate.tool.hbm2net
{
	
	/*
	* Represents a type/classname - both primitive and Class types.
	*  
	* @author MAX
	*
	* To change the template for this generated type comment go to
	* Window - Preferences - Java - Code Generation - Code and Comments
	*/
	public class ClassName
	{
		virtual public System.String FullyQualifiedName
		{
			get
			{
				return this.fullyQualifiedName;
			}
			
		}
		/// <summary>returns the atomar name for a class. 
		/// 
		/// java.util.Set -> "Set" 
		/// </summary>
		virtual public System.String Name
		{
			get
			{
				return this.name;
			}
			
		}
		/// <summary>returns the package name for a class/type. 
		/// 
		/// java.util.Set -> "java.util" and Foo -> ""
		/// </summary>
		/// <returns>
		/// </returns>
		virtual public System.String PackageName
		{
			get
			{
				return this.packageName;
			}
			
		}
		/// <summary>return true if this type is an array. Check is done by checking if the type ends with []. </summary>
		virtual public bool Array
		{
			get
			{
				return fullyQualifiedName.EndsWith("[]");
			}
			
		}
		/// <summary> Type is primitive if the basename (fqn without []) is in the PRIMITIVE set.</summary>
		/// <returns> boolean
		/// </returns>
		virtual public bool Primitive
		{
			get
			{
				System.String baseTypeName = StringHelper.Replace(fullyQualifiedName, "[]", "");
				return PRIMITIVES.Contains(baseTypeName);
			}
			
		}
		
		//UPGRADE_NOTE: Final was removed from the declaration of 'PRIMITIVES '. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1003"'
		internal static readonly SupportClass.SetSupport PRIMITIVES = new SupportClass.HashSetSupport();
		
		public ClassName(System.String fqn)
		{
			initFullyQualifiedName(fqn);
		}
		
		private System.String fullyQualifiedName = null;
		private System.String name = null;
		private System.String packageName = null;
		
		/// <summary>Two ClassName are equals if their fullyQualifiedName are the same/equals! </summary>
		public  override bool Equals(System.Object other)
		{
			ClassName otherClassName = (ClassName) other;
			return otherClassName.fullyQualifiedName.Equals(fullyQualifiedName);
		}
		
		public virtual bool inJavaLang()
		{
			return "java.lang".Equals(packageName);
		}
		
		public virtual bool inSamePackage(ClassName other)
		{
			return (System.Object) other.packageName == (System.Object) this.packageName || ((System.Object) other.packageName != null && other.packageName.Equals(this.packageName));
		}
		
		
		/*
		* Initialize the class fields with info from a fully qualified name.
		*/
		private void  initFullyQualifiedName(System.String fqn)
		{
			this.fullyQualifiedName = fqn;
			if (!Primitive)
			{
				if ((System.Object) fqn != null)
				{
					
					int lastDot = fqn.LastIndexOf(",");
					if (lastDot < 0)
					{
						name = fqn;
						packageName = null;
					}
					else
					{
						name = fqn.Substring(lastDot + 1);
						packageName = fqn.Substring(0, (lastDot) - (0));
					}
				}
				else
				{
					name = fqn;
					packageName = null;
				}
			}
			else
			{
				name = fqn;
				packageName = null;
			}
		}
		
		public override System.String ToString()
		{
			return FullyQualifiedName;
		}
		static ClassName()
		{
			{
				PRIMITIVES.Add("Byte");
				PRIMITIVES.Add("Short");
				PRIMITIVES.Add("Int32");
				PRIMITIVES.Add("Long");
				PRIMITIVES.Add("Float");
				PRIMITIVES.Add("Double");
				PRIMITIVES.Add("Char");
				PRIMITIVES.Add("Boolean");
				PRIMITIVES.Add("String");
				PRIMITIVES.Add("Ticks");
				PRIMITIVES.Add("TrueFalse");
				PRIMITIVES.Add("YesNo");
			}
		}
	}
}