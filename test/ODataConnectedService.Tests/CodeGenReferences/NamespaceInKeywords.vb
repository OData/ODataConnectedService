'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On


'Generation date: 03.03.2021 22:42:29
Namespace NamespaceInKeywords.[event].[string].int
        ''' <summary>
        ''' There are no comments for ComplexType in the schema.
        ''' </summary>
    Partial Public Class ComplexType
        ''' <summary>
        ''' Create a new ComplexType object.
        ''' </summary>
        ''' <param name="value">Initial value of Value.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateComplexType(ByVal value As String) As ComplexType
            Dim complexType As ComplexType = New ComplexType()
            complexType.Value = value
            Return complexType
        End Function
        ''' <summary>
        ''' There are no comments for Property Value in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Property Value() As String
            Get
                Return Me._Value
            End Get
            Set
                Me.OnValueChanging(value)
                Me._Value = value
                Me.OnValueChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Value As String
        Partial Private Sub OnValueChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnValueChanged()
        End Sub
    End Class
        ''' <summary>
        ''' There are no comments for TestType1Single in the schema.
        ''' </summary>
    Partial Public Class TestType1Single
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType1)
        ''' <summary>
        ''' Initialize a new TestType1Single object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new TestType1Single object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new TestType1Single object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType1))
            MyBase.New(query)
        End Sub
    End Class
        ''' <summary>
        ''' There are no comments for TestType1 in the schema.
        ''' </summary>
    ''' <KeyProperties>
    ''' KeyProp
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("KeyProp")>  _
    Partial Public Class TestType1
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        ''' <summary>
        ''' Create a new TestType1 object.
        ''' </summary>
        ''' <param name="keyProp">Initial value of KeyProp.</param>
        ''' <param name="valueProp">Initial value of ValueProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateTestType1(ByVal keyProp As Integer, ByVal valueProp As String) As TestType1
            Dim testType1 As TestType1 = New TestType1()
            testType1.KeyProp = keyProp
            testType1.ValueProp = valueProp
            Return testType1
        End Function
        ''' <summary>
        ''' There are no comments for Property KeyProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Property KeyProp() As Integer
            Get
                Return Me._KeyProp
            End Get
            Set
                Me.OnKeyPropChanging(value)
                Me._KeyProp = value
                Me.OnKeyPropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _KeyProp As Integer
        Partial Private Sub OnKeyPropChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnKeyPropChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property ValueProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Property ValueProp() As String
            Get
                Return Me._ValueProp
            End Get
            Set
                Me.OnValuePropChanging(value)
                Me._ValueProp = value
                Me.OnValuePropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _ValueProp As String
        Partial Private Sub OnValuePropChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnValuePropChanged()
        End Sub
    End Class
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type NamespaceInKeywords.[event].[string].int.TestType1 as NamespaceInKeywords.[event].[string].int.TestType1Single specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of NamespaceInKeywords.[event].[string].int.TestType1), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As NamespaceInKeywords.[event].[string].int.TestType1Single
            Return New NamespaceInKeywords.[event].[string].int.TestType1Single(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type NamespaceInKeywords.[event].[string].int.TestType1 as NamespaceInKeywords.[event].[string].int.TestType1Single specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of NamespaceInKeywords.[event].[string].int.TestType1),
            keyProp As Integer) As NamespaceInKeywords.[event].[string].int.TestType1Single
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "KeyProp", keyProp }
            }
            Return New NamespaceInKeywords.[event].[string].int.TestType1Single(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
    End Module
End Namespace
Namespace NamespaceInKeywords.[double]
        ''' <summary>
        ''' There are no comments for EntityContainer in the schema.
        ''' </summary>
    Partial Public Class EntityContainer
        Inherits Global.Microsoft.OData.Client.DataServiceContext
        ''' <summary>
        ''' Initialize a new EntityContainer object.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Sub New(ByVal serviceRoot As Global.System.Uri)
            MyBase.New(serviceRoot, Global.Microsoft.OData.Client.ODataProtocolVersion.V4)
            Me.OnContextCreated
            Me.Format.LoadServiceModel = AddressOf GeneratedEdmModel.GetInstance
            Me.Format.UseJson()
        End Sub
        Partial Private Sub OnContextCreated()
        End Sub
        ''' <summary>
        ''' There are no comments for Set1 in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable ReadOnly Property Set1() As Global.Microsoft.OData.Client.DataServiceQuery(Of NamespaceInKeywords.[event].[string].int.TestType1)
            Get
                If (Me._Set1 Is Nothing) Then
                    Me._Set1 = MyBase.CreateQuery(Of NamespaceInKeywords.[event].[string].int.TestType1)("Set1")
                End If
                Return Me._Set1
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Set1 As Global.Microsoft.OData.Client.DataServiceQuery(Of NamespaceInKeywords.[event].[string].int.TestType1)
        ''' <summary>
        ''' There are no comments for Set2 in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable ReadOnly Property Set2() As Global.Microsoft.OData.Client.DataServiceQuery(Of TestType2)
            Get
                If (Me._Set2 Is Nothing) Then
                    Me._Set2 = MyBase.CreateQuery(Of TestType2)("Set2")
                End If
                Return Me._Set2
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Set2 As Global.Microsoft.OData.Client.DataServiceQuery(Of TestType2)
        ''' <summary>
        ''' There are no comments for Set1 in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Sub AddToSet1(ByVal testType1 As NamespaceInKeywords.[event].[string].int.TestType1)
            MyBase.AddObject("Set1", testType1)
        End Sub
        ''' <summary>
        ''' There are no comments for Set2 in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Sub AddToSet2(ByVal testType2 As TestType2)
            MyBase.AddObject("Set2", testType2)
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private MustInherit Class GeneratedEdmModel
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared ParsedModel As Global.Microsoft.OData.Edm.IEdmModel = LoadModelFromString
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Const Edmx As String = "<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">" & _
 "  <edmx:DataServices>" & _
 "    <Schema Namespace=""NamespaceInKeywords.event.string.int"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <ComplexType Name=""ComplexType"">" & _
 "        <Property Name=""Value"" Type=""Edm.String"" Nullable=""false"" />" & _
 "      </ComplexType>" & _
 "      <EntityType Name=""TestType1"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""KeyProp"" />" & _
 "        </Key>" & _
 "        <Property Name=""KeyProp"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "        <Property Name=""ValueProp"" Type=""Edm.String"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "    </Schema>" & _
 "    <Schema Namespace=""NamespaceInKeywords.double"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityType Name=""TestType2"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""KeyProp"" />" & _
 "        </Key>" & _
 "        <Property Name=""KeyProp"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "        <Property Name=""ValueProp"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <Property Name=""ComplexValueProp"" Type=""NamespaceInKeywords.event.string.int.ComplexType"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "      <EntityContainer Name=""EntityContainer"">" & _
 "        <EntitySet Name=""Set1"" EntityType=""NamespaceInKeywords.event.string.int.TestType1"" />" & _
 "        <EntitySet Name=""Set2"" EntityType=""NamespaceInKeywords.double.TestType2"" />" & _
 "      </EntityContainer>" & _
 "    </Schema>" & _
 "  </edmx:DataServices>" & _
 "</edmx:Edmx>"
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Public Shared Function GetInstance() As Global.Microsoft.OData.Edm.IEdmModel
                Return ParsedModel
            End Function
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared Function LoadModelFromString() As Global.Microsoft.OData.Edm.IEdmModel
                Dim reader As Global.System.Xml.XmlReader = CreateXmlReader(Edmx)
                Try
                    Dim errors As Global.System.Collections.Generic.IEnumerable(Of Global.Microsoft.OData.Edm.Validation.EdmError) = Nothing
                    Dim edmModel As Global.Microsoft.OData.Edm.IEdmModel = Nothing
                    If Not Global.Microsoft.OData.Edm.Csdl.CsdlReader.TryParse(reader, False, edmModel, errors) Then
                        Dim errorMessages As Global.System.Text.StringBuilder = New Global.System.Text.StringBuilder()
                        For Each err As Global.Microsoft.OData.Edm.Validation.EdmError In errors
                            errorMessages.Append(err.ErrorMessage)
                            errorMessages.Append("; ")
                        Next
                        Throw New Global.System.InvalidOperationException(errorMessages.ToString())
                    End If

                    Return edmModel
                Finally
                    CType(reader, Global.System.IDisposable).Dispose()
                End Try
            End Function
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared Function CreateXmlReader(ByVal edmxToParse As String) As Global.System.Xml.XmlReader
                Return Global.System.Xml.XmlReader.Create(New Global.System.IO.StringReader(edmxToParse))
            End Function
        End Class
    End Class
        ''' <summary>
        ''' There are no comments for TestType2Single in the schema.
        ''' </summary>
    Partial Public Class TestType2Single
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType2)
        ''' <summary>
        ''' Initialize a new TestType2Single object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new TestType2Single object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new TestType2Single object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType2))
            MyBase.New(query)
        End Sub
    End Class
        ''' <summary>
        ''' There are no comments for TestType2 in the schema.
        ''' </summary>
    ''' <KeyProperties>
    ''' KeyProp
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("KeyProp")>  _
    Partial Public Class TestType2
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        ''' <summary>
        ''' Create a new TestType2 object.
        ''' </summary>
        ''' <param name="keyProp">Initial value of KeyProp.</param>
        ''' <param name="valueProp">Initial value of ValueProp.</param>
        ''' <param name="complexValueProp">Initial value of ComplexValueProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateTestType2(ByVal keyProp As Integer, ByVal valueProp As String, ByVal complexValueProp As NamespaceInKeywords.[event].[string].int.ComplexType) As TestType2
            Dim testType2 As TestType2 = New TestType2()
            testType2.KeyProp = keyProp
            testType2.ValueProp = valueProp
            If (complexValueProp Is Nothing) Then
                Throw New Global.System.ArgumentNullException("complexValueProp")
            End If
            testType2.ComplexValueProp = complexValueProp
            Return testType2
        End Function
        ''' <summary>
        ''' There are no comments for Property KeyProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Property KeyProp() As Integer
            Get
                Return Me._KeyProp
            End Get
            Set
                Me.OnKeyPropChanging(value)
                Me._KeyProp = value
                Me.OnKeyPropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _KeyProp As Integer
        Partial Private Sub OnKeyPropChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnKeyPropChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property ValueProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Property ValueProp() As String
            Get
                Return Me._ValueProp
            End Get
            Set
                Me.OnValuePropChanging(value)
                Me._ValueProp = value
                Me.OnValuePropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _ValueProp As String
        Partial Private Sub OnValuePropChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnValuePropChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property ComplexValueProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Property ComplexValueProp() As NamespaceInKeywords.[event].[string].int.ComplexType
            Get
                Return Me._ComplexValueProp
            End Get
            Set
                Me.OnComplexValuePropChanging(value)
                Me._ComplexValueProp = value
                Me.OnComplexValuePropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _ComplexValueProp As NamespaceInKeywords.[event].[string].int.ComplexType
        Partial Private Sub OnComplexValuePropChanging(ByVal value As NamespaceInKeywords.[event].[string].int.ComplexType)
        End Sub
        Partial Private Sub OnComplexValuePropChanged()
        End Sub
    End Class
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type NamespaceInKeywords.[double].TestType2 as NamespaceInKeywords.[double].TestType2Single specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of NamespaceInKeywords.[double].TestType2), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As NamespaceInKeywords.[double].TestType2Single
            Return New NamespaceInKeywords.[double].TestType2Single(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type NamespaceInKeywords.[double].TestType2 as NamespaceInKeywords.[double].TestType2Single specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of NamespaceInKeywords.[double].TestType2),
            keyProp As Integer) As NamespaceInKeywords.[double].TestType2Single
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "KeyProp", keyProp }
            }
            Return New NamespaceInKeywords.[double].TestType2Single(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
    End Module
End Namespace
