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


'Generation date: 10.03.2021 19:48:40
Namespace Microsoft.OData.TestService.DSC
    ''' <summary>
    ''' There are no comments for PersonSingle in the schema.
    ''' </summary>
    Partial Public Class PersonSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of Person)
        ''' <summary>
        ''' Initialize a new PersonSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new PersonSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new PersonSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of Person))
            MyBase.New(query)
        End Sub
    End Class
    ''' <summary>
    ''' There are no comments for Person in the schema.
    ''' </summary>
    ''' <KeyProperties>
    ''' Keys
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("Keys")>  _
    Partial Public Class Person
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        Implements Global.System.ComponentModel.INotifyPropertyChanged
        ''' <summary>
        ''' Create a new Person object.
        ''' </summary>
        ''' <param name="keys">Initial value of Keys.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreatePerson(ByVal keys As String) As Person
            Dim person As Person = New Person()
            person.Keys = keys
            Return person
        End Function
        ''' <summary>
        ''' There are no comments for Property Keys in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property Keys() As String
            Get
                Return Me._Keys
            End Get
            Set
                Me.OnKeysChanging(value)
                Me._Keys = value
                Me.OnKeysChanged
                Me.OnPropertyChanged("Keys")
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Keys As String
        Partial Private Sub OnKeysChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnKeysChanged()
        End Sub
        ''' <summary>
        ''' This event is raised when the value of the property is changed
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements Global.System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        ''' <summary>
        ''' The value of the property is changed
        ''' </summary>
        ''' <param name="property">property name</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
            End If
        End Sub
        ''' <summary>
        ''' There are no comments for TestFunction in the schema.
        ''' </summary>
        Public Overridable Function TestFunction(source As String) As Global.Microsoft.OData.Client.DataServiceQuery(Of Microsoft.OData.TestService.DSC.Person)
            Dim requestUri As Global.System.Uri = Nothing
            Context.TryGetUri(Me, requestUri)
            Return Me.Context.CreateFunctionQuery(Of Microsoft.OData.TestService.DSC.Person)("", String.Join("/", Global.System.Linq.Enumerable.Select(Global.System.Linq.Enumerable.Skip(requestUri.Segments, Me.Context.BaseUri.Segments.Length), Function(s) s.Trim("/"C))) + "/Microsoft.OData.TestService.DSC.TestFunction", False, New Global.Microsoft.OData.Client.UriOperationParameter("source", source))
        End Function
    End Class
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type Microsoft.OData.TestService.DSC.Person as Microsoft.OData.TestService.DSC.PersonSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of Microsoft.OData.TestService.DSC.Person), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As Microsoft.OData.TestService.DSC.PersonSingle
            Return New Microsoft.OData.TestService.DSC.PersonSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type Microsoft.OData.TestService.DSC.Person as Microsoft.OData.TestService.DSC.PersonSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="keys">The value of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of Microsoft.OData.TestService.DSC.Person),
            keys As String) As Microsoft.OData.TestService.DSC.PersonSingle
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "Keys", keys }
            }
            Return New Microsoft.OData.TestService.DSC.PersonSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' There are no comments for TestFunction in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function TestFunction(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of Microsoft.OData.TestService.DSC.Person), source As String) As Global.Microsoft.OData.Client.DataServiceQuery(Of Microsoft.OData.TestService.DSC.Person)
            If Not _source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If

            Return _source.CreateFunctionQuery(Of Microsoft.OData.TestService.DSC.Person)("Microsoft.OData.TestService.DSC.TestFunction", False, New Global.Microsoft.OData.Client.UriOperationParameter("source", source))
        End Function
    End Module
End Namespace
