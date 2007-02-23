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
		public virtual string FullyQualifiedName
		{
			get { return this.fullyQualifiedName; }
		}

		/// <summary>returns the atomar name for a class. 
		/// 
		/// java.util.Set -> "Set" 
		/// </summary>
		public virtual string Name
		{
			get { return this.name; }
		}

		/// <summary>returns the package name for a class/type. 
		/// 
		/// java.util.Set -> "java.util" and Foo -> ""
		/// </summary>
		/// <returns>
		/// </returns>
		public virtual string PackageName
		{
			get { return this.packageName; }
		}

		/// <summary>return true if this type is an array. Check is done by checking if the type ends with []. </summary>
		public virtual bool Array
		{
			get { return fullyQualifiedName.EndsWith("[]"); }
		}

		/// <summary> Type is primitive if the basename (fqn without []) is in the PRIMITIVE set.</summary>
		/// <returns> boolean
		/// </returns>
		public virtual bool Primitive
		{
			get
			{
				string baseTypeName = StringHelper.Replace(fullyQualifiedName, "[]", "");
				return PRIMITIVES.Contains(baseTypeName);
			}
		}

		internal static readonly SupportClass.SetSupport PRIMITIVES = new SupportClass.HashSetSupport();

		public ClassName(string fqn)
		{
			initFullyQualifiedName(fqn);
		}

		private string fullyQualifiedName = null;
		private string name = null;
		private string packageName = null;

		/// <summary>Two ClassName are equals if their fullyQualifiedName are the same/equals! </summary>
		public override bool Equals(object other)
		{
			ClassName otherClassName = (ClassName) other;
			return otherClassName.fullyQualifiedName.Equals(fullyQualifiedName);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual bool inJavaLang()
		{
			return "System".Equals(packageName);
		}

		public virtual bool inSamePackage(ClassName other)
		{
			return
				(object) other.packageName == (Object) this.packageName ||
				((Object) other.packageName != null && other.packageName.Equals(this.packageName));
		}


		/*
		* Initialize the class fields with info from a fully qualified name.
		*/

		private void initFullyQualifiedName(string fqn)
		{
			AssemblyQualifiedTypeName tn = TypeNameParser.Parse(fqn);

			this.fullyQualifiedName = tn.Type;

			if (!Primitive)
			{
				int lastDot = fullyQualifiedName.LastIndexOf(".");
				if (lastDot < 0)
				{
					name = fullyQualifiedName;
					packageName = null;
				}
				else
				{
					name = fullyQualifiedName.Substring(lastDot + 1);
					packageName = fullyQualifiedName.Substring(0, lastDot);
				}
			}
			else
			{
				name = fullyQualifiedName;
				packageName = null;
			}
		}

		public override string ToString()
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