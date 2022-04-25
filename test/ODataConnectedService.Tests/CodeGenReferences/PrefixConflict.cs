//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generation date: 08.03.2021 22:52:16
namespace PrefixConflict
{
    /// <summary>
    /// There are no comments for EntityContainer in the schema.
    /// </summary>
    public partial class EntityContainer : global::Microsoft.OData.Client.DataServiceContext
    {
        /// <summary>
        /// Initialize a new EntityContainer object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public EntityContainer(global::System.Uri serviceRoot) :
                this(serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion.V4)
        {
        }
        /// <summary>
        /// Initialize a new EntityContainer object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public EntityContainer(global::System.Uri serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion protocolVersion) :
                base(serviceRoot, protocolVersion)
        {
            this.OnContextCreated();
            this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
            this.Format.UseJson();
        }
        partial void OnContextCreated();
        /// <summary>
        /// There are no comments for Set1 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<EntityType> Set1
        {
            get
            {
                if ((this._Set1 == null))
                {
                    this._Set1 = base.CreateQuery<EntityType>("Set1");
                }
                return this._Set1;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<EntityType> _Set1;
        /// <summary>
        /// There are no comments for Set1 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual void AddToSet1(EntityType entityType)
        {
            base.AddObject("Set1", entityType);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""PrefixConflict"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""_Name"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""__Name"" Type=""Edm.Int32"" Nullable=""true"" />
      </EntityType>
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""PrefixConflict.EntityType"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            public static global::Microsoft.OData.Edm.IEdmModel GetInstance()
            {
                return ParsedModel;
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
            {
                global::System.Xml.XmlReader reader = CreateXmlReader(Edmx);
                try
                {
                    global::System.Collections.Generic.IEnumerable<global::Microsoft.OData.Edm.Validation.EdmError> errors;
                    global::Microsoft.OData.Edm.IEdmModel edmModel;

                    if (!global::Microsoft.OData.Edm.Csdl.CsdlReader.TryParse(reader, false, out edmModel, out errors))
                    {
                        global::System.Text.StringBuilder errorMessages = new global::System.Text.StringBuilder();
                        foreach (var error in errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                            errorMessages.Append("; ");
                        }
                        throw new global::System.InvalidOperationException(errorMessages.ToString());
                    }

                    return edmModel;
                }
                finally
                {
                    ((global::System.IDisposable)(reader)).Dispose();
                }
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::System.Xml.XmlReader CreateXmlReader(string edmxToParse)
            {
                return global::System.Xml.XmlReader.Create(new global::System.IO.StringReader(edmxToParse));
            }

        }
    }
    /// <summary>
    /// There are no comments for EntityTypeSingle in the schema.
    /// </summary>
    public partial class EntityTypeSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<EntityType>
    {
        /// <summary>
        /// Initialize a new EntityTypeSingle object.
        /// </summary>
        public EntityTypeSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new EntityTypeSingle object.
        /// </summary>
        public EntityTypeSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new EntityTypeSingle object.
        /// </summary>
        public EntityTypeSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<EntityType> query)
            : base(query) { }

    }
    /// <summary>
    /// There are no comments for EntityType in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Id
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("Id")]
    public partial class EntityType : global::Microsoft.OData.Client.BaseEntityType
    {
        /// <summary>
        /// Create a new EntityType object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public static EntityType CreateEntityType(global::System.Guid ID)
        {
            EntityType entityType = new EntityType();
            entityType.Id = ID;
            return entityType;
        }
        /// <summary>
        /// There are no comments for Property Id in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Id is required.")]
        public virtual global::System.Guid Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this.OnIdChanging(value);
                this._Id = value;
                this.OnIdChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _Id;
        partial void OnIdChanging(global::System.Guid value);
        partial void OnIdChanged();
        /// <summary>
        /// There are no comments for Property Name in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> Name
        {
            get
            {
                return this._Name1;
            }
            set
            {
                this.OnNameChanging(value);
                this._Name1 = value;
                this.OnNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _Name1;
        partial void OnNameChanging(global::System.Nullable<int> value);
        partial void OnNameChanged();
        /// <summary>
        /// There are no comments for Property _Name in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual string _Name
        {
            get
            {
                return this.__Name1;
            }
            set
            {
                this.On_NameChanging(value);
                this.__Name1 = value;
                this.On_NameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private string __Name1;
        partial void On_NameChanging(string value);
        partial void On_NameChanged();
        /// <summary>
        /// There are no comments for Property __Name in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> __Name
        {
            get
            {
                return this.___Name;
            }
            set
            {
                this.On__NameChanging(value);
                this.___Name = value;
                this.On__NameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> ___Name;
        partial void On__NameChanging(global::System.Nullable<int> value);
        partial void On__NameChanged();
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::PrefixConflict.EntityType as global::PrefixConflict.EntityTypeSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::PrefixConflict.EntityTypeSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::PrefixConflict.EntityType> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::PrefixConflict.EntityTypeSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::PrefixConflict.EntityType as global::PrefixConflict.EntityTypeSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::PrefixConflict.EntityTypeSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::PrefixConflict.EntityType> _source,
            global::System.Guid id)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::PrefixConflict.EntityTypeSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
    }
}
