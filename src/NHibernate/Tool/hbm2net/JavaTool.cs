/*
* Created on 12-10-2003
*
* To change the template for this generated file go to
* Window - Preferences - Java - Code Generation - Code and Comments
*/
using System;
using StringHelper = NHibernate.Util.StringHelper;

namespace NHibernate.Tool.hbm2net
{
	
	/// <author>  MAX
	/// 
	/// To change the template for this generated type comment go to
	/// Window - Preferences - Java - Code Generation - Code and Comments
	/// </author>
	public class JavaTool
	{
		
		/// <summary> Returns "package packagename;" where packagename is either the declared packagename,
		/// or the one provide via meta attribute "generated-class".
		/// 
		/// Returns "// default package" if no package declarition available.
		/// 
		/// </summary>
		/// <param name="">cm
		/// </param>
		/// <returns>
		/// </returns>
		public virtual System.String getPackageDeclaration(ClassMapping cm)
		{
			if ((System.Object) cm.GeneratedPackageName != null)
			{
				return "namespace " + cm.GeneratedPackageName + "";
			}
			else
			{
				return "// default namespace";
			}
		}
		
		/// <summary> Returns the true name for the given fields class name. By true name is
		/// that it will return the Proxy for the class name if the class was
		/// defined with a proxy attribute.
		/// 
		/// If the Field has an <meta attribute="property-type"></meta> then that
		/// will overrule any other information.
		/// 
		/// </summary>
		/// <param name="field">class name that we use to serach in class2classmap
		/// </param>
		/// <param name="class2classmap">a map from classname to classmappings
		/// </param>
		/// <returns> String return either name or the proxy name of the classmap
		/// </returns>
		static public System.String getTrueTypeName(FieldProperty field, System.Collections.IDictionary class2classmap)
		{
			System.String name = (field.ClassType != null)?field.ClassType.FullyQualifiedName:field.FullyQualifiedTypeName;
			
			if (field.getMeta("property-type") != null)
			{
				name = field.getMetaAsString("property-type");
			}
			ClassMapping cmap = (ClassMapping) class2classmap[name];
			
			if (cmap != null)
			{
				if ((System.Object) cmap.Proxy != null)
				{
					return cmap.Proxy;
				}
			}
			return name;
		}
		
		public virtual System.String getTrueTypeName(ClassName cn, System.Collections.IDictionary class2classmap)
		{
			System.String name = cn.FullyQualifiedName;
			ClassMapping cmap = (ClassMapping) class2classmap[name];
			
			if (cmap != null)
			{
				if ((System.Object) cmap.Proxy != null)
				{
					return cmap.Proxy;
				}
			}
			return name;
		}
		
		/// <summary> Returns the last part of type if it is in the set of imports.
		/// e.g. java.util.Date would become Date, if imports contains 
		/// java.util.Date.
		/// 
		/// </summary>
		/// <param name="">type
		/// </param>
		/// <param name="">imports
		/// </param>
		/// <returns> String
		/// </returns>
		static public System.String shortenType(System.String type, SupportClass.SetSupport imports)
		{
			System.String result = type;
			if (imports.Contains(type))
			{
				result = type.Substring(type.LastIndexOf(StringHelper.Dot) + 1);
			}
			else if (type.EndsWith("[]"))
			{
				result = shortenType(type.Substring(0, (type.Length - 2) - (0)), imports) + "[]";
			}
			else if (type.StartsWith("java.lang."))
			{
				result = type.Substring("java.lang.".Length);
			}
			
			return result;
		}
		
		/// <summary> Convert string into something that can be rendered nicely into a javadoc
		/// comment.
		/// Prefix each line with a star ('*').
		/// </summary>
		/// <param name="">string
		/// </param>
		public virtual System.String toJavaDoc(System.String string_Renamed, int indent)
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			
			if ((System.Object) string_Renamed != null)
			{
				System.String[] lines = string_Renamed.Split('\n', '\r', '\f');
				for (int i = 0; i < lines.Length; i++)
				{
					System.String docline = "///" + lines[i] + "\n";
					for(int j = 0;j < indent;j++)
						docline = docline.Insert(0," ");
					result.Append(docline);
				}
			}
			
			return result.ToString();
		}
		
		public virtual bool hasExtends(ClassMapping cmap)
		{
			return (System.Object) getExtends(cmap) != string.Empty;
		}
		
