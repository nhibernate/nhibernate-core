//$Id$
using System;
using StringHelper = NHibernate.Util.StringHelper;

namespace NHibernate.tool.hbm2java
{
	
	public class BasicRenderer:AbstractRenderer
	{
		public BasicRenderer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			javaTool = new JavaTool();
			object tempObject;
			tempObject = "Character";
			primitiveToObject["char"] = tempObject;
			System.Object generatedAux = tempObject;
			
			object tempObject2;
			tempObject2 = "Byte";
			primitiveToObject["byte"] = tempObject2;
			System.Object generatedAux2 = tempObject2;
			object tempObject3;
			tempObject3 = "Short";
			primitiveToObject["short"] = tempObject3;
			System.Object generatedAux3 = tempObject3;
			object tempObject4;
			tempObject4 = "Integer";
			primitiveToObject["int"] = tempObject4;
			System.Object generatedAux4 = tempObject4;
			object tempObject5;
			tempObject5 = "Long";
			primitiveToObject["long"] = tempObject5;
			System.Object generatedAux5 = tempObject5;
			
			object tempObject6;
			tempObject6 = "Boolean";
			primitiveToObject["boolean"] = tempObject6;
			System.Object generatedAux6 = tempObject6;
			
			object tempObject7;
			tempObject7 = "Float";
			primitiveToObject["float"] = tempObject7;
			System.Object generatedAux7 = tempObject7;
			object tempObject8;
			tempObject8 = "Double";
			primitiveToObject["double"] = tempObject8;
			System.Object generatedAux8 = tempObject8;
		}
		
		protected internal const int ORDINARY = 0;
		protected internal const int BOUND = 1;
		protected internal const int CONSTRAINT = 3; //any constraint properties are bound as well
		
		//UPGRADE_NOTE: The initialization of  'javaTool' was moved to method 'InitBlock'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		internal JavaTool javaTool;
		
		public override void  render(System.String savedToPackage, System.String savedToClass, ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StreamWriter mainwriter)
		{
			
			
			genPackageDelaration(savedToPackage, classMapping, mainwriter);
			mainwriter.WriteLine();
			
			// switch to another writer to be able to insert the actually
			// used imports when whole class has been rendered. 
			System.IO.StringWriter writer = new System.IO.StringWriter();
			
			
			// class declaration
			if (classMapping.getMeta("class-description") == null)
			{
				writer.WriteLine("/** @author Hibernate CodeGenerator */");
			}
			else
			{
				writer.WriteLine("/** \n" + javaTool.toJavaDoc(classMapping.getMetaAsString("class-description"), 0) + "*/");
			}
			
			System.String classScope = classMapping.Scope;
			System.String declarationType = classMapping.DeclarationType;
			
			
			classMapping.addImport(typeof(System.Runtime.Serialization.ISerializable));
			//String modifiers = classMapping.getModifiers();
			if (classMapping.shouldBeAbstract() && (classScope.IndexOf("abstract") == - 1))
			{
				writer.Write("abstract " + classScope + " " + declarationType + " " + savedToClass);
			}
			else
			{
				writer.Write(classScope + " " + declarationType + " " + savedToClass);
			}
			
			if (javaTool.hasExtends(classMapping))
			{
				writer.Write(" extends " + javaTool.getExtends(classMapping));
			}
			
			if (javaTool.hasImplements(classMapping))
			{
				writer.Write(" implements " + javaTool.getImplements(classMapping));
			}
			
			writer.WriteLine(" {");
			writer.WriteLine();
			
			// switch to another writer to be able to insert the 
			// veto- and changeSupport fields
			System.IO.StringWriter propWriter = new System.IO.StringWriter();
			
			if (!classMapping.Interface)
			{
				doFields(classMapping, class2classmap, propWriter);
				doConstructors(savedToClass, classMapping, class2classmap, propWriter);
			}
			
			System.String vetoSupport = makeSupportField("vetos", classMapping.AllFields);
			System.String changeSupport = makeSupportField("changes", classMapping.AllFields);
			int fieldTypes = doFieldAccessors(classMapping, class2classmap, propWriter, vetoSupport, changeSupport);
			
			if (!classMapping.Interface)
			{
				doSupportMethods(fieldTypes, vetoSupport, changeSupport, propWriter);
				
				doToString(classMapping, propWriter);
				
				doEqualsAndHashCode(savedToClass, classMapping, propWriter);
			}
			if (classMapping.getMeta("class-code") != null)
			{
				propWriter.WriteLine("// The following is extra code specified in the hbm.xml files");
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport extras = classMapping.getMeta("class-code");
				System.Collections.IEnumerator iter = extras.GetEnumerator();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				while (iter.MoveNext())
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					System.String code = iter.Current.ToString();
					propWriter.WriteLine(code);
				}
				
				propWriter.WriteLine("// end of extra code specified in the hbm.xml files");
			}
			
