using System;
using System.Collections;
using System.IO;
using System.Reflection;

using log4net;

namespace NHibernate.Tool.hbm2net
{
	/// <summary> <p>Title: Basic Finder Generator for Hibernate 2</p>
	/// <p>Description: Generate basic finders for hibernate properties.
	/// This requires two things in the hbm.xml files.
	/// 
	/// The first is an indication of which fields you want to generate finders for.
	/// You indicate that with a meta block inside a property tag such as
	/// 
	/// <property name="name" column="name" type="string">
	/// <meta attribute="finder-method">findByName</meta>
	/// </property>
	/// 
	/// The finder method name will be the text enclosed in the meta tags.
	/// 
	/// If you want to generate a finder based on a join you can do something like this:
	/// 
	/// <set name="games" inverse="true" lazy="true" table="GamePlayers">
	/// <meta attribute="foreign-finder-name">findSavedGames</meta>
	/// <meta attribute="foreign-finder-field">save</meta>
	/// <meta attribute="foreign-join-field">players</meta>
	/// <key column="playerID"/>
	/// <many-to-many class="com.whatever.Game" column="gameID"/>
	/// </set>
	/// 
	/// Where foreign-finder-name will be the name of the finder when generated, foreign-finder-field is the field in
	/// the foreign class that you will want as a paramter to the finder (the criteria in the query) and foreign-join-field
	/// is the field in teh foreign class that joins to this object (in case there are more than one collection of these
	/// objects in the foreign class).
	/// 
	/// After you've defined your finders, the second thing to do is to create a config file for hbm2net of the format:
	/// 
	/// <codegen>
	/// <generate renderer="NHibernate.Tool.hbm2net.BasicRenderer"/>
	/// <generate suffix="Finder" renderer="NHibernate.Tool.hbm2net.FinderRenderer"/>
	/// </codegen>
	/// 
	/// And then use the param to hbm2net --config=xxx.xml where xxx.xml is the config file you
	/// just created.
	/// 
	/// An optional parameter is meta tag at the class level of the format:
	/// 
	/// <meta attribute="session-method">com.whatever.SessionTable.getSessionTable().getSession();</meta>
	/// 
	/// Which would be the way in which you get sessions if you use the Thread Local Session pattern
	/// like I do.
	/// </p>
	/// <p>Copyright: Copyright (c) 2003</p>
	/// </summary>
	/// <author>  Matt Hall (matt2k(at)users.sf.net)
	/// </author>
	/// <author>  Max Rydahl Andersen (small adjustments and bugfixes)
	/// </author>
	/// <version>  1.0
	/// </version>
	public class FinderRenderer : AbstractRenderer
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public FinderRenderer()
		{
			InitBlock();
		}

		private void InitBlock()
		{
			object tempObject;
			tempObject = "Character";
			primitiveToObject["char"] = tempObject;

			object tempObject2;
			tempObject2 = "Byte";
			primitiveToObject["byte"] = tempObject2;

			object tempObject3;
			tempObject3 = "Short";
			primitiveToObject["short"] = tempObject3;

			object tempObject4;
			tempObject4 = "Integer";
			primitiveToObject["int"] = tempObject4;

			object tempObject5;
			tempObject5 = "Long";
			primitiveToObject["long"] = tempObject5;

			object tempObject6;
			tempObject6 = "Boolean";
			primitiveToObject["boolean"] = tempObject6;

			object tempObject7;
			tempObject7 = "Float";
			primitiveToObject["float"] = tempObject7;

			object tempObject8;
			tempObject8 = "Double";
			primitiveToObject["double"] = tempObject8;

			hibType["char"] = "Hibernate.CHARACTER";

			hibType["byte"] = "Hibernate.BYTE";
			tempObject3 = "Hibernate.SHORT";
			hibType["short"] = "Hibernate.SHORT";
			tempObject4 = "Hibernate.INTEGER";
			hibType["int"] = tempObject4;
			tempObject5 = "Hibernate.LONG";
			hibType["long"] = tempObject5;
			tempObject6 = "Hibernate.INTEGER";
			hibType["Integer"] = tempObject6;

			tempObject7 = "Hibernate.BOOLEAN";
			hibType["boolean"] = tempObject7;

			tempObject8 = "Hibernate.FLOAT";
			hibType["float"] = tempObject8;
			object tempObject9;
			tempObject9 = "Hibernate.DOUBLE";
			hibType["double"] = tempObject9;

			object tempObject10;
			tempObject10 = "Hibernate.STRING";
			hibType["String"] = tempObject10;

			object tempObject11;
			tempObject11 = "Hibernate.LOCALE";
			hibType["Locale"] = tempObject11;
		}

