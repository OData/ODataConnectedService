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


'Generation date: 10.03.2021 19:45:18
Namespace Namespace1
    ''' <summary>
    ''' There are no comments for New in the schema.
    ''' </summary>
    Partial Public Class [New]
        Inherits Global.Microsoft.OData.Client.DataServiceContext
        ''' <summary>
        ''' Initialize a new New object.
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
        ''' There are no comments for double in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable ReadOnly Property [double]() As Global.Microsoft.OData.Client.DataServiceQuery(Of [event])
            Get
                If (Me._double Is Nothing) Then
                    Me._double = MyBase.CreateQuery(Of [event])("double")
                End If
                Return Me._double
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _double As Global.Microsoft.OData.Client.DataServiceQuery(Of [event])
        ''' <summary>
        ''' There are no comments for double in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Sub AddTodouble(ByVal [event] As [event])
            MyBase.AddObject("double", [event])
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private MustInherit Class GeneratedEdmModel
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared ParsedModel As Global.Microsoft.OData.Edm.IEdmModel = LoadModelFromString
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Const Edmx As String = "<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">" & _
 "  <edmx:DataServices>" & _
 "    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityType Name=""event"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""string"" />" & _
 "        </Key>" & _
 "        <Property Name=""string"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <NavigationProperty Name=""event"" Type=""Namespace1.event"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "      <Function Name=""const"" IsBound=""true"">" & _
 "        <Parameter Name=""p0"" Type=""Namespace1.event"" />" & _
 "        <ReturnType Type=""Namespace1.event"" />" & _
 "      </Function>" & _
 "      <Function Name=""short"">" & _
 "        <Parameter Name=""p0"" Type=""Namespace1.event"" />" & _
 "        <ReturnType Type=""Namespace1.event"" />" & _
 "      </Function>" & _
 "      <Action Name=""as"" IsBound=""true"">" & _
 "        <Parameter Name=""p0"" Type=""Namespace1.event"" />" & _
 "        <ReturnType Type=""Namespace1.event"" />" & _
 "      </Action>" & _
 "      <Action Name=""enum"">" & _
 "        <Parameter Name=""p0"" Type=""Namespace1.event"" />" & _
 "        <ReturnType Type=""Namespace1.event"" />" & _
 "      </Action>" & _
 "      <EntityContainer Name=""New"">" & _
 "        <EntitySet Name=""double"" EntityType=""Namespace1.event"">" & _
 "          <NavigationPropertyBinding Path=""event"" Target=""double"" />" & _
 "        </EntitySet>" & _
 "        <FunctionImport Name=""short"" Function=""Namespace1.short"" />" & _
 "        <ActionImport Name=""enum"" Action=""Namespace1.enum"" />" & _
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
        ''' <summary>
        ''' There are no comments for [short] in the schema.
        ''' </summary>
        Public Overridable Function [short](p0 As Namespace1.[event], Optional ByVal useEntityReference As Boolean = False) As Namespace1.eventSingle
            Return New Namespace1.eventSingle(Me.CreateFunctionQuerySingle(Of Namespace1.[event])("", "/short", False, New Global.Microsoft.OData.Client.UriEntityOperationParameter("p0", p0, useEntityReference)))
        End Function
        ''' <summary>
        ''' There are no comments for [enum] in the schema.
        ''' </summary>
        Public Overridable Function [enum](p0 As Namespace1.[event]) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Namespace1.[event])
            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Namespace1.[event])(Me, Me.BaseUri.OriginalString.Trim("/"C) + "/enum", New Global.Microsoft.OData.Client.BodyOperationParameter("p0", p0))
        End Function
    End Class
    ''' <summary>
    ''' There are no comments for eventSingle in the schema.
    ''' </summary>
    Partial Public Class eventSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [event])
        ''' <summary>
        ''' Initialize a new eventSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new eventSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new eventSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [event]))
            MyBase.New(query)
        End Sub
        ''' <summary>
        ''' There are no comments for event in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable ReadOnly Property [event]() As Namespace1.eventSingle
            Get
                If Not Me.IsComposable Then
                    Throw New Global.System.NotSupportedException("The previous function is not composable.")
                End If
                If (Me._event Is Nothing) Then
                    Me._event = New Namespace1.eventSingle(Me.Context, GetPath("event"))
                End If
                Return Me._event
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _event As Namespace1.eventSingle
    End Class
    ''' <summary>
    ''' There are no comments for event in the schema.
    ''' </summary>
    ''' <KeyProperties>
    ''' string
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("string")>  _
    Partial Public Class [event]
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        ''' <summary>
        ''' Create a new event object.
        ''' </summary>
        ''' <param name="string">Initial value of string.</param>
        ''' <param name="event1">Initial value of event.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function Createevent(ByVal [string] As String, ByVal event1 As Namespace1.[event]) As [event]
            Dim [event] As [event] = New [event]()
            [event].[string] = [string]
            If (event1 Is Nothing) Then
                Throw New Global.System.ArgumentNullException("event1")
            End If
            [event].[event] = event1
            Return [event]
        End Function
        ''' <summary>
        ''' There are no comments for Property string in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property [string]() As String
            Get
                Return Me._string
            End Get
            Set
                Me.OnstringChanging(value)
                Me._string = value
                Me.OnstringChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _string As String
        Partial Private Sub OnstringChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnstringChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property event in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property [event]() As Namespace1.[event]
            Get
                Return Me._event
            End Get
            Set
                Me.OneventChanging(value)
                Me._event = value
                Me.OneventChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _event As Namespace1.[event]
        Partial Private Sub OneventChanging(ByVal value As Namespace1.[event])
        End Sub
        Partial Private Sub OneventChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for [const] in the schema.
        ''' </summary>
        Public Overridable Function [const]() As Namespace1.eventSingle
            Dim requestUri As Global.System.Uri = Nothing
            Context.TryGetUri(Me, requestUri)
            Return New Namespace1.eventSingle(Me.Context.CreateFunctionQuerySingle(Of Namespace1.[event])(String.Join("/", Global.System.Linq.Enumerable.Select(Global.System.Linq.Enumerable.Skip(requestUri.Segments, Me.Context.BaseUri.Segments.Length), Function(s) s.Trim("/"C))), "/Namespace1.const", False))
        End Function
        ''' <summary>
        ''' There are no comments for [as] in the schema.
        ''' </summary>
        Public Overridable Function [as]() As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Namespace1.[event])
            Dim resource As Global.Microsoft.OData.Client.EntityDescriptor = Context.EntityTracker.TryGetEntityDescriptor(Me)
            If resource Is Nothing Then
                Throw New Global.System.Exception("cannot find entity")
            End If

            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Namespace1.[event])(Me.Context, resource.EditLink.OriginalString.Trim("/"C) + "/Namespace1.as")
        End Function
    End Class
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type Namespace1.[event] as Namespace1.eventSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of Namespace1.[event]), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As Namespace1.eventSingle
            Return New Namespace1.eventSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type Namespace1.[event] as Namespace1.eventSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="string">The value of string</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of Namespace1.[event]),
            [string] As String) As Namespace1.eventSingle
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "string", [string] }
            }
            Return New Namespace1.eventSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' There are no comments for [const] in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function [const](ByVal _source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of Namespace1.[event])) As Namespace1.eventSingle
            If Not _source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If

            Return New Namespace1.eventSingle(_source.CreateFunctionQuerySingle(Of Namespace1.[event])("Namespace1.const", False))
        End Function
        ''' <summary>
        ''' There are no comments for [as] in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function [as](ByVal _source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of Namespace1.[event])) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Namespace1.[event])
            If Not _source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Namespace1.[event])(_source.Context, _source.AppendRequestUri("Namespace1.as"))
        End Function
    End Module
End Namespace