		public virtual System.String getExtends(ClassMapping cmap)
		{
			System.String extendz = string.Empty;
			
			if (cmap.Interface)
			{
				if (cmap.SuperClassMapping != null && cmap.SuperClassMapping.Interface)
				{
					extendz += cmap.SuperClassMapping.Name;
				}
				if (cmap.getMeta(extendz) != null)
				{
					if ((System.Object) extendz != null)
					{
						extendz += ",";
					}
					extendz = cmap.getMetaAsString("extends");
				}
			}
			else if ((System.Object) cmap.SuperClass != null)
			{
				if (cmap.SuperClassMapping != null && cmap.SuperClassMapping.Interface)
				{
					// class cannot extend it's superclass because the superclass is marked as an interface
				}
				else
				{
					extendz = cmap.SuperClass;
				}
			}
			else if (cmap.getMeta("extends") != null)
			{
				extendz = cmap.getMetaAsString("extends");
			}
			
			return extendz;
		}
		
		public virtual bool hasImplements(ClassMapping cmap)
		{
			return (System.Object) getImplements(cmap) != null;
		}
		
		public virtual System.String getImplements(ClassMapping cmap)
		{
			SupportClass.ListCollectionSupport interfaces = new SupportClass.ListCollectionSupport();
			
			//			implement proxy, but NOT if the proxy is the class it self!
			if ((System.Object) cmap.Proxy != null && (!cmap.Proxy.Equals(cmap.FullyQualifiedName)))
			{
				interfaces.Add(cmap.Proxy);
			}
			
			if (!cmap.Interface)
			{
				if (cmap.SuperClassMapping != null && cmap.SuperClassMapping.Interface)
				{
					interfaces.Add(cmap.SuperClassMapping.ClassName.FullyQualifiedName);
				}
				if (cmap.getMeta("implements") != null)
				{
					interfaces.AddAll(cmap.getMeta("implements"));
				}
				interfaces.Add(typeof(System.Runtime.Serialization.ISerializable).FullName);
			}
			else
			{
				// interfaces can't implement suff
			}
			
			
			if (interfaces.Count > 0)
			{
				System.Text.StringBuilder sbuf = new System.Text.StringBuilder();
				bool first = true;
				for (System.Collections.IEnumerator iter = interfaces.GetEnumerator(); iter.MoveNext(); )
				{
					if (first)
						first = false;
					else
						sbuf.Append(",");
					sbuf.Append(JavaTool.shortenType(iter.Current.ToString(), cmap.Imports));
				}
				return sbuf.ToString();
			}
			else
			{
				return string.Empty;
			}
		}
		
		public virtual System.String fieldsAsParameters(SupportClass.ListCollectionSupport fieldslist, ClassMapping classMapping, System.Collections.IDictionary class2classmap)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			bool first = true;
			for (System.Collections.IEnumerator fields = fieldslist.GetEnumerator(); fields.MoveNext(); )
			{
				if (first)
					first = false;
				else
					buf.Append(", ");
				FieldProperty field = (FieldProperty) fields.Current;
				buf.Append(JavaTool.shortenType(JavaTool.getTrueTypeName(field, class2classmap), classMapping.Imports) + " " + field.FieldName);
			}
			return buf.ToString();
		}
		
		public virtual System.String fieldsAsArguments(SupportClass.ListCollectionSupport fieldslist, ClassMapping classMapping, System.Collections.IDictionary class2classmap)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			bool first = true;
			for (System.Collections.IEnumerator fields = fieldslist.GetEnumerator(); fields.MoveNext(); )
			{
				if (first)
					first = false;
				else
					buf.Append(", ");

				FieldProperty field = (FieldProperty) fields.Current;
				buf.Append(field.FieldName);
			}
			return buf.ToString();
		}
		
		public virtual System.String genImports(ClassMapping classMapping)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			
			for (System.Collections.IEnumerator imports = classMapping.Imports.GetEnumerator(); imports.MoveNext(); )
			{
				buf.Append("using " + imports.Current + ";\n");
			}
			
			
			SupportClass.ListCollectionSupport imports2 = classMapping.getMeta("extra-import");
			if (imports2 != null)
			{
				for (System.Collections.IEnumerator it = imports2.GetEnumerator(); it.MoveNext(); )
				{
					System.String cname = it.Current.ToString();
					buf.Append("using " + cname + ";\n");
				}
			}
			return buf.ToString();
		}
	}
}