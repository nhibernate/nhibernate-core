using System;
namespace NHibernate.tool.hbm2java
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
			paramList = new System.Collections.ArrayList();
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			throwsList = new System.Collections.ArrayList();
		}
		virtual public System.String Name
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
		virtual public System.String ReturnType
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
		virtual public System.String AccessModifier
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
		virtual public System.Collections.ArrayList ParamList
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
		
		private System.String name = "";
		private System.String returnType = "";
		private System.String accessModifier = "";
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'paramList' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private System.Collections.ArrayList paramList;
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: The initialization of  'throwsList' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		private System.Collections.ArrayList throwsList;
		
		public MethodSignatureBuilder(System.String methodName, System.String returnType, System.String accessModifier)
		{
			InitBlock();
			name = methodName;
			this.returnType = returnType;
			this.accessModifier = accessModifier;
		}
		
		public virtual System.String buildMethodSignature()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(accessModifier + " " + returnType + " " + name + "(");
			
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.size' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			for (int i = 0; i < paramList.Count; i++)
			{
				System.String param = (System.String) paramList[i];
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
				System.String thr = (System.String) throwsList[i];
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
		
		public virtual void  addParam(System.String param)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			this.paramList.Add(param);
		}
		
		public virtual void  addThrows(System.String throwsString)
		{
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.ArrayList.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			this.throwsList.Add(throwsString);
		}
	}
}