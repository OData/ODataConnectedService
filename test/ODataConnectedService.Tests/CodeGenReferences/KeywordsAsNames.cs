//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generation date: 08.03.2021 23:03:56
namespace Namespace1
{
    /// <summary>
    /// There are no comments for New in the schema.
    /// </summary>
    public partial class New : global::Microsoft.OData.Client.DataServiceContext
    {
        /// <summary>
        /// Initialize a new New object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public New(global::System.Uri serviceRoot) :
                this(serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion.V4)
        {
        }
        /// <summary>
        /// Initialize a new New object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public New(global::System.Uri serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion protocolVersion) :
                base(serviceRoot, protocolVersion)
        {
            this.OnContextCreated();
            this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
            this.Format.UseJson();
        }
        partial void OnContextCreated();
        /// <summary>
        /// There are no comments for double in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<@event> @double
        {
            get
            {
                if ((this._double == null))
                {
                    this._double = base.CreateQuery<@event>("double");
                }
                return this._double;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Microsoft.OData.Client.DataServiceQuery<@event> _double;
        /// <summary>
        /// There are no comments for double in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual void AddTodouble(@event @event)
        {
            base.AddObject("double", @event);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
            private const string Edmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""event"">
        <Key>
          <PropertyRef Name=""string"" />
        </Key>
        <Property Name=""string"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""event"" Type=""Namespace1.event"" Nullable=""false"" />
      </EntityType>
      <Function Name=""const"" IsBound=""true"">
        <Parameter Name=""p0"" Type=""Namespace1.event"" />
        <ReturnType Type=""Namespace1.event"" />
      </Function>
      <Function Name=""short"">
        <Parameter Name=""p0"" Type=""Namespace1.event"" />
        <ReturnType Type=""Namespace1.event"" />
      </Function>
      <Action Name=""as"" IsBound=""true"">
        <Parameter Name=""p0"" Type=""Namespace1.event"" />
        <ReturnType Type=""Namespace1.event"" />
      </Action>
      <Action Name=""enum"">
        <Parameter Name=""p0"" Type=""Namespace1.event"" />
        <ReturnType Type=""Namespace1.event"" />
      </Action>
      <EntityContainer Name=""New"">
        <EntitySet Name=""double"" EntityType=""Namespace1.event"">
          <NavigationPropertyBinding Path=""event"" Target=""double"" />
        </EntitySet>
        <FunctionImport Name=""short"" Function=""Namespace1.short"" />
        <ActionImport Name=""enum"" Action=""Namespace1.enum"" />
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
        /// <summary>
        /// There are no comments for @short in the schema.
        /// </summary>
        public virtual global::Namespace1.eventSingle @short(global::Namespace1.@event p0, bool useEntityReference = false)
        {
            return new global::Namespace1.eventSingle(this.CreateFunctionQuerySingle<global::Namespace1.@event>("", "short", false, new global::Microsoft.OData.Client.UriEntityOperationParameter("p0", p0, useEntityReference)));
        }
        /// <summary>
        /// There are no comments for @enum in the schema.
        /// </summary>
        public virtual global::Microsoft.OData.Client.DataServiceActionQuerySingle<global::Namespace1.@event> @enum(global::Namespace1.@event p0)
        {
            return new global::Microsoft.OData.Client.DataServiceActionQuerySingle<global::Namespace1.@event>(this, this.BaseUri.OriginalString.Trim('/') + "/enum", new global::Microsoft.OData.Client.BodyOperationParameter("p0", p0));
        }
    }
    /// <summary>
    /// There are no comments for eventSingle in the schema.
    /// </summary>
    public partial class eventSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<@event>
    {
        /// <summary>
        /// Initialize a new eventSingle object.
        /// </summary>
        public eventSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new eventSingle object.
        /// </summary>
        public eventSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new eventSingle object.
        /// </summary>
        public eventSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<@event> query)
            : base(query) { }

        /// <summary>
        /// There are no comments for event in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public virtual global::Namespace1.eventSingle @event
        {
            get
            {
                if (!this.IsComposable)
                {
                    throw new global::System.NotSupportedException("The previous function is not composable.");
                }
                if ((this._event == null))
                {
                    this._event = new global::Namespace1.eventSingle(this.Context, GetPath("event"));
                }
                return this._event;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Namespace1.eventSingle _event;
    }
    /// <summary>
    /// There are no comments for event in the schema.
    /// </summary>
    /// <KeyProperties>
    /// string
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("string")]
    public partial class @event : global::Microsoft.OData.Client.BaseEntityType
    {
        /// <summary>
        /// Create a new event object.
        /// </summary>
        /// <param name="string">Initial value of string.</param>
        /// <param name="event1">Initial value of event1.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        public static @event Createevent(string @string, global::Namespace1.@event event1)
        {
            @event @event = new @event();
            @event.@string = @string;
            if ((event1 == null))
            {
                throw new global::System.ArgumentNullException("event1");
            }
            @event.event1 = event1;
            return @event;
        }
        /// <summary>
        /// There are no comments for Property string in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "string is required.")]
        public virtual string @string
        {
            get
            {
                return this._string;
            }
            set
            {
                this.OnstringChanging(value);
                this._string = value;
                this.OnstringChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private string _string;
        partial void OnstringChanging(string value);
        partial void OnstringChanged();
        /// <summary>
        /// There are no comments for Property event1 in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        [global::Microsoft.OData.Client.OriginalNameAttribute("event")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "event1 is required.")]
        public virtual global::Namespace1.@event event1
        {
            get
            {
                return this._event1;
            }
            set
            {
                this.Onevent1Changing(value);
                this._event1 = value;
                this.Onevent1Changed();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")]
        private global::Namespace1.@event _event1;
        partial void Onevent1Changing(global::Namespace1.@event value);
        partial void Onevent1Changed();
        /// <summary>
        /// There are no comments for @const in the schema.
        /// </summary>
        public virtual global::Namespace1.eventSingle @const()
        {
            global::System.Uri requestUri;
            Context.TryGetUri(this, out requestUri);

            return new global::Namespace1.eventSingle(this.Context.CreateFunctionQuerySingle<global::Namespace1.@event>(string.Join("/", global::System.Linq.Enumerable.Select(global::System.Linq.Enumerable.Skip(requestUri.Segments, this.Context.BaseUri.Segments.Length), s => s.Trim('/'))), "Namespace1.const", false));
        }
        /// <summary>
        /// There are no comments for @as in the schema.
        /// </summary>
        public virtual global::Microsoft.OData.Client.DataServiceActionQuerySingle<global::Namespace1.@event> @as()
        {
            global::Microsoft.OData.Client.EntityDescriptor resource = Context.EntityTracker.TryGetEntityDescriptor(this);
            if (resource == null)
            {
                throw new global::System.Exception("cannot find entity");
            }

            return new global::Microsoft.OData.Client.DataServiceActionQuerySingle<global::Namespace1.@event>(this.Context, resource.EditLink.OriginalString.Trim('/') + "/Namespace1.as");
        }
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::Namespace1.@event as global::Namespace1.eventSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::Namespace1.eventSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Namespace1.@event> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::Namespace1.eventSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::Namespace1.@event as global::Namespace1.eventSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="string">The value of string</param>
        public static global::Namespace1.eventSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::Namespace1.@event> _source,
            string @string)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "string", @string }
            };
            return new global::Namespace1.eventSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// There are no comments for @const in the schema.
        /// </summary>
        public static global::Namespace1.eventSingle @const(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Namespace1.@event> _source)
        {
            if (!_source.IsComposable)
            {
                throw new global::System.NotSupportedException("The previous function is not composable.");
            }

            return new global::Namespace1.eventSingle(_source.CreateFunctionQuerySingle<global::Namespace1.@event>("Namespace1.const", false));
        }
        /// <summary>
        /// There are no comments for @as in the schema.
        /// </summary>
        public static global::Microsoft.OData.Client.DataServiceActionQuerySingle<global::Namespace1.@event> @as(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::Namespace1.@event> _source)
        {
            if (!_source.IsComposable)
            {
                throw new global::System.NotSupportedException("The previous function is not composable.");
            }

            return new global::Microsoft.OData.Client.DataServiceActionQuerySingle<global::Namespace1.@event>(_source.Context, _source.AppendRequestUri("Namespace1.as"));
        }
    }
}
