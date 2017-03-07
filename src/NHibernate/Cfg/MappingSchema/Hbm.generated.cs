namespace NHibernate.Cfg.MappingSchema {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("any", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmAny {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta-value")]
        public HbmMetaValue[] metavalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id-type")]
        public string idtype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("meta-type")]
        public string metatype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        public HbmAny() {
            this.insert = true;
            this.update = true;
            this.optimisticlock = true;
            this.lazy = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("meta", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmMeta {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string attribute;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool inherit;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        public HbmMeta() {
            this.inherit = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("meta-value", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmMetaValue {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("column", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmColumn {
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string precision;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        public string scale;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notnullSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool uniqueSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unique-key")]
        public string uniquekey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("sql-type")]
        public string sqltype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @default;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("comment", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmComment {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("array", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmArray {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("index", typeof(HbmIndex))]
        [System.Xml.Serialization.XmlElementAttribute("list-index", typeof(HbmListIndex))]
        public object Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(HbmCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(HbmElement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(HbmManyToAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(HbmManyToMany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(HbmOneToMany))]
        public object Item1;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public HbmCustomSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("element-class")]
        public string elementclass;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size")]
        public int batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool batchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        public HbmArray() {
            this.inverse = false;
            this.mutable = true;
            this.optimisticlock = true;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("subselect", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmSubselect {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("cache", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCache {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCacheUsage usage;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string region;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(HbmCacheInclude.All)]
        public HbmCacheInclude include;
        
        public HbmCache() {
            this.include = HbmCacheInclude.All;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmCacheUsage {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("read-only")]
        ReadOnly,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("read-write")]
        ReadWrite,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("nonstrict-read-write")]
        NonstrictReadWrite,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("transactional")]
        Transactional,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmCacheInclude {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("all")]
        All,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("non-lazy")]
        NonLazy,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("synchronize", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmSynchronize {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("key", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmKey {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("on-delete")]
        [System.ComponentModel.DefaultValueAttribute(HbmOndelete.Noaction)]
        public HbmOndelete ondelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notnullSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool updateSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool uniqueSpecified;
        
        public HbmKey() {
            this.ondelete = HbmOndelete.Noaction;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmOndelete {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("cascade")]
        Cascade,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("noaction")]
        Noaction,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("index", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmIndex {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("list-index", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmListIndex {
        
        /// <remarks/>
        public HbmColumn column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string @base;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("composite-element", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCompositeElement {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmParent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("nested-composite-element", typeof(HbmNestedCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("parent", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmParent {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("many-to-one", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmManyToOne {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column", typeof(HbmColumn))]
        [System.Xml.Serialization.XmlElementAttribute("formula", typeof(HbmFormula))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notnullSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unique-key")]
        public string uniquekey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmLaziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-found")]
        [System.ComponentModel.DefaultValueAttribute(HbmNotFoundMode.Exception)]
        public HbmNotFoundMode notfound;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        public HbmManyToOne() {
            this.unique = false;
            this.update = true;
            this.insert = true;
            this.optimisticlock = true;
            this.notfound = HbmNotFoundMode.Exception;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("formula", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmFormula {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmOuterJoinStrategy {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("auto")]
        Auto,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("true")]
        True,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("false")]
        False,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmFetchMode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("select")]
        Select,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("join")]
        Join,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmLaziness {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("false")]
        False,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("proxy")]
        Proxy,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("no-proxy")]
        NoProxy,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmNotFoundMode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("ignore")]
        Ignore,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("exception")]
        Exception,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("nested-composite-element", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmNestedCompositeElement {
        
        /// <remarks/>
        public HbmParent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("nested-composite-element", typeof(HbmNestedCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("property", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmProperty {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column", typeof(HbmColumn))]
        [System.Xml.Serialization.XmlElementAttribute("formula", typeof(HbmFormula))]
        public object[] Items;
        
        /// <remarks/>
        public HbmType type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("type")]
        public string type1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string precision;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        public string scale;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool notnullSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unique-key")]
        public string uniquekey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool updateSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool insertSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(HbmPropertyGeneration.Never)]
        public HbmPropertyGeneration generated;
        
        public HbmProperty() {
            this.unique = false;
            this.optimisticlock = true;
            this.lazy = false;
            this.generated = HbmPropertyGeneration.Never;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("type", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public HbmParam[] param;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("param", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmParam {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmPropertyGeneration {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("never")]
        Never,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("insert")]
        Insert,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("always")]
        Always,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("element", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmElement {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column", typeof(HbmColumn))]
        [System.Xml.Serialization.XmlElementAttribute("formula", typeof(HbmFormula))]
        public object[] Items;
        
        /// <remarks/>
        public HbmType type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("type")]
        public string type1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string precision;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="nonNegativeInteger")]
        public string scale;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        public HbmElement() {
            this.notnull = false;
            this.unique = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("many-to-any", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmManyToAny {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta-value")]
        public HbmMetaValue[] metavalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id-type")]
        public string idtype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("meta-type")]
        public string metatype;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("many-to-many", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmManyToMany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column", typeof(HbmColumn))]
        [System.Xml.Serialization.XmlElementAttribute("formula", typeof(HbmFormula))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-found")]
        [System.ComponentModel.DefaultValueAttribute(HbmNotFoundMode.Exception)]
        public HbmNotFoundMode notfound;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmRestrictedLaziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
        
        public HbmManyToMany() {
            this.embedxml = true;
            this.notfound = HbmNotFoundMode.Exception;
            this.unique = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("filter", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmFilter {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string condition;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmRestrictedLaziness {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("false")]
        False,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("proxy")]
        Proxy,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("one-to-many", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmOneToMany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-found")]
        [System.ComponentModel.DefaultValueAttribute(HbmNotFoundMode.Exception)]
        public HbmNotFoundMode notfound;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        public HbmOneToMany() {
            this.notfound = HbmNotFoundMode.Exception;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("loader", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmLoader {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("query-ref")]
        public string queryref;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("sql-delete", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCustomSQL {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool callable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool callableSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCustomSQLCheck check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool checkSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmCustomSQLCheck {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("none")]
        None,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("rowcount")]
        Rowcount,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("param")]
        Param,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmCollectionFetchMode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("select")]
        Select,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("join")]
        Join,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("subselect")]
        Subselect,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("bag", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmBag {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(HbmCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(HbmElement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(HbmManyToAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(HbmManyToMany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(HbmOneToMany))]
        public object Item;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public HbmCustomSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionLazy lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size")]
        public int batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool batchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        public HbmBag() {
            this.inverse = false;
            this.mutable = true;
            this.optimisticlock = true;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmCollectionLazy {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("true")]
        True,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("false")]
        False,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("extra")]
        Extra,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("class", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmClass {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tuplizer")]
        public HbmTuplizer[] tuplizer;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-id", typeof(HbmCompositeId))]
        [System.Xml.Serialization.XmlElementAttribute("id", typeof(HbmId))]
        public object Item;
        
        /// <remarks/>
        public HbmDiscriminator discriminator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("natural-id")]
        public HbmNaturalId naturalid;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("timestamp", typeof(HbmTimestamp))]
        [System.Xml.Serialization.XmlElementAttribute("version", typeof(HbmVersion))]
        public object Item1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(HbmArray))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(HbmBag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(HbmIdbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(HbmList))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(HbmMap))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(HbmOneToOne))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(HbmPrimitiveArray))]
        [System.Xml.Serialization.XmlElementAttribute("properties", typeof(HbmProperties))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(HbmSet))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("join", typeof(HbmJoin))]
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass", typeof(HbmJoinedSubclass))]
        [System.Xml.Serialization.XmlElementAttribute("subclass", typeof(HbmSubclass))]
        [System.Xml.Serialization.XmlElementAttribute("union-subclass", typeof(HbmUnionSubclass))]
        public object[] Items1;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resultset")]
        public HbmResultSet[] resultset;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query", typeof(HbmQuery))]
        [System.Xml.Serialization.XmlElementAttribute("sql-query", typeof(HbmSqlQuery))]
        public object[] Items2;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string proxy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("schema-action")]
        public string schemaaction;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("discriminator-value")]
        public string discriminatorvalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @abstract;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool abstractSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(HbmPolymorphismType.Implicit)]
        public HbmPolymorphismType polymorphism;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size")]
        public int batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool batchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("select-before-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selectbeforeupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(HbmOptimisticLockMode.Version)]
        public HbmOptimisticLockMode optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string rowid;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        public HbmClass() {
            this.mutable = true;
            this.polymorphism = HbmPolymorphismType.Implicit;
            this.dynamicupdate = false;
            this.dynamicinsert = false;
            this.selectbeforeupdate = false;
            this.optimisticlock = HbmOptimisticLockMode.Version;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("tuplizer", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmTuplizer {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-mode")]
        public HbmTuplizerEntitymode entitymode;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool entitymodeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmTuplizerEntitymode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("poco")]
        Poco,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("xml")]
        Xml,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("dynamic-map")]
        DynamicMap,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("composite-id", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCompositeId {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("key-many-to-one", typeof(HbmKeyManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("key-property", typeof(HbmKeyProperty))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool mapped;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        [System.ComponentModel.DefaultValueAttribute(HbmUnsavedValueType.Undefined)]
        public HbmUnsavedValueType unsavedvalue;
        
        public HbmCompositeId() {
            this.mapped = false;
            this.unsavedvalue = HbmUnsavedValueType.Undefined;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("key-many-to-one", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmKeyManyToOne {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmRestrictedLaziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-found")]
        [System.ComponentModel.DefaultValueAttribute(HbmNotFoundMode.Exception)]
        public HbmNotFoundMode notfound;
        
        public HbmKeyManyToOne() {
            this.notfound = HbmNotFoundMode.Exception;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("key-property", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmKeyProperty {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        public HbmType type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("type")]
        public string type1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmUnsavedValueType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("undefined")]
        Undefined,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("any")]
        Any,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("none")]
        None,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("id", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmId {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        public HbmType type;
        
        /// <remarks/>
        public HbmGenerator generator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("type")]
        public string type1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        public string unsavedvalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("generator")]
        public string generator1;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("generator", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmGenerator {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public HbmParam[] param;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("discriminator", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmDiscriminator {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column", typeof(HbmColumn))]
        [System.Xml.Serialization.XmlElementAttribute("formula", typeof(HbmFormula))]
        public object Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("string")]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("not-null")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool notnull;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool force;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        public HbmDiscriminator() {
            this.type = "string";
            this.notnull = true;
            this.force = false;
            this.insert = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("natural-id", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmNaturalId {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool mutable;
        
        public HbmNaturalId() {
            this.mutable = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("component", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmComponent {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tuplizer")]
        public HbmTuplizer[] tuplizer;
        
        /// <remarks/>
        public HbmParent parent;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(HbmArray))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(HbmBag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(HbmIdbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(HbmList))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(HbmMap))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(HbmOneToOne))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(HbmPrimitiveArray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(HbmSet))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        public HbmComponent() {
            this.unique = false;
            this.update = true;
            this.insert = true;
            this.lazy = false;
            this.optimisticlock = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("dynamic-component", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmDynamicComponent {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(HbmArray))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(HbmBag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(HbmList))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(HbmMap))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(HbmOneToOne))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(HbmPrimitiveArray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(HbmSet))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        public HbmDynamicComponent() {
            this.unique = false;
            this.update = true;
            this.insert = true;
            this.optimisticlock = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("list", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmList {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("index", typeof(HbmIndex))]
        [System.Xml.Serialization.XmlElementAttribute("list-index", typeof(HbmListIndex))]
        public object Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(HbmCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(HbmElement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(HbmManyToAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(HbmManyToMany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(HbmOneToMany))]
        public object Item1;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public HbmCustomSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionLazy lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size")]
        public int batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool batchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        public HbmList() {
            this.inverse = false;
            this.mutable = true;
            this.optimisticlock = true;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("map", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmMap {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-index", typeof(HbmCompositeIndex))]
        [System.Xml.Serialization.XmlElementAttribute("composite-map-key", typeof(HbmCompositeMapKey))]
        [System.Xml.Serialization.XmlElementAttribute("index", typeof(HbmIndex))]
        [System.Xml.Serialization.XmlElementAttribute("index-many-to-any", typeof(HbmIndexManyToAny))]
        [System.Xml.Serialization.XmlElementAttribute("index-many-to-many", typeof(HbmIndexManyToMany))]
        [System.Xml.Serialization.XmlElementAttribute("map-key", typeof(HbmMapKey))]
        [System.Xml.Serialization.XmlElementAttribute("map-key-many-to-many", typeof(HbmMapKeyManyToMany))]
        public object Item;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(HbmCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(HbmElement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(HbmManyToAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(HbmManyToMany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(HbmOneToMany))]
        public object Item1;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public HbmCustomSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionLazy lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size")]
        public int batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool batchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string sort;
        
        public HbmMap() {
            this.inverse = false;
            this.mutable = true;
            this.optimisticlock = true;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("composite-index", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCompositeIndex {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("key-many-to-one", typeof(HbmKeyManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("key-property", typeof(HbmKeyProperty))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("composite-map-key", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCompositeMapKey {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("key-many-to-one", typeof(HbmKeyManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("key-property", typeof(HbmKeyProperty))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("index-many-to-any", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmIndexManyToAny {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("id-type")]
        public string idtype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("meta-type")]
        public string metatype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("index-many-to-many", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmIndexManyToMany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("map-key", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmMapKey {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column", typeof(HbmColumn))]
        [System.Xml.Serialization.XmlElementAttribute("formula", typeof(HbmFormula))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("map-key-many-to-many", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmMapKeyManyToMany {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column", typeof(HbmColumn))]
        [System.Xml.Serialization.XmlElementAttribute("formula", typeof(HbmFormula))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("one-to-one", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmOneToOne {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("formula")]
        public HbmFormula[] formula;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("formula")]
        public string formula1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool constrained;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("foreign-key")]
        public string foreignkey;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("property-ref")]
        public string propertyref;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmLaziness lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        public HbmOneToOne() {
            this.constrained = false;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("primitive-array", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmPrimitiveArray {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("index", typeof(HbmIndex))]
        [System.Xml.Serialization.XmlElementAttribute("list-index", typeof(HbmListIndex))]
        public object Item;
        
        /// <remarks/>
        public HbmElement element;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public HbmCustomSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmPrimitivearrayOuterjoin outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmPrimitivearrayFetch fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        public HbmPrimitiveArray() {
            this.mutable = true;
            this.optimisticlock = true;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmPrimitivearrayOuterjoin {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("true")]
        True,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("false")]
        False,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("auto")]
        Auto,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmPrimitivearrayFetch {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("join")]
        Join,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("select")]
        Select,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("subselect")]
        Subselect,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("set", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmSet {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(HbmCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(HbmElement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(HbmManyToAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(HbmManyToMany))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-many", typeof(HbmOneToMany))]
        public object Item;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public HbmCustomSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionLazy lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size")]
        public int batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool batchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string sort;
        
        public HbmSet() {
            this.inverse = false;
            this.mutable = true;
            this.optimisticlock = true;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("idbag", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmIdbag {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmCache cache;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("collection-id")]
        public HbmCollectionId collectionid;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("composite-element", typeof(HbmCompositeElement))]
        [System.Xml.Serialization.XmlElementAttribute("element", typeof(HbmElement))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-any", typeof(HbmManyToAny))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-many", typeof(HbmManyToMany))]
        public object Item;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete-all")]
        public HbmCustomSQL sqldeleteall;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionLazy lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool mutable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("order-by")]
        public string orderby;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @where;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size")]
        public int batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool batchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("outer-join")]
        public HbmOuterJoinStrategy outerjoin;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool outerjoinSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public HbmCollectionFetchMode fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collection-type")]
        public string collectiontype;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("embed-xml")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool embedxml;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool generic;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool genericSpecified;
        
        public HbmIdbag() {
            this.inverse = false;
            this.mutable = true;
            this.optimisticlock = true;
            this.embedxml = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("collection-id", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCollectionId {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        public HbmGenerator generator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string length;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("timestamp", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmTimestamp {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        public HbmTimestampUnsavedvalue unsavedvalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool unsavedvalueSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(HbmTimestampSource.Vm)]
        public HbmTimestampSource source;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(HbmVersionGeneration.Never)]
        public HbmVersionGeneration generated;
        
        public HbmTimestamp() {
            this.source = HbmTimestampSource.Vm;
            this.generated = HbmVersionGeneration.Never;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmTimestampUnsavedvalue {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("null")]
        Null,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("undefined")]
        Undefined,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmTimestampSource {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("vm")]
        Vm,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("db")]
        Db,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmVersionGeneration {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("never")]
        Never,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("always")]
        Always,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("version", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmVersion {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("column")]
        public HbmColumn[] column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string access;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("column")]
        public string column1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("Int32")]
        public string type;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("unsaved-value")]
        public string unsavedvalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(HbmVersionGeneration.Never)]
        public HbmVersionGeneration generated;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool insertSpecified;
        
        public HbmVersion() {
            this.type = "Int32";
            this.generated = HbmVersionGeneration.Never;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("properties", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmProperties {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool unique;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool insert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool update;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("optimistic-lock")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool optimisticlock;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        public HbmProperties() {
            this.unique = false;
            this.insert = true;
            this.update = true;
            this.optimisticlock = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("join", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmJoin {
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(HbmArray))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(HbmBag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(HbmIdbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(HbmList))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(HbmMap))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(HbmPrimitiveArray))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(HbmSet))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(HbmJoinFetch.Join)]
        public HbmJoinFetch fetch;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool inverse;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool optional;
        
        public HbmJoin() {
            this.fetch = HbmJoinFetch.Join;
            this.inverse = false;
            this.optional = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmJoinFetch {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("join")]
        Join,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("select")]
        Select,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("joined-subclass", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmJoinedSubclass {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tuplizer")]
        public HbmTuplizer[] tuplizer;
        
        /// <remarks/>
        public HbmKey key;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(HbmArray))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(HbmBag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(HbmIdbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(HbmList))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(HbmMap))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(HbmOneToOne))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(HbmPrimitiveArray))]
        [System.Xml.Serialization.XmlElementAttribute("properties", typeof(HbmProperties))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(HbmSet))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass")]
        public HbmJoinedSubclass[] joinedsubclass1;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resultset")]
        public HbmResultSet[] resultset;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query", typeof(HbmQuery))]
        [System.Xml.Serialization.XmlElementAttribute("sql-query", typeof(HbmSqlQuery))]
        public object[] Items1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string proxy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("schema-action")]
        public string schemaaction;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("select-before-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selectbeforeupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extends;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @abstract;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool abstractSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;

        public HbmJoinedSubclass() {
            this.dynamicupdate = false;
            this.dynamicinsert = false;
            this.selectbeforeupdate = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("resultset", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmResultSet {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("load-collection", typeof(HbmLoadCollection))]
        [System.Xml.Serialization.XmlElementAttribute("return", typeof(HbmReturn))]
        [System.Xml.Serialization.XmlElementAttribute("return-join", typeof(HbmReturnJoin))]
        [System.Xml.Serialization.XmlElementAttribute("return-scalar", typeof(HbmReturnScalar))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("load-collection", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmLoadCollection {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-property")]
        public HbmReturnProperty[] returnproperty;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string alias;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string role;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("lock-mode")]
        [System.ComponentModel.DefaultValueAttribute(HbmLockMode.Read)]
        public HbmLockMode lockmode;
        
        public HbmLoadCollection() {
            this.lockmode = HbmLockMode.Read;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("return-property", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmReturnProperty {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-column")]
        public HbmReturnColumn[] returncolumn;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("return-column", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmReturnColumn {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmLockMode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("none")]
        None,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("read")]
        Read,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("upgrade")]
        Upgrade,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("upgrade-nowait")]
        UpgradeNowait,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("write")]
        Write,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("return", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmReturn {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-discriminator")]
        public HbmReturnDiscriminator returndiscriminator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-property")]
        public HbmReturnProperty[] returnproperty;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string alias;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("lock-mode")]
        [System.ComponentModel.DefaultValueAttribute(HbmLockMode.Read)]
        public HbmLockMode lockmode;
        
        public HbmReturn() {
            this.lockmode = HbmLockMode.Read;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("return-discriminator", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmReturnDiscriminator {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("return-join", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmReturnJoin {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("return-property")]
        public HbmReturnProperty[] returnproperty;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string alias;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string property;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("lock-mode")]
        [System.ComponentModel.DefaultValueAttribute(HbmLockMode.Read)]
        public HbmLockMode lockmode;
        
        public HbmReturnJoin() {
            this.lockmode = HbmLockMode.Read;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("return-scalar", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmReturnScalar {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string column;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("query", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmQuery {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query-param")]
        public HbmQueryParam[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("flush-mode")]
        public HbmFlushMode flushmode;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool flushmodeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool cacheable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("cache-region")]
        public string cacheregion;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("fetch-size")]
        public int fetchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string timeout;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("cache-mode")]
        public HbmCacheMode cachemode;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cachemodeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("read-only")]
        public bool @readonly;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool readonlySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string comment;
        
        public HbmQuery() {
            this.cacheable = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("query-param", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmQueryParam {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmFlushMode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("auto")]
        Auto,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("never")]
        Never,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("always")]
        Always,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmCacheMode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("get")]
        Get,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("ignore")]
        Ignore,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("normal")]
        Normal,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("put")]
        Put,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("refresh")]
        Refresh,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("sql-query", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmSqlQuery {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("load-collection", typeof(HbmLoadCollection))]
        [System.Xml.Serialization.XmlElementAttribute("query-param", typeof(HbmQueryParam))]
        [System.Xml.Serialization.XmlElementAttribute("return", typeof(HbmReturn))]
        [System.Xml.Serialization.XmlElementAttribute("return-join", typeof(HbmReturnJoin))]
        [System.Xml.Serialization.XmlElementAttribute("return-scalar", typeof(HbmReturnScalar))]
        [System.Xml.Serialization.XmlElementAttribute("synchronize", typeof(HbmSynchronize))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("resultset-ref")]
        public string resultsetref;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("flush-mode")]
        public HbmFlushMode flushmode;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool flushmodeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool cacheable;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("cache-region")]
        public string cacheregion;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("fetch-size")]
        public int fetchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool fetchsizeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="positiveInteger")]
        public string timeout;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("cache-mode")]
        public HbmCacheMode cachemode;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cachemodeSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("read-only")]
        public bool @readonly;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool readonlySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string comment;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool callable;
        
        public HbmSqlQuery() {
            this.cacheable = false;
            this.callable = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("subclass", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmSubclass {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tuplizer")]
        public HbmTuplizer[] tuplizer;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(HbmArray))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(HbmBag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(HbmIdbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(HbmList))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(HbmMap))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(HbmOneToOne))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(HbmPrimitiveArray))]
        [System.Xml.Serialization.XmlElementAttribute("properties", typeof(HbmProperties))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(HbmSet))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("join")]
        public HbmJoin[] join;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subclass")]
        public HbmSubclass[] subclass1;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resultset")]
        public HbmResultSet[] resultset;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query", typeof(HbmQuery))]
        [System.Xml.Serialization.XmlElementAttribute("sql-query", typeof(HbmSqlQuery))]
        public object[] Items1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string proxy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("discriminator-value")]
        public string discriminatorvalue;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("select-before-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selectbeforeupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extends;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @abstract;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool abstractSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter")]
        public HbmFilter[] filter;
        
        public HbmSubclass() {
            this.dynamicupdate = false;
            this.dynamicinsert = false;
            this.selectbeforeupdate = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("union-subclass", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmUnionSubclass {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        public HbmSubselect subselect;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("synchronize")]
        public HbmSynchronize[] synchronize;
        
        /// <remarks/>
        public HbmComment comment;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tuplizer")]
        public HbmTuplizer[] tuplizer;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("any", typeof(HbmAny))]
        [System.Xml.Serialization.XmlElementAttribute("array", typeof(HbmArray))]
        [System.Xml.Serialization.XmlElementAttribute("bag", typeof(HbmBag))]
        [System.Xml.Serialization.XmlElementAttribute("component", typeof(HbmComponent))]
        [System.Xml.Serialization.XmlElementAttribute("dynamic-component", typeof(HbmDynamicComponent))]
        [System.Xml.Serialization.XmlElementAttribute("idbag", typeof(HbmIdbag))]
        [System.Xml.Serialization.XmlElementAttribute("list", typeof(HbmList))]
        [System.Xml.Serialization.XmlElementAttribute("many-to-one", typeof(HbmManyToOne))]
        [System.Xml.Serialization.XmlElementAttribute("map", typeof(HbmMap))]
        [System.Xml.Serialization.XmlElementAttribute("one-to-one", typeof(HbmOneToOne))]
        [System.Xml.Serialization.XmlElementAttribute("primitive-array", typeof(HbmPrimitiveArray))]
        [System.Xml.Serialization.XmlElementAttribute("properties", typeof(HbmProperties))]
        [System.Xml.Serialization.XmlElementAttribute("property", typeof(HbmProperty))]
        [System.Xml.Serialization.XmlElementAttribute("set", typeof(HbmSet))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("union-subclass")]
        public HbmUnionSubclass[] unionsubclass1;
        
        /// <remarks/>
        public HbmLoader loader;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-insert")]
        public HbmCustomSQL sqlinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-update")]
        public HbmCustomSQL sqlupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("sql-delete")]
        public HbmCustomSQL sqldelete;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resultset")]
        public HbmResultSet[] resultset;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query", typeof(HbmQuery))]
        [System.Xml.Serialization.XmlElementAttribute("sql-query", typeof(HbmSqlQuery))]
        public object[] Items1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("entity-name")]
        public string entityname;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string proxy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string table;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("subselect")]
        public string subselect1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("dynamic-insert")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool dynamicinsert;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("select-before-update")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool selectbeforeupdate;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string extends;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool lazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool lazySpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @abstract;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool abstractSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string persister;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string check;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("batch-size", DataType="positiveInteger")]
        public string batchsize;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string node;
        
        public HbmUnionSubclass() {
            this.dynamicupdate = false;
            this.dynamicinsert = false;
            this.selectbeforeupdate = false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmPolymorphismType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("implicit")]
        Implicit,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("explicit")]
        Explicit,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:nhibernate-mapping-2.2")]
    public enum HbmOptimisticLockMode {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("none")]
        None,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("version")]
        Version,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("dirty")]
        Dirty,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("all")]
        All,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("create", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmCreate {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("database-object", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmDatabaseObject {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("create", typeof(HbmCreate))]
        [System.Xml.Serialization.XmlElementAttribute("definition", typeof(HbmDefinition))]
        [System.Xml.Serialization.XmlElementAttribute("drop", typeof(HbmDrop))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("dialect-scope")]
        public HbmDialectScope[] dialectscope;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("definition", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmDefinition {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public HbmParam[] param;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("drop", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmDrop {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("dialect-scope", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmDialectScope {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("filter-def", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmFilterDef {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter-param")]
        public HbmFilterParam[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string condition;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("use-many-to-one")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool usemanytoone;
        
        public HbmFilterDef() {
            this.usemanytoone = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("filter-param", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmFilterParam {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("hibernate-mapping", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmMapping {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("meta")]
        public HbmMeta[] meta;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("typedef")]
        public HbmTypedef[] typedef;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("import")]
        public HbmImport[] import;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("class", typeof(HbmClass))]
        [System.Xml.Serialization.XmlElementAttribute("joined-subclass", typeof(HbmJoinedSubclass))]
        [System.Xml.Serialization.XmlElementAttribute("subclass", typeof(HbmSubclass))]
        [System.Xml.Serialization.XmlElementAttribute("union-subclass", typeof(HbmUnionSubclass))]
        public object[] Items;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("resultset")]
        public HbmResultSet[] resultset;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("query", typeof(HbmQuery))]
        [System.Xml.Serialization.XmlElementAttribute("sql-query", typeof(HbmSqlQuery))]
        public object[] Items1;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("filter-def")]
        public HbmFilterDef[] filterdef;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("database-object")]
        public HbmDatabaseObject[] databaseobject;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string catalog;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-cascade")]
        [System.ComponentModel.DefaultValueAttribute("none")]
        public string defaultcascade;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-access")]
        [System.ComponentModel.DefaultValueAttribute("property")]
        public string defaultaccess;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-lazy")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool defaultlazy;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("auto-import")]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool autoimport;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @namespace;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string assembly;
        
        public HbmMapping() {
            this.defaultcascade = "none";
            this.defaultaccess = "property";
            this.defaultlazy = true;
            this.autoimport = true;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("typedef", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmTypedef {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public HbmParam[] param;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("HbmXsd", "3.2.0.1001")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:nhibernate-mapping-2.2")]
    [System.Xml.Serialization.XmlRootAttribute("import", Namespace="urn:nhibernate-mapping-2.2", IsNullable=false)]
    public partial class HbmImport {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string rename;
    }
}
