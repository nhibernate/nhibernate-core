/*
* Created on 25-03-2003
*
* To change this generated comment go to 
* Window>Preferences>Java>Code Generation>Code Template
*/
using System;
namespace NHibernate.tool.hbm2net
{
	
	/// <author>  max
	/// </author>
	public abstract class AbstractRenderer : Renderer
	{
		
		public virtual void  render(System.String savedToPackage, System.String savedToClass, ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StreamWriter writer)
		{
		}

		internal System.Collections.Specialized.NameValueCollection properties;
		
		public virtual void  configure(System.Collections.Specialized.NameValueCollection props)
		{
			this.properties = props;
		}
		
		public virtual System.String getFieldScope(FieldProperty field, System.String localScopeName, System.String defaultScope)
		{
			return field.getScope(localScopeName, defaultScope);
		}
		
		public virtual System.String getPackageDeclaration(System.String savedToPackage, ClassMapping classMapping)
		{
			if ((System.Object) savedToPackage != null && !savedToPackage.Trim().Equals(""))
			{
				return "namespace " + savedToPackage + "";
			}
			else if ((System.Object) classMapping.GeneratedPackageName != null)
			{
				return "namespace " + classMapping.GeneratedPackageName + "";
			}
			return "";
		}
		
		protected internal virtual void  genPackageDelaration(System.String savedToPackage, ClassMapping classMapping, System.IO.StreamWriter w)
		{
			System.String string_Renamed = getPackageDeclaration(savedToPackage, classMapping);
			if (string_Renamed.Length > 0)
			{
				w.WriteLine(string_Renamed);
			}
			else
			{
				w.WriteLine("// default package");
			}
		}
		
		public virtual System.String getSaveToClassName(ClassMapping classMapping)
		{
			return classMapping.GeneratedName;
		}
		
		public virtual System.String getSaveToPackage(ClassMapping classMapping)
		{
			return classMapping.GeneratedPackageName;
		}
	}
}