		private const string MT_FINDERMETHOD = "finder-method";
		private const string MT_FOREIGNFINDERMETHOD = "foreign-finder-name";
		private const string MT_FOREIGNFINDERFIELD = "foreign-finder-field";
		private const string MT_FOREIGNJOINFIELD = "foreign-join-field";


		/// <summary> Render finder classes.</summary>
		/// <exception cref="Exception">Exception</exception>
		public override void render(string savedToPackage, string savedToClass, ClassMapping classMapping,
		                            IDictionary class2classmap, StreamWriter mainwriter)
		{
			genPackageDelaration(savedToPackage, classMapping, mainwriter);
			mainwriter.WriteLine();

			// switch to another writer to be able to insert the actually
			// used imports when whole class has been rendered.
			StringWriter writer = new StringWriter();

			writer.WriteLine("/** Automatically generated Finder class for " + savedToClass + ".\n" +
			                 " * @author Hibernate FinderGenerator " + " **/");

			string classScope = "public";
			writer.Write(classScope + " class " + savedToClass);

			// always implements Serializable
			writer.Write(" implements Serializable");

			writer.WriteLine(" {");
			writer.WriteLine();

			// switch to another writer to be able to insert the
			// veto- and changeSupport fields
			StringWriter propWriter = new StringWriter();

			doFinders(classMapping, class2classmap, propWriter);

			propWriter.WriteLine("}");

			writer.Write(propWriter.ToString());

			// finally write the imports
			doImports(classMapping, mainwriter);
			mainwriter.Write(writer.ToString());
		}


		/// <summary>  Create finders for properties that have the <meta atttribute="finder-method">
		/// finderName</meta> block defined. Also, create a findAll(Session) method.
		/// 
		/// </summary>
		public virtual void doFinders(ClassMapping classMapping, IDictionary class2classmap, StringWriter writer)
		{
			// Find out of there is a system wide way to get sessions defined
			string sessionMethod = classMapping.getMetaAsString("session-method").Trim();

			// fields
			foreach (FieldProperty field in classMapping.Fields)
			{
				if (field.getMeta(MT_FINDERMETHOD) != null)
				{
					string finderName = field.getMetaAsString(MT_FINDERMETHOD);

					if ("".Equals(sessionMethod))
					{
						// Make the method signature require a session to be passed in
						writer.WriteLine("    public static List " + finderName + "(Session session, " +
						                 LanguageTool.getTrueTypeName(field, class2classmap) + " " + field.FieldName + ") " +
						                 "throws SQLException, HibernateException {");
					}
					else
					{
						// Use the session method to get the session to execute the query
						writer.WriteLine("    public static List " + finderName + "(" +
						                 LanguageTool.getTrueTypeName(field, class2classmap) + " " + field.FieldName + ") " +
						                 "throws SQLException, HibernateException {");
						writer.WriteLine("        Session session = " + sessionMethod);
					}

					writer.WriteLine("        List finds = session.find(\"from " + classMapping.FullyQualifiedName + " as " +
					                 classMapping.Name.ToLower() + " where " + classMapping.Name.ToLower() + "." + field.FieldName +
					                 "=?\", " + getFieldAsObject(false, field) + ", " + getFieldAsHibernateType(false, field) + ");");
					writer.WriteLine("        return finds;");
					writer.WriteLine("    }");
					writer.WriteLine();
				}
				else if (field.getMeta(MT_FOREIGNFINDERMETHOD) != null)
				{
					string finderName = field.getMetaAsString(MT_FOREIGNFINDERMETHOD);
					string fieldName = field.getMetaAsString(MT_FOREIGNFINDERFIELD);
					string joinFieldName = field.getMetaAsString(MT_FOREIGNJOINFIELD);

					// Build the query
					QueryBuilder qb = new QueryBuilder();
					qb.LocalClass = classMapping;
					qb.setForeignClass(field.ForeignClass, class2classmap, joinFieldName);

					ClassMapping foreignClass = (ClassMapping) class2classmap[field.ForeignClass.FullyQualifiedName];
					if (foreignClass == null)
					{
						// Can't find the class, return
						log.Error("Could not find the class " + field.ForeignClass.Name);
						return;
					}
					FieldProperty foreignField = null;

					foreach (FieldProperty f in foreignClass.Fields)
					{
						if (f.FieldName.Equals(fieldName))
						{
							foreignField = f;
						}
					}
					if (foreignField != null)
					{
						qb.addCritera(foreignClass, foreignField, "=");
					}
					else
					{
						// Can't find the field, return
						log.Error("Could not find the field " + fieldName + " that was supposed to be in class " + field.ForeignClass.Name);
						return;
					}

					MethodSignatureBuilder msb = new MethodSignatureBuilder(finderName, "List", "public static");
					if ("".Equals(sessionMethod))
					{
						// Make the method signature require a session to be passed in
						msb.addParam("Session session");
						/*
						writer.println("    public static List " + finderName +
						"(Session session, " + getTrueTypeName(foreignField, class2classmap) + " " + foreignField.getName() + ") "
						+ "throws SQLException, HibernateException {");*/
					}
					else
					{
						// Use the session method to get the session to execute the query
						/*
						writer.println("    public static List " + finderName +
						"(" + getTrueTypeName(foreignField, class2classmap) + " " + foreignField.getName() + ") "
						+ "throws SQLException, HibernateException {");
						writer.println("        Session session = " + sessionMethod);*/
					}
					// Always need the object we're basing the query on
					msb.addParam(classMapping.Name + " " + classMapping.Name.ToLower());

					// And the foreign class field
					msb.addParam(LanguageTool.getTrueTypeName(foreignField, class2classmap) + " " + foreignField.FieldName);

					msb.addThrows("SQLException");
					msb.addThrows("HibernateException");

					writer.WriteLine("    " + msb.buildMethodSignature());
					if (!"".Equals(sessionMethod))
					{
						writer.WriteLine("        Session session = " + sessionMethod);
					}

					writer.WriteLine("        List finds = session.find(\"" + qb.Query + "\", " + qb.ParamsAsString + ", " +
					                 qb.ParamTypesAsString + ");");
					writer.WriteLine("        return finds;");
					writer.WriteLine("    }");
					writer.WriteLine();
				}
			}

			// Create the findAll() method
			if ("".Equals(sessionMethod))
			{
				writer.WriteLine("    public static List findAll" + "(Session session) " +
				                 "throws SQLException, HibernateException {");
			}
			else
			{
				writer.WriteLine("    public static List findAll() " + "throws SQLException, HibernateException {");
				writer.WriteLine("        Session session = " + sessionMethod);
			}
			writer.WriteLine("        List finds = session.find(\"from " + classMapping.Name + " in class " +
			                 classMapping.PackageName + "." + classMapping.Name + "\");");
			writer.WriteLine("        return finds;");
			writer.WriteLine("    }");
			writer.WriteLine();
		}