			propWriter.WriteLine("}");
			
			//insert change and VetoSupport
			if (!classMapping.Interface)
			{
				doSupports(fieldTypes, classMapping, vetoSupport, changeSupport, writer);
			}
			
			writer.Write(propWriter.ToString());
			
			// finally write the imports
			doImports(classMapping, mainwriter);
			mainwriter.Write(writer.ToString());
		}
		
		/// <summary> Method doSupportMethods.</summary>
		/// <param name="">fieldTypes
		/// </param>
		/// <param name="">vetoSupport
		/// </param>
		/// <param name="">changeSupport
		/// </param>
		/// <param name="">propWriter
		/// </param>
		private void  doSupportMethods(int fieldTypes, System.String vetoSupport, System.String changeSupport, System.IO.StringWriter writer)
		{
			if ((fieldTypes & CONSTRAINT) == CONSTRAINT)
			{
				writer.WriteLine("    public void addVetoableChangeListener( VetoableChangeListener l ) {");
				writer.WriteLine("        " + vetoSupport + ".addVetoableChangeListener(l);");
				writer.WriteLine("    }");
				writer.WriteLine("    public void removeVetoableChangeListener( VetoableChangeListener l ) {");
				writer.WriteLine("        " + vetoSupport + ".removeVetoableChangeListener(l);");
				writer.WriteLine("    }");
				writer.WriteLine();
			}
			if ((fieldTypes & BOUND) == BOUND)
			{
				writer.WriteLine("    public void addPropertyChangeListener( PropertyChangeListener l ) {");
				writer.WriteLine("        " + changeSupport + ".addPropertyChangeListener(l);");
				writer.WriteLine("    }");
				writer.WriteLine("    public void removePropertyChangeListener( PropertyChangeListener l ) {");
				writer.WriteLine("        " + changeSupport + ".removePropertyChangeListener(l);");
				writer.WriteLine("    }");
				writer.WriteLine();
			}
		}
		
		
		/// <summary> Method doSupports.</summary>
		/// <param name="">vetoSupport
		/// </param>
		/// <param name="">changeSupport
		/// </param>
		/// <param name="">writer
		/// </param>
		private void  doSupports(int fieldTypes, ClassMapping classMapping, System.String vetoSupport, System.String changeSupport, System.IO.StringWriter writer)
		{
			if ((fieldTypes & CONSTRAINT) == CONSTRAINT)
			{
				writer.WriteLine("    private VetoableChangeSupport " + vetoSupport + " = new VetoableChangeSupport(this);");
				classMapping.Imports.Add("java.beans.VetoableChangeSupport");
				classMapping.Imports.Add("java.beans.PropertyVetoException");
				classMapping.Imports.Add("java.beans.VetoableChangeListener");
			}
			if ((fieldTypes & BOUND) == BOUND)
			{
				writer.WriteLine("    private PropertyChangeSupport " + changeSupport + " = new PropertyChangeSupport(this);");
				writer.WriteLine();
				classMapping.Imports.Add("java.beans.PropertyChangeSupport");
				classMapping.Imports.Add("java.beans.PropertyChangeListener");
			}
		}
		
		
		public virtual void  doConstructors(System.String savedToClass, ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StringWriter writer)
		{
			// full constructor
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport allFieldsForFullConstructor = classMapping.AllFieldsForFullConstructor;
			
			writer.WriteLine("    /** full constructor */");
			System.String fullCons = "    public " + savedToClass + StringHelper.OpenParen;
			
			fullCons += javaTool.fieldsAsParameters(allFieldsForFullConstructor, classMapping, class2classmap);
			
			writer.WriteLine(fullCons + ") {");
			//invoke super to initialize superclass...
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport supersConstructorFields = classMapping.FieldsForSupersFullConstructor;
			if (!(supersConstructorFields.Count == 0))
			{
				writer.Write("        super(");
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator fields = supersConstructorFields.GetEnumerator(); fields.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty field = (FieldProperty) fields.Current;
					writer.Write(field.FieldName);
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
					if (fields.MoveNext())
					{
						writer.Write(", ");
					}
				}
				writer.WriteLine(");");
			}
			
			// initialisation of localfields
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator fields = classMapping.LocalFieldsForFullConstructor.GetEnumerator(); fields.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				FieldProperty field = (FieldProperty) fields.Current;
				if (field.GeneratedAsProperty)
				{
					writer.WriteLine("        this." + field.FieldName + " = " + field.FieldName + ";");
				}
			}
			writer.WriteLine("    }");
			writer.WriteLine();
			
			// no args constructor (if fullconstructor had any arguments!)
			if (allFieldsForFullConstructor.Count > 0)
			{
				writer.WriteLine("    /** default constructor */");
				writer.WriteLine("    public " + savedToClass + "() {");
				writer.WriteLine("    }");
				writer.WriteLine();
			}
			
			// minimal constructor (only if the fullconstructor had any arguments)
			if ((allFieldsForFullConstructor.Count > 0) && classMapping.needsMinimalConstructor())
			{
				
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport allFieldsForMinimalConstructor = classMapping.AllFieldsForMinimalConstructor;
				writer.WriteLine("    /** minimal constructor */");
				
				System.String minCons = "    public " + savedToClass + "(";
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator fields = allFieldsForMinimalConstructor.GetEnumerator(); fields.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty field = (FieldProperty) fields.Current;
					minCons = minCons + JavaTool.shortenType(JavaTool.getTrueTypeName(field, class2classmap), classMapping.Imports) + " " + field.FieldName;
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
					if (fields.MoveNext())
					{
						minCons = minCons + ", ";
					}
				}
				
				writer.WriteLine(minCons + ") {");
				// invoke super to initialize superclass...
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport supersMinConstructorFields = classMapping.FieldsForSupersMinimalConstructor;
				if (!(supersMinConstructorFields.Count == 0))
				{
					writer.Write("      super(");
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
					for (System.Collections.IEnumerator fields = supersMinConstructorFields.GetEnumerator(); fields.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
						FieldProperty field = (FieldProperty) fields.Current;
						writer.Write(field.FieldName);
						//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
						if (fields.MoveNext())
						{
							writer.Write(StringHelper.CommaSpace);
						}
					}
					writer.WriteLine(");");
				}
				
				// initialisation of localfields
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator fields = classMapping.LocalFieldsForMinimalConstructor.GetEnumerator(); fields.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty field = (FieldProperty) fields.Current;
					if (field.GeneratedAsProperty)
					{
						writer.WriteLine("        this." + field.FieldName + " = " + field.FieldName + ";");
					}
				}
				writer.WriteLine("    }");
				writer.WriteLine();
			}
		}
		
		public virtual void  doFields(ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StringWriter writer)
		{
			// fields
			if (!classMapping.Interface)
			{
				if (classMapping.SuperInterface)
				{
					doFields(classMapping.AllFields, classMapping.Imports, class2classmap, writer);
				}
			}
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport fieldList = classMapping.Fields;
			SupportClass.SetSupport imports = classMapping.Imports;
			doFields(fieldList, imports, class2classmap, writer);
		}
		
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		private void  doFields(SupportClass.ListCollectionSupport fieldList, SupportClass.SetSupport imports, System.Collections.IDictionary class2classmap, System.IO.StringWriter writer)
		{
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator fields = fieldList.GetEnumerator(); fields.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				FieldProperty field = (FieldProperty) fields.Current;
				
				if (field.GeneratedAsProperty)
				{
					System.String fieldScope = getFieldScope(field, "scope-field", "private");
					writer.WriteLine("    /** " + (field.Nullable && !field.Identifier?"nullable ":string.Empty) + (field.Identifier?"identifier":"persistent") + " field */");
					
					writer.Write("    " + fieldScope + " " + JavaTool.shortenType(JavaTool.getTrueTypeName(field, class2classmap), imports) + ' ' + field.FieldName);
					
					if (field.getMeta("default-value") != null)
					{
						writer.Write(" = " + field.getMetaAsString("default-value"));
					}
					writer.WriteLine(';');
				}
				writer.WriteLine();
			}
		}
		
		public virtual void  doEqualsAndHashCode(System.String savedToClass, ClassMapping classMapping, System.IO.StringWriter writer)
		{
			if (classMapping.mustImplementEquals())
			{
				classMapping.Imports.Add("org.apache.commons.lang.builder.EqualsBuilder");
				classMapping.Imports.Add("org.apache.commons.lang.builder.HashCodeBuilder");
				writer.WriteLine("    public boolean equals(Object other) {");
				writer.WriteLine("        if ( (other == other ) ) return true;"); // == identity
				writer.WriteLine("        if ( !(other instanceof " + savedToClass + ") ) return false;"); // same class ?
				writer.WriteLine("        " + savedToClass + " castOther = (" + savedToClass + ") other;");
				writer.WriteLine("        return new EqualsBuilder()");
				int usedFields = 0;
				//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
				SupportClass.ListCollectionSupport idFields = new SupportClass.ListCollectionSupport();
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator fields = classMapping.Fields.GetEnumerator(); fields.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty field = (FieldProperty) fields.Current;
					if (field.getMetaAsBool("use-in-equals"))
					{
						writer.WriteLine("            .append(this." + field.GetterSignature + ", castOther." + field.GetterSignature + StringHelper.ClosedParen);
						usedFields++;
					}
					if (field.Identifier)
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
						idFields.Add(field);
					}
				}
				if (usedFields == 0)
				{
					//log.warn("No properties has been marked as being used in equals/hashcode for " + classMapping.Name + ". Using object identifier which is RARELY safe to use! See http://hibernate.org/109.html");
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
					for (System.Collections.IEnumerator fields = idFields.GetEnumerator(); fields.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
						FieldProperty field = (FieldProperty) fields.Current;
						writer.WriteLine("            .append(this." + field.GetterSignature + ", castOther." + field.GetterSignature + StringHelper.ClosedParen);
					}
				}
				writer.WriteLine("            .isEquals();");
				writer.WriteLine("    }");
				writer.WriteLine();
				
				writer.WriteLine("    public int hashCode() {");
				writer.WriteLine("        return new HashCodeBuilder()");
				
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
				for (System.Collections.IEnumerator fields = classMapping.Fields.GetEnumerator(); fields.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
					FieldProperty field = (FieldProperty) fields.Current;
					if (field.getMetaAsBool("use-in-equals"))
					{
						writer.WriteLine("            .append(" + field.GetterSignature + StringHelper.ClosedParen);
					}
				}
				if (usedFields == 0)
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
					for (System.Collections.IEnumerator fields = idFields.GetEnumerator(); fields.MoveNext(); )
					{
						//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
						FieldProperty field = (FieldProperty) fields.Current;
						writer.WriteLine("            .append(" + field.GetterSignature + StringHelper.ClosedParen);
					}
				}
				
				writer.WriteLine("            .toHashCode();");
				writer.WriteLine("    }");
				writer.WriteLine();
			}
		}
		
		public virtual void  doToString(ClassMapping classMapping, System.IO.StringWriter writer)
		{
			
			classMapping.addImport("org.apache.commons.lang.builder.ToStringBuilder");
			writer.WriteLine("    public String toString() {");
			writer.WriteLine("        return new ToStringBuilder(this)");
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator fields = classMapping.AllFields.GetEnumerator(); fields.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				FieldProperty field = (FieldProperty) fields.Current;
				// If nothing is stated about id then include it in toString()
				if (field.Identifier && field.getMeta("use-in-tostring") == null)
				{
					writer.WriteLine("            .append(\"" + field.FieldName + "\", " + field.GetterSignature + ")");
				}
				else if (field.getMetaAsBool("use-in-tostring"))
				{
					writer.WriteLine("            .append(\"" + field.FieldName + "\", " + field.GetterSignature + ")");
				}
			}
			writer.WriteLine("            .toString();");
			writer.WriteLine("    }");
			writer.WriteLine();
		}
		
		//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilHashMap"'
		//UPGRADE_NOTE: The initialization of  'primitiveToObject' was moved to static method 'NHibernate.tool.hbm2java.BasicRenderer'. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1005"'
		internal static System.Collections.IDictionary primitiveToObject;
		
		public virtual int doFieldAccessors(ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StringWriter writer, System.String vetoSupport, System.String changeSupport)
		{
			int fieldTypes = ORDINARY;
			
			if (classMapping.SuperInterface)
			{
				fieldTypes = doFields(classMapping, class2classmap, writer, vetoSupport, changeSupport, fieldTypes, classMapping.AllFields);
			}
			//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			SupportClass.ListCollectionSupport fieldz = classMapping.Fields;
			fieldTypes = doFields(classMapping, class2classmap, writer, vetoSupport, changeSupport, fieldTypes, fieldz);
			return fieldTypes;
		}
		
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		private int doFields(ClassMapping classMapping, System.Collections.IDictionary class2classmap, System.IO.StringWriter writer, System.String vetoSupport, System.String changeSupport, int fieldTypes, SupportClass.ListCollectionSupport fieldz)
		{
			// field accessors
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator fields = fieldz.GetEnumerator(); fields.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				FieldProperty field = (FieldProperty) fields.Current;
				if (field.GeneratedAsProperty)
				{
					
					// getter
					System.String getAccessScope = getFieldScope(field, "scope-get", "public");
					
					
					if (field.getMeta("field-description") != null)
					{
						writer.WriteLine("    /** \n" + javaTool.toJavaDoc(field.getMetaAsString("field-description"), 4) + "     */");
					}
					writer.Write("    " + getAccessScope + " " + JavaTool.shortenType(JavaTool.getTrueTypeName(field, class2classmap), classMapping.Imports) + " " + field.GetterSignature);
					if (classMapping.Interface)
					{
						writer.WriteLine(";");
					}
					else
					{
						writer.WriteLine(" {");
						writer.WriteLine("        return this." + field.FieldName + ";");
						writer.WriteLine("    }");
					}
					writer.WriteLine();
					
					// setter
					int fieldType = 0;
					if (field.getMeta("beans-property-type") != null)
					{
						System.String beansPropertyType = field.getMetaAsString("beans-property-type").Trim().ToLower();
						if (beansPropertyType.Equals("constraint"))
						{
							fieldTypes = (fieldTypes | CONSTRAINT);
							fieldType = CONSTRAINT;
						}
						else if (beansPropertyType.Equals("bound"))
						{
							fieldTypes = (fieldTypes | BOUND);
							fieldType = BOUND;
						}
					}
					System.String setAccessScope = getFieldScope(field, "scope-set", "public");
					writer.Write("    " + setAccessScope + " void set" + field.AccessorName + StringHelper.OpenParen + JavaTool.shortenType(JavaTool.getTrueTypeName(field, class2classmap), classMapping.Imports) + " " + field.FieldName + ")");
					writer.Write((fieldType & CONSTRAINT) == CONSTRAINT?" throws PropertyVetoException ":"");
					if (classMapping.Interface)
					{
						writer.WriteLine(";");
					}
					else
					{
						writer.WriteLine(" {");
						if ((fieldType & CONSTRAINT) == CONSTRAINT || (fieldType & BOUND) == BOUND)
						{
							writer.WriteLine("        Object oldValue = " + getFieldAsObject(true, field) + ";");
						}
						if ((fieldType & CONSTRAINT) == CONSTRAINT)
						{
							
							writer.WriteLine("        " + vetoSupport + ".fireVetoableChange(\"" + field.FieldName + "\",");
							writer.WriteLine("                oldValue,");
							writer.WriteLine("                " + getFieldAsObject(false, field) + ");");
						}
						
						writer.WriteLine("        this." + field.FieldName + " = " + field.FieldName + ";");
						if ((fieldType & BOUND) == BOUND)
						{
							writer.WriteLine("        " + changeSupport + ".firePropertyChange(\"" + field.FieldName + "\",");
							writer.WriteLine("                oldValue,");
							writer.WriteLine("                " + getFieldAsObject(false, field) + ");");
						}
						writer.WriteLine("    }");
					}
					writer.WriteLine();
					
					// add/remove'rs (commented out for now)
					/* 
					if(field.getForeignClass()!=null) { 
					ClassName foreignClass = field.getForeignClass();
					
					String trueforeign = getTrueTypeName(foreignClass, class2classmap);
					classMapping.addImport(trueforeign);
					
					// Try to identify the matching set method on the child.
					ClassMapping forignMap = (ClassMapping) class2classmap.get(foreignClass.getFullyQualifiedName());
					
					if(forignMap!=null) {
					Iterator foreignFields = forignMap.getFields().iterator();
					while (foreignFields.hasNext()) {
					Field ffield = (Field) foreignFields.next();
					if(ffield.isIdentifier()) {
					log.debug("Trying to match " + ffield.getName() + " with " + field.getForeignKeys());   
					}
					}
					
					} else {
					log.error("Could not find foreign class's mapping - cannot provide bidirectional setters!");   
					}
					
					String addAccessScope = getFieldScope(field, "scope", "scope-add");
					writer.println("    " + setAccessScope + " void add" + field.getAsSuffix() + StringHelper.OPEN + shortenType(trueforeign, classMapping.getImports()) + " a" + field.getName() + ") {");
					writer.println("        this." + getterType + field.getAsSuffix() + "().add(a" + field.getName() + ");");
					writer.println("        a" + field.getName() + ".setXXX(this);");
					writer.println("    }");
					writer.println();
					
					
					}
					*/
				}
			}
			return fieldTypes;
		}
		
		public virtual void  doImports(ClassMapping classMapping, System.IO.StreamWriter writer)
		{
			writer.WriteLine(javaTool.genImports(classMapping));
			writer.WriteLine();
		}
		
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		protected internal virtual System.String makeSupportField(System.String fieldName, SupportClass.ListCollectionSupport fieldList)
		{
			System.String suffix = "";
			bool needSuffix = false;
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			for (System.Collections.IEnumerator fields = fieldList.GetEnumerator(); fields.MoveNext(); )
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				System.String name = ((FieldProperty) fields.Current).FieldName;
				if (name.Equals(fieldName))
					needSuffix = true;
				suffix += name;
			}
			return needSuffix?fieldName + "_" + suffix:fieldName;
		}
		
		private System.String getFieldAsObject(bool prependThis, FieldProperty field)
		{
			ClassName type = field.ClassType;
			if (type != null && type.Primitive && !type.Array)
			{
				//UPGRADE_TODO: Method 'java.util.Map.get' was converted to 'System.Collections.IDictionary.Item' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilMapget_javalangObject"'
				System.String typeName = (System.String) primitiveToObject[type.Name];
				typeName = "new " + typeName + "( ";
				typeName += (prependThis?"this.":"");
				return typeName + field.FieldName + " )";
			}
			return (prependThis?"this.":"") + field.FieldName;
		}
		static BasicRenderer()
		{
			//log = LogFactory.getLog(typeof(BasicRenderer));
			primitiveToObject = new System.Collections.Hashtable();
		}
	}
}