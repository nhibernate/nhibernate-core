using System;
namespace NHibernate.tool.hbm2net
{
	
	/// <summary>  Renderer that generates a StateLess-SessionBean per entity.
	/// Consider this more an example of how to write a renderer, than how use Hibernate with sessionbeans! 
	/// 
	/// Uses meta attribute jndi-name and initial-context for generating the xdoclet remarks and initial context lookup.
	/// Inspired by postings by carniz at Hibernate forum:
	/// 
	/// The basic idea is:
	/// - one SLSB per persistent class
	/// - each SLSB is a "service" for adding, removing, updating and finding objects of the actual class
	/// 
	/// 
	/// </summary>
	/// <author>  MAX
	/// 
	/// </author>
	public class SLSBperEntityRenderer:AbstractRenderer
	{
		
		/* (non-Javadoc)
		* @see NHibernate.tool.hbm2net.Renderer#render(java.lang.String, java.lang.String, NHibernate.tool.hbm2net.ClassMapping, java.util.Map, java.io.PrintWriter)
		*/
		public override void  render(System.String savedToPackage, System.String savedToClass, ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StreamWriter mainwriter)
		{
			
			genPackageDelaration(savedToPackage, classMapping, mainwriter);
			mainwriter.WriteLine();
			
			// switch to another writer to be able to insert the actually
			// used imports when whole class has been rendered. 
			System.IO.StringWriter writer = new System.IO.StringWriter();
			
			System.String jndiName = classMapping.getMetaAsString("jndi-name");
			System.String initialContext = classMapping.getMetaAsString("initial-context");
			System.String classname = classMapping.Name;
			System.String serviceName = classname + "Service";
			
			writer.WriteLine("/**");
			writer.WriteLine("* @ejb:bean name=\"" + serviceName + "\"");
			writer.WriteLine("*  jndi-name=\"" + jndiName + "\"");
			writer.WriteLine("*  type=\"Stateless\"");
			writer.WriteLine("**/");
			
			writer.WriteLine("public abstract class " + savedToClass + " implements SessionBean {");
			writer.WriteLine();
			writer.WriteLine("    SessionContext sessionContext;");
			writer.WriteLine("    SessionFactory sessionFactory;");
			writer.WriteLine("    Session session;");
			
			
			writer.WriteLine("    public void setSessionContext(SessionContext sessionContext) throws EJBException, RemoteException {");
			writer.WriteLine("        this.sessionContext = sessionContext;");
			writer.WriteLine("        try {");
			writer.WriteLine("            sessionFactory = (SessionFactory) new InitialContext().lookup(\"" + initialContext + "\");");
			writer.WriteLine("            }");
			writer.WriteLine("        catch (NamingException ne) {");
			writer.WriteLine("            throw new EJBException(ne.getExplanation(), ne);");
			writer.WriteLine("        }");
			writer.WriteLine("    }");
			
			System.String localName = classname.ToLower();
			
			writer.WriteLine("    /**");
			writer.WriteLine("     * @ejb:interface-method");
			writer.WriteLine("     *  tview-type=\"remote\"");
			writer.WriteLine("     **/");
			writer.WriteLine("     public void add" + classname + "(" + classname + " " + localName + ") throws HibernateException {");
			writer.WriteLine("       try {");
			writer.WriteLine("          session = sessionFactory.openSession();");
			writer.WriteLine("          session.save(" + localName + ");");
			writer.WriteLine("            session.flush();");
			writer.WriteLine("        }");
			writer.WriteLine("        catch (HibernateException he) {");
			writer.WriteLine("            sessionContext.setRollbackOnly();");
			writer.WriteLine("            throw he;");
			writer.WriteLine("        }");
			writer.WriteLine("        finally {");
			writer.WriteLine("            if (session != null)");
			writer.WriteLine("                try {");
			writer.WriteLine("                    session.close();");
			writer.WriteLine("                }");
			writer.WriteLine("                catch (Exception e) {   // ignore");
			writer.WriteLine("                }");
			writer.WriteLine("        }");
			writer.WriteLine("    }");
			
			writer.WriteLine("    /**");
			writer.WriteLine("     * @ejb:interface-method");
			writer.WriteLine("     *  tview-type=\"remote\"");
			writer.WriteLine("     **/");
			writer.WriteLine("     public void delete" + classname + "(" + classname + " " + localName + ") throws HibernateException {");
			writer.WriteLine("       try {");
			writer.WriteLine("          session = sessionFactory.openSession();");
			writer.WriteLine("          session.delete(" + localName + ");");
			writer.WriteLine("          session.flush();");
			writer.WriteLine("        }");
			writer.WriteLine("        catch (HibernateException he) {");
			writer.WriteLine("            sessionContext.setRollbackOnly();");
			writer.WriteLine("            throw he;");
			writer.WriteLine("        }");
			writer.WriteLine("        finally {");
			writer.WriteLine("            if (session != null)");
			writer.WriteLine("                try {");
			writer.WriteLine("                    session.close();");
			writer.WriteLine("                }");
			writer.WriteLine("                catch (Exception e) {   // ignore");
			writer.WriteLine("                }");
			writer.WriteLine("        }");
			writer.WriteLine("    }");
			
			localName = "idValue";
			System.String identifierType = null;
			
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport list = classMapping.AllFields;
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator fields = list.GetEnumerator(); fields.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				FieldProperty element = (FieldProperty) fields.Current;
				if (element.Identifier)
				{
					if ((System.Object) identifierType == null)
					{
						identifierType = element.FullyQualifiedTypeName;
					}
					else
					{
						throw new System.SystemException("Entities with multiple properties for fields are not supported in SLSBperEntity renderer.");
					}
				}
			}
			writer.WriteLine("    /**");
			writer.WriteLine("     * @ejb:interface-method");
			writer.WriteLine("     *  tview-type=\"remote\"");
			writer.WriteLine("     **/");
			writer.WriteLine("     public " + classname + " findById" + classname + "(" + identifierType + " " + localName + ") throws HibernateException {");
			writer.WriteLine("       " + classname + " result = null;");
			writer.WriteLine("       try {");
			writer.WriteLine("          session = sessionFactory.openSession();");
			writer.WriteLine("          session.load(" + classname + ".class, " + localName + ");");
			writer.WriteLine("        }");
			writer.WriteLine("        catch (HibernateException he) {");
			writer.WriteLine("            sessionContext.setRollbackOnly();");
			writer.WriteLine("            throw he;");
			writer.WriteLine("        }");
			writer.WriteLine("        finally {");
			writer.WriteLine("            if (session != null)");
			writer.WriteLine("                try {");
			writer.WriteLine("                    session.close();");
			writer.WriteLine("                }");
			writer.WriteLine("                catch (Exception e) {   // ignore");
			writer.WriteLine("               }");
			writer.WriteLine("        }");
			writer.WriteLine("        return result;");
			writer.WriteLine("       }");
			