		internal static IDictionary primitiveToObject;


		/// <summary>  Generate the imports for the finder class.
		/// 
		/// </summary>
		public virtual void doImports(ClassMapping classMapping, StreamWriter writer)
		{
			// imports is not included from the class it self as this is a separate generated class.
			/*   classMapping.getImports().add("java.io.Serializable");
			
			for (Iterator imports = classMapping.getImports().iterator(); imports.hasNext(); ) {
			writer.println("import " + imports.next() + ";");
			}*/
			// Imports for finders
			writer.WriteLine("import java.io.Serializable;");
			writer.WriteLine("import java.util.List;");
			writer.WriteLine("import java.sql.SQLException;");
			writer.WriteLine();
			// * import is bad style. But better than importing classing that we don't necesarrily uses...
			writer.WriteLine("import NHibernate.*;");
			writer.WriteLine("import NHibernate.type.Type;");
			//        writer.println("import NHibernate.Hibernate;");
			//        writer.println("import NHibernate.HibernateException;");

			writer.WriteLine();
		}


		/// <summary>  Gets the fieldAsObject attribute of the FinderRenderer object
		/// 
		/// </summary>
		/// <returns>
		/// </returns>
		public static string getFieldAsObject(bool prependThis, FieldProperty field)
		{
			ClassName type = field.ClassType;
			if (type != null && type.Primitive && !type.Array)
			{
				string typeName = (String) primitiveToObject[type.Name];
				typeName = "new " + typeName + "( ";
				typeName += (prependThis ? "this." : "");
				return typeName + field.FieldName + " )";
			}
			return field.FieldName;
		}


		/// <summary>  Coversion map for field types to Hibernate types, might be good to move
		/// this to some other more general class
		/// </summary>
		internal static IDictionary hibType;


		/// <summary>  Return the hibernate type string for the given field
		/// 
		/// </summary>
		/// <returns>
		/// </returns>
		public static string getFieldAsHibernateType(bool prependThis, FieldProperty field)
		{
			ClassName type = field.ClassType;

			string hibTypeString = (String) hibType[type.Name];
			if ((object) hibTypeString != null)
			{
				return hibTypeString;
			}
			else
			{
				return "Hibernate.OBJECT";
			}
		}

		static FinderRenderer()
		{
			primitiveToObject = new Hashtable();
			hibType = new Hashtable();
		}
	}
}