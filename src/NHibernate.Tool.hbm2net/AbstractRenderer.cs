/*
* Created on 25-03-2003
*
* To change this generated comment go to 
* Window>Preferences>Java>Code Generation>Code Template
*/
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace NHibernate.Tool.hbm2net
{
	
	/// <author>  max
	/// </author>
	public abstract class AbstractRenderer : Renderer
	{
		
		public virtual void  render(String savedToPackage, String savedToClass, ClassMapping classMapping, IDictionary class2classmap, StreamWriter writer)
		{
		}

		internal NameValueCollection properties;
		
		public virtual void  configure(NameValueCollection props)
		{
			this.properties = props;
		}
		
		public virtual String getFieldScope(FieldProperty field, String localScopeName, String defaultScope)
		{
			return field.getScope(localScopeName, defaultScope);
		}
		
		public virtual String getPackageDeclaration(String savedToPackage, ClassMapping classMapping)
		{
			if ((Object) savedToPackage != null && !savedToPackage.Trim().Equals(""))
			{
				return "namespace " + savedToPackage + "";
			}
			else if ((Object) classMapping.GeneratedPackageName != null)
			{
				return "namespace " + classMapping.GeneratedPackageName + "";
			}
			return "";
		}
		
		protected internal virtual void  genPackageDelaration(String savedToPackage, ClassMapping classMapping, StreamWriter w)
		{
			String string_Renamed = getPackageDeclaration(savedToPackage, classMapping);
			if (string_Renamed.Length > 0)
			{
				w.WriteLine(string_Renamed);
			}
			else
			{
				w.WriteLine("// default package");
			}
		}
		
		public virtual String getSaveToClassName(ClassMapping classMapping)
		{
			return classMapping.GeneratedName;
		}
		
		public virtual String getSaveToPackage(ClassMapping classMapping)
		{
			return classMapping.GeneratedPackageName;
		}
	}
}