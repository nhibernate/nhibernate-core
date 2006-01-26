using System;
using System.Collections;
using System.Text;
using Type = NHibernate.Type.TypeType;
namespace NHibernate.Tool.hbm2net
{
	
	/// <summary> Build queries for use in finder generation.</summary>
	/// <author>  Matt Hall (matt2k(at)users.sf.net)
	/// </author>
	public class QueryBuilder
	{
		private void  InitBlock()
		{
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			objects = new ArrayList();
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			joinConditions = new ArrayList();
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			criteria = new ArrayList();
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			params_Renamed = new SupportClass.ListCollectionSupport();
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			criteriaParamTypes = new SupportClass.ListCollectionSupport();
		}
		virtual public ClassMapping LocalClass
		{
			set
			{
				this.localClass = value;
			}
			
		}
		/// <returns> The query in string form
		/// </returns>
		virtual public string Query
		{
			get
			{
				StringBuilder sb = new StringBuilder("select ");
				
				// Foreign class is what we're selecting from
				sb.Append(foreignClass.Name.ToLower() + " from ");
				sb.Append(foreignClass.Name.ToLower() + " in class ");
				sb.Append(foreignClass.Name + ", ");
				
				// Now the collections stuff based on the local class
				sb.Append(localClass.Name.ToLower() + " in ");
				sb.Append(foreignClass.Name.ToLower() + ".");
				sb.Append(joinFieldName + ".elements where ");
				
				// The join back to the local class
				sb.Append(localClass.Name.ToLower() + "=? and ");
				
				/*
				if (objects.size() > 0) {
				sb.append(" from ");
				for (int i = 0; i < objects.size(); i++) {
				ClassMapping classMapping = (ClassMapping) objects.get(i);
				sb.append(classMapping.getCanonicalName() + " " + classMapping.getName().toLowerCase());
				sb.append(" ");
				}
				}*/
				
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				if (criteria.Count > 0)
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					for (int i = 0; i < criteria.Count; i++)
					{
						string thisCriteria = (String) criteria[i];
						sb.Append(" " + thisCriteria + " ");
						//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
						if (i < criteria.Count - 1)
						{
							sb.Append(" and ");
						}
					}
				}
				
				return sb.ToString();
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport ParamTypes
		{
			get
			{
				return criteriaParamTypes;
			}
			
		}
		virtual public string ParamTypesAsString
		{
			get
			{
				string types = "new Type[] {";
				// Always need the local class as an association type
				types += ("Hibernate.association(" + localClass.Name + ".class), ");
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				for (int i = 0; i < criteriaParamTypes.Count; i++)
				{
					string s = (String) criteriaParamTypes[i];
					types += s;
					//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					if (i != criteriaParamTypes.Count - 1)
					{
						types += ",";
					}
				}
				return types + "}";
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public SupportClass.ListCollectionSupport Params
		{
			get
			{
				return params_Renamed;
			}
			
		}
		virtual public string ParamsAsString
		{
			get
			{
				string types = "new Object[] {";
				// Always joining via the local class
				types += (localClass.Name.ToLower() + ", ");
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				for (int i = 0; i < params_Renamed.Count; i++)
				{
					string s = (String) params_Renamed[i];
					types += s;
					//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					if (i != params_Renamed.Count - 1)
					{
						types += ",";
					}
				}
				return types + "}";
			}
			
		}
		
		public const string CRITERIA_EQUALS = "=";
		public const string CRITERIA_GREATER_THAN = ">";
		public const string CRITERIA_LESS_THAN = "<";
		public const string CRITERIA_LIKE = "LIKE";
		
		// List of strings that will later be put together to form the query
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'objects' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private ArrayList objects;
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'joinConditions' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private ArrayList joinConditions;
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'criteria' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private ArrayList criteria;
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'params_Renamed' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private SupportClass.ListCollectionSupport params_Renamed;
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'criteriaParamTypes' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private SupportClass.ListCollectionSupport criteriaParamTypes;
		
		private ClassMapping localClass = null;
		private ClassMapping foreignClass = null;
		private string joinFieldName = "";
		
		public QueryBuilder()
		{
			InitBlock();
		}
		
		public virtual void  setForeignClass(ClassName foreignClass, IDictionary classMappings, string joinFieldName)
		{
			//UPGRADE_TODO: Method 'java.util.Map.get' was converted to 'System.Collections.IDictionary.Item' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilMapget_javalangObject"'
			ClassMapping classMapToAdd = (ClassMapping) classMappings[foreignClass.FullyQualifiedName];
			this.foreignClass = classMapToAdd;
			this.joinFieldName = joinFieldName;
		}
		
		public virtual void  addCritera(ClassMapping criteriaClass, FieldProperty field, string condition)
		{
			string newCritera = criteriaClass.Name.ToLower() + "." + field.FieldName + condition + "?";
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			params_Renamed.Add(FinderRenderer.getFieldAsObject(false, field));
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			criteria.Add(newCritera);
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			criteriaParamTypes.Add(FinderRenderer.getFieldAsHibernateType(false, field));
		}
	}
}