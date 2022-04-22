//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generation date: 08.03.2021 22:46:13
namespace DupNames
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
        public virtual global::Microsoft.OData.Client.DataServiceQuery<DupWithTypeName> Set1
        {
            get
            {
                if ((this._Set1 == null))
                {
                    this._Set1 = base.CreateQuery<DupWithTypeName>("Set1");
                }
                return this._Set1;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<DupWithTypeName> _Set1;
        /// <summary>
        /// There are no comments for Set2 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<DupWithTypeName1> Set2
        {
            get
            {
                if ((this._Set2 == null))
                {
                    this._Set2 = base.CreateQuery<DupWithTypeName1>("Set2");
                }
                return this._Set2;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<DupWithTypeName1> _Set2;
        /// <summary>
        /// There are no comments for Set1 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual void AddToSet1(DupWithTypeName dupWithTypeName)
        {
            base.AddObject("Set1", dupWithTypeName);
        }
        /// <summary>
        /// There are no comments for Set2 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual void AddToSet2(DupWithTypeName1 dupWithTypeName1)
        {
            base.AddObject("Set2", dupWithTypeName1);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DupNames"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""DupWithTypeName"">
        <Key>
          <PropertyRef Name=""DupWithTypeName"" />
        </Key>
        <Property Name=""DupWithTypeName"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""dupWithTypeName"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""DupWithTypeName1"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""dupWithTypeName1"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""DupWithTypeName2"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""DupWithTypeName3"" Type=""DupNames.DupWithComplexTypeName"" Nullable=""false"" />
        <Property Name=""DupPropertyName"" Type=""Edm.Int32"" Nullable=""true"" />
        <NavigationProperty Name=""dupPropertyName"" Type=""Collection(DupNames.DupWithTypeName1)"" />
      </EntityType>
      <ComplexType Name=""DupWithComplexTypeName"">
        <Property Name=""DupWithComplexTypeName"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""dupWithComplexTypeName"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""DupWithComplexTypeName1"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""_DupWithComplexTypeName2"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""__DupWithComplexTypeName2"" Type=""Edm.Int32"" Nullable=""true"" />
      </ComplexType>
      <EntityType Name=""DupWithTypeName1"">
        <Key>
          <PropertyRef Name=""DupWithTypeName"" />
        </Key>
        <Property Name=""dupWithTypeName"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""dupwithtypeName"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""DupWithTypeName"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""DupWithTypeName1"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""DupWithTypeName3"" Type=""Edm.Int32"" Nullable=""true"" />
      </EntityType>
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""DupNames.DupWithTypeName"" />
        <EntitySet Name=""Set2"" EntityType=""DupNames.DupWithTypeName1"">
          <NavigationPropertyBinding Path=""Set1"" Target=""Set1"" />
        </EntitySet>
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
    /// There are no comments for DupWithTypeNameSingle in the schema.
    /// </summary>
    public partial class DupWithTypeNameSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<DupWithTypeName>
    {
        /// <summary>
        /// Initialize a new DupWithTypeNameSingle object.
        /// </summary>
        public DupWithTypeNameSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new DupWithTypeNameSingle object.
        /// </summary>
        public DupWithTypeNameSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new DupWithTypeNameSingle object.
        /// </summary>
        public DupWithTypeNameSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<DupWithTypeName> query)
            : base(query) { }

        /// <summary>
        /// There are no comments for dupPropertyName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<global::DupNames.DupWithTypeName1> dupPropertyName
        {
            get
            {
                if (!this.IsComposable)
                {
                    throw new global::System.NotSupportedException("The previous function is not composable.");
                }
                if ((this._dupPropertyName == null))
                {
                    this._dupPropertyName = Context.CreateQuery<global::DupNames.DupWithTypeName1>(GetPath("dupPropertyName"));
                }
                return this._dupPropertyName;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<global::DupNames.DupWithTypeName1> _dupPropertyName;
    }
    /// <summary>
    /// There are no comments for DupWithTypeName in the schema.
    /// </summary>
    /// <KeyProperties>
    /// DupWithTypeName
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("DupWithTypeName")]
    public partial class DupWithTypeName : global::Microsoft.OData.Client.BaseEntityType
    {
        /// <summary>
        /// Create a new DupWithTypeName object.
        /// </summary>
        /// <param name="dupWithTypeName4">Initial value of DupWithTypeName4.</param>
        /// <param name="dupWithTypeName1">Initial value of DupWithTypeName1.</param>
        /// <param name="dupWithTypeName11">Initial value of dupWithTypeName1.</param>
        /// <param name="dupWithTypeName2">Initial value of DupWithTypeName2.</param>
        /// <param name="dupWithTypeName3">Initial value of DupWithTypeName3.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public static DupWithTypeName CreateDupWithTypeName(global::System.Guid dupWithTypeName4, global::System.Guid dupWithTypeName1, global::System.Guid dupWithTypeName11, global::System.Guid dupWithTypeName2, global::DupNames.DupWithComplexTypeName dupWithTypeName3)
        {
            DupWithTypeName dupWithTypeName = new DupWithTypeName();
            dupWithTypeName.DupWithTypeName4 = dupWithTypeName4;
            dupWithTypeName.DupWithTypeName1 = dupWithTypeName1;
            dupWithTypeName.dupWithTypeName1 = dupWithTypeName11;
            dupWithTypeName.DupWithTypeName2 = dupWithTypeName2;
            if ((dupWithTypeName3 == null))
            {
                throw new global::System.ArgumentNullException("dupWithTypeName3");
            }
            dupWithTypeName.DupWithTypeName3 = dupWithTypeName3;
            return dupWithTypeName;
        }
        /// <summary>
        /// There are no comments for Property DupWithTypeName4 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("DupWithTypeName")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "DupWithTypeName4 is required.")]
        public virtual global::System.Guid DupWithTypeName4
        {
            get
            {
                return this._DupWithTypeName4;
            }
            set
            {
                this.OnDupWithTypeName4Changing(value);
                this._DupWithTypeName4 = value;
                this.OnDupWithTypeName4Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _DupWithTypeName4;
        partial void OnDupWithTypeName4Changing(global::System.Guid value);
        partial void OnDupWithTypeName4Changed();
        /// <summary>
        /// There are no comments for Property dupWithTypeName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> dupWithTypeName
        {
            get
            {
                return this._dupWithTypeName;
            }
            set
            {
                this.OndupWithTypeNameChanging(value);
                this._dupWithTypeName = value;
                this.OndupWithTypeNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _dupWithTypeName;
        partial void OndupWithTypeNameChanging(global::System.Nullable<int> value);
        partial void OndupWithTypeNameChanged();
        /// <summary>
        /// There are no comments for Property DupWithTypeName1 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "DupWithTypeName1 is required.")]
        public virtual global::System.Guid DupWithTypeName1
        {
            get
            {
                return this._DupWithTypeName1;
            }
            set
            {
                this.OnDupWithTypeName1Changing(value);
                this._DupWithTypeName1 = value;
                this.OnDupWithTypeName1Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _DupWithTypeName1;
        partial void OnDupWithTypeName1Changing(global::System.Guid value);
        partial void OnDupWithTypeName1Changed();
        /// <summary>
        /// There are no comments for Property dupWithTypeName1 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "dupWithTypeName1 is required.")]
        public virtual global::System.Guid dupWithTypeName1
        {
            get
            {
                return this._dupWithTypeName1;
            }
            set
            {
                this.OndupWithTypeName1Changing(value);
                this._dupWithTypeName1 = value;
                this.OndupWithTypeName1Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _dupWithTypeName1;
        partial void OndupWithTypeName1Changing(global::System.Guid value);
        partial void OndupWithTypeName1Changed();
        /// <summary>
        /// There are no comments for Property DupWithTypeName2 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "DupWithTypeName2 is required.")]
        public virtual global::System.Guid DupWithTypeName2
        {
            get
            {
                return this._DupWithTypeName2;
            }
            set
            {
                this.OnDupWithTypeName2Changing(value);
                this._DupWithTypeName2 = value;
                this.OnDupWithTypeName2Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _DupWithTypeName2;
        partial void OnDupWithTypeName2Changing(global::System.Guid value);
        partial void OnDupWithTypeName2Changed();
        /// <summary>
        /// There are no comments for Property DupWithTypeName3 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "DupWithTypeName3 is required.")]
        public virtual global::DupNames.DupWithComplexTypeName DupWithTypeName3
        {
            get
            {
                return this._DupWithTypeName3;
            }
            set
            {
                this.OnDupWithTypeName3Changing(value);
                this._DupWithTypeName3 = value;
                this.OnDupWithTypeName3Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::DupNames.DupWithComplexTypeName _DupWithTypeName3;
        partial void OnDupWithTypeName3Changing(global::DupNames.DupWithComplexTypeName value);
        partial void OnDupWithTypeName3Changed();
        /// <summary>
        /// There are no comments for Property DupPropertyName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> DupPropertyName
        {
            get
            {
                return this._DupPropertyName;
            }
            set
            {
                this.OnDupPropertyNameChanging(value);
                this._DupPropertyName = value;
                this.OnDupPropertyNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _DupPropertyName;
        partial void OnDupPropertyNameChanging(global::System.Nullable<int> value);
        partial void OnDupPropertyNameChanged();
        /// <summary>
        /// There are no comments for Property dupPropertyName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Collections.ObjectModel.Collection<global::DupNames.DupWithTypeName1> dupPropertyName
        {
            get
            {
                return this._dupPropertyName;
            }
            set
            {
                this.OndupPropertyNameChanging(value);
                this._dupPropertyName = value;
                this.OndupPropertyNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Collections.ObjectModel.Collection<global::DupNames.DupWithTypeName1> _dupPropertyName = new global::System.Collections.ObjectModel.Collection<global::DupNames.DupWithTypeName1>();
        partial void OndupPropertyNameChanging(global::System.Collections.ObjectModel.Collection<global::DupNames.DupWithTypeName1> value);
        partial void OndupPropertyNameChanged();
    }
    /// <summary>
    /// There are no comments for DupWithComplexTypeName in the schema.
    /// </summary>
    public partial class DupWithComplexTypeName
    {
        /// <summary>
        /// There are no comments for Property DupWithComplexTypeName2 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("DupWithComplexTypeName")]
        public virtual global::System.Nullable<int> DupWithComplexTypeName2
        {
            get
            {
                return this._DupWithComplexTypeName21;
            }
            set
            {
                this.OnDupWithComplexTypeName2Changing(value);
                this._DupWithComplexTypeName21 = value;
                this.OnDupWithComplexTypeName2Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _DupWithComplexTypeName21;
        partial void OnDupWithComplexTypeName2Changing(global::System.Nullable<int> value);
        partial void OnDupWithComplexTypeName2Changed();
        /// <summary>
        /// There are no comments for Property dupWithComplexTypeName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> dupWithComplexTypeName
        {
            get
            {
                return this._dupWithComplexTypeName;
            }
            set
            {
                this.OndupWithComplexTypeNameChanging(value);
                this._dupWithComplexTypeName = value;
                this.OndupWithComplexTypeNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _dupWithComplexTypeName;
        partial void OndupWithComplexTypeNameChanging(global::System.Nullable<int> value);
        partial void OndupWithComplexTypeNameChanged();
        /// <summary>
        /// There are no comments for Property DupWithComplexTypeName1 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual string DupWithComplexTypeName1
        {
            get
            {
                return this._DupWithComplexTypeName1;
            }
            set
            {
                this.OnDupWithComplexTypeName1Changing(value);
                this._DupWithComplexTypeName1 = value;
                this.OnDupWithComplexTypeName1Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private string _DupWithComplexTypeName1;
        partial void OnDupWithComplexTypeName1Changing(string value);
        partial void OnDupWithComplexTypeName1Changed();
        /// <summary>
        /// There are no comments for Property _DupWithComplexTypeName2 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> _DupWithComplexTypeName2
        {
            get
            {
                return this.__DupWithComplexTypeName21;
            }
            set
            {
                this.On_DupWithComplexTypeName2Changing(value);
                this.__DupWithComplexTypeName21 = value;
                this.On_DupWithComplexTypeName2Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> __DupWithComplexTypeName21;
        partial void On_DupWithComplexTypeName2Changing(global::System.Nullable<int> value);
        partial void On_DupWithComplexTypeName2Changed();
        /// <summary>
        /// There are no comments for Property __DupWithComplexTypeName2 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> __DupWithComplexTypeName2
        {
            get
            {
                return this.___DupWithComplexTypeName2;
            }
            set
            {
                this.On__DupWithComplexTypeName2Changing(value);
                this.___DupWithComplexTypeName2 = value;
                this.On__DupWithComplexTypeName2Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> ___DupWithComplexTypeName2;
        partial void On__DupWithComplexTypeName2Changing(global::System.Nullable<int> value);
        partial void On__DupWithComplexTypeName2Changed();
    }
    /// <summary>
    /// There are no comments for DupWithTypeName1Single in the schema.
    /// </summary>
    public partial class DupWithTypeName1Single : global::Microsoft.OData.Client.DataServiceQuerySingle<DupWithTypeName1>
    {
        /// <summary>
        /// Initialize a new DupWithTypeName1Single object.
        /// </summary>
        public DupWithTypeName1Single(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new DupWithTypeName1Single object.
        /// </summary>
        public DupWithTypeName1Single(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new DupWithTypeName1Single object.
        /// </summary>
        public DupWithTypeName1Single(global::Microsoft.OData.Client.DataServiceQuerySingle<DupWithTypeName1> query)
            : base(query) { }

    }
    /// <summary>
    /// There are no comments for DupWithTypeName1 in the schema.
    /// </summary>
    /// <KeyProperties>
    /// DupWithTypeName
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("DupWithTypeName")]
    public partial class DupWithTypeName1 : global::Microsoft.OData.Client.BaseEntityType
    {
        /// <summary>
        /// Create a new DupWithTypeName1 object.
        /// </summary>
        /// <param name="dupWithTypeName">Initial value of dupWithTypeName.</param>
        /// <param name="dupwithtypeName">Initial value of dupwithtypeName.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public static DupWithTypeName1 CreateDupWithTypeName1(global::System.Guid dupWithTypeName, global::System.Guid dupwithtypeName)
        {
            DupWithTypeName1 dupWithTypeName1 = new DupWithTypeName1();
            dupWithTypeName1.dupWithTypeName = dupWithTypeName;
            dupWithTypeName1.dupwithtypeName = dupwithtypeName;
            return dupWithTypeName1;
        }
        /// <summary>
        /// There are no comments for Property dupWithTypeName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "dupWithTypeName is required.")]
        public virtual global::System.Guid dupWithTypeName
        {
            get
            {
                return this._dupWithTypeName;
            }
            set
            {
                this.OndupWithTypeNameChanging(value);
                this._dupWithTypeName = value;
                this.OndupWithTypeNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _dupWithTypeName;
        partial void OndupWithTypeNameChanging(global::System.Guid value);
        partial void OndupWithTypeNameChanged();
        /// <summary>
        /// There are no comments for Property dupwithtypeName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "dupwithtypeName is required.")]
        public virtual global::System.Guid dupwithtypeName
        {
            get
            {
                return this._dupwithtypeName;
            }
            set
            {
                this.OndupwithtypeNameChanging(value);
                this._dupwithtypeName = value;
                this.OndupwithtypeNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Guid _dupwithtypeName;
        partial void OndupwithtypeNameChanging(global::System.Guid value);
        partial void OndupwithtypeNameChanged();
        /// <summary>
        /// There are no comments for Property DupWithTypeName in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> DupWithTypeName
        {
            get
            {
                return this._DupWithTypeName;
            }
            set
            {
                this.OnDupWithTypeNameChanging(value);
                this._DupWithTypeName = value;
                this.OnDupWithTypeNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _DupWithTypeName;
        partial void OnDupWithTypeNameChanging(global::System.Nullable<int> value);
        partial void OnDupWithTypeNameChanged();
        /// <summary>
        /// There are no comments for Property DupWithTypeName11 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("DupWithTypeName1")]
        public virtual global::System.Nullable<int> DupWithTypeName11
        {
            get
            {
                return this._DupWithTypeName11;
            }
            set
            {
                this.OnDupWithTypeName11Changing(value);
                this._DupWithTypeName11 = value;
                this.OnDupWithTypeName11Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _DupWithTypeName11;
        partial void OnDupWithTypeName11Changing(global::System.Nullable<int> value);
        partial void OnDupWithTypeName11Changed();
        /// <summary>
        /// There are no comments for Property DupWithTypeName3 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::System.Nullable<int> DupWithTypeName3
        {
            get
            {
                return this._DupWithTypeName3;
            }
            set
            {
                this.OnDupWithTypeName3Changing(value);
                this._DupWithTypeName3 = value;
                this.OnDupWithTypeName3Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::System.Nullable<int> _DupWithTypeName3;
        partial void OnDupWithTypeName3Changing(global::System.Nullable<int> value);
        partial void OnDupWithTypeName3Changed();
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::DupNames.DupWithTypeName as global::DupNames.DupWithTypeNameSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::DupNames.DupWithTypeNameSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::DupNames.DupWithTypeName> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::DupNames.DupWithTypeNameSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::DupNames.DupWithTypeName as global::DupNames.DupWithTypeNameSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="dupWithTypeName">The value of dupWithTypeName</param>
        public static global::DupNames.DupWithTypeNameSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::DupNames.DupWithTypeName> _source,
            global::System.Guid dupWithTypeName)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "DupWithTypeName", dupWithTypeName }
            };
            return new global::DupNames.DupWithTypeNameSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::DupNames.DupWithTypeName1 as global::DupNames.DupWithTypeName1Single specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::DupNames.DupWithTypeName1Single ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::DupNames.DupWithTypeName1> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::DupNames.DupWithTypeName1Single(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::DupNames.DupWithTypeName1 as global::DupNames.DupWithTypeName1Single specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="dupWithTypeName">The value of dupWithTypeName</param>
        public static global::DupNames.DupWithTypeName1Single ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::DupNames.DupWithTypeName1> _source,
            global::System.Nullable<int> dupWithTypeName)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "DupWithTypeName", dupWithTypeName }
            };
            return new global::DupNames.DupWithTypeName1Single(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
    }
}
