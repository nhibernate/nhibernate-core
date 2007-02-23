using System;
using System.Collections;
using System.Text;

using NHibernate.Util;

namespace NHibernate.Tool.hbm2net
{
	/// <author>  MAX
	/// 
	/// To change the template for this generated type comment go to
	/// Window - Preferences - Java - Code Generation - Code and Comments
	/// </author>
	public class LanguageTool
	{
		/// <summary> Returns "package packagename;" where packagename is either the declared packagename,
		/// or the one provide via meta attribute "generated-class".
		/// 
		/// Returns "// default package" if no package declarition available.
		/// 
		/// </summary>
		/// <returns>
		/// </returns>
		public virtual string getPackageDeclaration(ClassMapping cm)
		{
			if ((object) cm.GeneratedPackageName != null)
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
		/// <returns> string return either name or the proxy name of the classmap
		/// </returns>
		public static string getTrueTypeName(FieldProperty field, IDictionary class2classmap)
		{
			string name = (field.ClassType != null) ? field.ClassType.FullyQualifiedName : field.FullyQualifiedTypeName;

			if (field.getMeta("property-type") != null)
			{
				name = field.getMetaAsString("property-type");
			}
			ClassMapping cmap = (ClassMapping) class2classmap[name];

			if (cmap != null)
			{
				if ((Object) cmap.Proxy != null)
				{
					return cmap.Proxy;
				}
			}
			return name;
		}

		public virtual string getTrueTypeName(ClassName cn, IDictionary class2classmap)
		{
			string name = cn.FullyQualifiedName;
			ClassMapping cmap = (ClassMapping) class2classmap[name];

			if (cmap != null)
			{
				if ((Object) cmap.Proxy != null)
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
		/// <returns> String
		/// </returns>
		public static string shortenType(string type, SupportClass.SetSupport imports)
		{
			string result = type;
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
		public virtual string toJavaDoc(string string_Renamed, int indent)
		{
			StringBuilder result = new StringBuilder();
			string padding = new String(' ', indent);

			if (string_Renamed != null)
			{
				string[] lines = string_Renamed.Split('\n', '\r', '\f');
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines[i].Length > 0)
					{
						result.Append(padding).Append("/// ").Append(lines[i]).Append("\n");
					}
					//string docline = "///" + lines[i] + "\n";
					//for(int j = 0;j < indent;j++)
					//	docline = docline.Insert(0," ");
					//result.Append(docline);
				}
			}
			int len = result.ToString().Length;
			return result.ToString().Substring(0, len - 2);
		}

		public virtual bool hasExtends(ClassMapping cmap)
		{
			return getExtends(cmap).Length != 0;
		}

		public virtual string getExtends(ClassMapping cmap)
		{
			string extendz = string.Empty;

			if (cmap.Interface)
			{
				if (cmap.SuperClassMapping != null && cmap.SuperClassMapping.Interface)
				{
					extendz += cmap.SuperClassMapping.Name;
				}
				if (cmap.getMeta(extendz) != null)
				{
					if ((Object) extendz != null)
					{
						extendz += ",";
					}
					extendz = cmap.getMetaAsString("extends");
				}
			}
			else if ((Object) cmap.SuperClass != null)
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
			return getImplements(cmap).Length != 0;
		}

		public virtual string getImplements(ClassMapping cmap)
		{
			SupportClass.ListCollectionSupport interfaces = new SupportClass.ListCollectionSupport();

			//			implement proxy, but NOT if the proxy is the class it self!
			if ((Object) cmap.Proxy != null && (!cmap.Proxy.Equals(cmap.FullyQualifiedName)))
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
				//interfaces.Add(typeof(System.Runtime.Serialization.ISerializable).FullName);
			}
			else
			{
				// interfaces can't implement suff
			}


			if (interfaces.Count > 0)
			{
				StringBuilder sbuf = new StringBuilder();
				bool first = true;
				for (IEnumerator iter = interfaces.GetEnumerator(); iter.MoveNext();)
				{
					if (first)
						first = false;
					else
						sbuf.Append(",");
					sbuf.Append(shortenType(iter.Current.ToString(), cmap.Imports));
				}
				return sbuf.ToString();
			}
			else
			{
				return string.Empty;
			}
		}

		public virtual string fieldsAsParameters(SupportClass.ListCollectionSupport fieldslist, ClassMapping classMapping,
		                                         IDictionary class2classmap)
		{
			StringBuilder buf = new StringBuilder();
			bool first = true;
			for (IEnumerator fields = fieldslist.GetEnumerator(); fields.MoveNext();)
			{
				if (first)
					first = false;
				else
					buf.Append(", ");
				FieldProperty field = (FieldProperty) fields.Current;
				buf.Append(shortenType(field.FullyQualifiedTypeName, classMapping.Imports) + " " + field.fieldcase);
			}
			return buf.ToString();
		}

		public virtual string fieldsAsArguments(SupportClass.ListCollectionSupport fieldslist, ClassMapping classMapping,
		                                        IDictionary class2classmap)
		{
			StringBuilder buf = new StringBuilder();
			bool first = true;
			for (IEnumerator fields = fieldslist.GetEnumerator(); fields.MoveNext();)
			{
				if (first)
					first = false;
				else
					buf.Append(", ");

				FieldProperty field = (FieldProperty) fields.Current;
				buf.Append(field.fieldcase);
			}
			return buf.ToString();
		}

		public virtual string genImports(ClassMapping classMapping)
		{
			StringBuilder buf = new StringBuilder();

			for (IEnumerator imports = classMapping.Imports.GetEnumerator(); imports.MoveNext();)
			{
				buf.Append("using " + imports.Current + ";\n");
			}


			SupportClass.ListCollectionSupport imports2 = classMapping.getMeta("extra-import");
			if (imports2 != null)
			{
				for (IEnumerator it = imports2.GetEnumerator(); it.MoveNext();)
				{
					string cname = it.Current.ToString();
					buf.Append("using " + cname + ";\n");
				}
			}
			return buf.ToString();
		}
	}
}