			writer.WriteLine("    /**");
			writer.WriteLine("     * @ejb:interface-method");
			writer.WriteLine("     *  tview-type=\"remote\"");
			writer.WriteLine("     **/");
			writer.WriteLine("     public List findAll() throws HibernateException {");
			writer.WriteLine("       List result = null;");
			writer.WriteLine("       try {");
			writer.WriteLine("          session = sessionFactory.openSession();");
			writer.WriteLine("          session.find(\"from " + classname.ToLower() + " in class " + classname + ".class.getName() + \");");
			writer.WriteLine("          return result;");
			writer.WriteLine("        }");
			writer.WriteLine("        catch (HibernateException he) {");
			writer.WriteLine("            sessionContext.setRollbackOnly();");
			writer.WriteLine("            throw he;");
			writer.WriteLine("        }");
			writer.WriteLine("        finally {");
			writer.WriteLine("            if (session != null)");
			writer.WriteLine("                try {");
			writer.WriteLine("                    session.close();");
			writer.WriteLine("                }");
			writer.WriteLine("                catch (Exception e) {   // ignore");
			writer.WriteLine("               }");
			writer.WriteLine("        }");
			writer.WriteLine("      }");
			writer.WriteLine("}");
			
			// finally write the imports
			doImports(classMapping, mainwriter);
			mainwriter.Write(writer.ToString());
		}
		
		/// <param name="">classMapping
		/// </param>
		/// <param name="">mainwriter
		/// </param>
		private void  doImports(ClassMapping classMapping, System.IO.StreamWriter writer)
		{
			writer.WriteLine("import " + classMapping.FullyQualifiedName + ";");
			writer.WriteLine("import java.rmi.RemoteException;");
			writer.WriteLine("import java.util.List;");
			writer.WriteLine("import javax.ejb.SessionBean;");
			writer.WriteLine("import javax.ejb.SessionContext;");
			writer.WriteLine("import javax.ejb.EJBException;");
			
			writer.WriteLine("import NHibernate.SessionFactory;");
			writer.WriteLine("import NHibernate.HibernateException;");
			writer.WriteLine("import NHibernate.Session;");
			
			writer.WriteLine("import javax.naming.InitialContext;");
			writer.WriteLine("import javax.naming.NamingException;");
		}
	}
}