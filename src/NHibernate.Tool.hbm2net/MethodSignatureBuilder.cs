using System;
using System.Collections;
using System.Text;

namespace NHibernate.Tool.hbm2net
{
	
	/// <summary> Build method signatures given lots of parameters
	/// Date: Apr 15, 2003
	/// Time: 7:30:09 PM
	/// </summary>
	/// <author>  Matt Hall (matt2k(at)users.sf.net)
	/// </author>
	public class MethodSignatureBuilder
	{
		private void  InitBlock()
		{
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			paramList = new ArrayList();
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			throwsList = new ArrayList();
		}
		virtual public String Name
		{
			get
			{
				return name;
			}
			
			set
			{
				this.name = value;
			}
			
		}
		virtual public String ReturnType
		{
			get
			{
				return returnType;
			}
			
			set
			{
				this.returnType = value;
			}
			
		}
		virtual public String AccessModifier
		{
			get
			{
				return accessModifier;
			}
			
			set
			{
				this.accessModifier = value;
			}
			
		}
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		virtual public ArrayList ParamList
		{
			get
			{
				return paramList;
			}
			
			set
			{
				this.paramList = value;
			}
			
		}
		
		private String name = "";
		private String returnType = "";
		private String accessModifier = "";
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'paramList' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private ArrayList paramList;
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'throwsList' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private ArrayList throwsList;
		
		public MethodSignatureBuilder(String methodName, String returnType, String accessModifier)
		{
			InitBlock();
			name = methodName;
			this.returnType = returnType;
			this.accessModifier = accessModifier;
		}
		
		public virtual String buildMethodSignature()
		{
			StringBuilder sb = new StringBuilder(accessModifier + " " + returnType + " " + name + "(");
			
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			for (int i = 0; i < paramList.Count; i++)
			{
				String param = (String) paramList[i];
				sb.Append(param);
				
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				if (i < paramList.Count - 1)
				{
					sb.Append(", ");
				}
			}
			sb.Append(") ");
			
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			for (int i = 0; i < throwsList.Count; i++)
			{
				if (i == 0)
				{
					sb.Append(" throws ");
				}
				String thr = (String) throwsList[i];
				sb.Append(thr);
				
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				if (i < throwsList.Count - 1)
				{
					sb.Append(", ");
				}
			}
			
			sb.Append(" {");
			return sb.ToString();
		}
		
		public virtual void  addParam(String param)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			this.paramList.Add(param);
		}
		
		public virtual void  addThrows(String throwsString)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			this.throwsList.Add(throwsString);
		}
	}
}