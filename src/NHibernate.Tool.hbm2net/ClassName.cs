using System;
using NHibernate.Util;

namespace NHibernate.Tool.hbm2net
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
		virtual public String FullyQualifiedName
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
		virtual public String Name
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
		virtual public String PackageName
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
				String baseTypeName = StringHelper.Replace(fullyQualifiedName, "[]", "");
				return PRIMITIVES.Contains(baseTypeName);
			}
			
		}
		
		internal static readonly SupportClass.SetSupport PRIMITIVES = new SupportClass.HashSetSupport();
		
		public ClassName(String fqn)
		{
			initFullyQualifiedName(fqn);
		}
		
		private String fullyQualifiedName = null;
		private String name = null;
		private String packageName = null;
		
		/// <summary>Two ClassName are equals if their fullyQualifiedName are the same/equals! </summary>
		public  override bool Equals(Object other)
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
			return (Object) other.packageName == (Object) this.packageName || ((Object) other.packageName != null && other.packageName.Equals(this.packageName));
		}
		
		
		/*
		* Initialize the class fields with info from a fully qualified name.
		*/
		private void  initFullyQualifiedName(String fqn)
		{
			this.fullyQualifiedName = fqn;
			if (fullyQualifiedName.IndexOf(",")>0)
				fullyQualifiedName = fullyQualifiedName.Substring(0,fullyQualifiedName.IndexOf(","));
			if (!Primitive)
			{
				if ((Object) fqn != null)
				{
					
					int lastDot = fqn.LastIndexOf(",");
					if (lastDot < 0)
					{
						name = fqn;
						packageName = null;
					}
					else
					{
						packageName = fqn.Substring(0, (lastDot) - (0));
						name = packageName.Substring(packageName.LastIndexOf(".") + 1);
						packageName = packageName.Substring(0, (packageName.LastIndexOf(".")) - (0));
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
		
		public override String ToString()
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