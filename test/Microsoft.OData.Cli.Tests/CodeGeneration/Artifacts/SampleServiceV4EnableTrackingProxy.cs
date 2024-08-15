﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:8.0.4
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generation date: 07-May-24 3:53:11 PM
namespace SampleServiceV4.Models
{
    /// <summary>
    /// There are no comments for CustomerSingle in the schema.
    /// </summary>
    public partial class CustomerSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<Customer>
    {
        /// <summary>
        /// Initialize a new CustomerSingle object.
        /// </summary>
        public CustomerSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new CustomerSingle object.
        /// </summary>
        public CustomerSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new CustomerSingle object.
        /// </summary>
        public CustomerSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<Customer> query)
            : base(query) { }

        /// <summary>
        /// There are no comments for Orders in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Order> Orders
        {
            get
            {
                if (!this.IsComposable)
                {
                    throw new global::System.NotSupportedException("The previous function is not composable.");
                }
                if ((this._Orders == null))
                {
                    this._Orders = Context.CreateQuery<global::SampleServiceV4.Models.Order>(GetPath("Orders"));
                }
                return this._Orders;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Order> _Orders;
    }
    /// <summary>
    /// There are no comments for Customer in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Id
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("Id")]
    public partial class Customer : global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new Customer object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        /// <param name="name">Initial value of Name.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static Customer CreateCustomer(int ID, string name)
        {
            Customer customer = new Customer();
            customer.Id = ID;
            customer.Name = name;
            return customer;
        }
        /// <summary>
        /// There are no comments for Property Id in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Id is required.")]
        public virtual int Id
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
                this.OnPropertyChanged("Id");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private int _Id;
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        /// <summary>
        /// There are no comments for Property Name in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Name is required.")]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this.OnNameChanging(value);
                this._Name = value;
                this.OnNameChanged();
                this.OnPropertyChanged("Name");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _Name;
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        /// <summary>
        /// There are no comments for Property Orders in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual global::Microsoft.OData.Client.DataServiceCollection<global::SampleServiceV4.Models.Order> Orders
        {
            get
            {
                return this._Orders;
            }
            set
            {
                this.OnOrdersChanging(value);
                this._Orders = value;
                this.OnOrdersChanged();
                this.OnPropertyChanged("Orders");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.DataServiceCollection<global::SampleServiceV4.Models.Order> _Orders = new global::Microsoft.OData.Client.DataServiceCollection<global::SampleServiceV4.Models.Order>(null, global::Microsoft.OData.Client.TrackingMode.None);
        partial void OnOrdersChanging(global::Microsoft.OData.Client.DataServiceCollection<global::SampleServiceV4.Models.Order> value);
        partial void OnOrdersChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for OrderSingle in the schema.
    /// </summary>
    public partial class OrderSingle : global::Microsoft.OData.Client.DataServiceQuerySingle<Order>
    {
        /// <summary>
        /// Initialize a new OrderSingle object.
        /// </summary>
        public OrderSingle(global::Microsoft.OData.Client.DataServiceContext context, string path)
            : base(context, path) { }

        /// <summary>
        /// Initialize a new OrderSingle object.
        /// </summary>
        public OrderSingle(global::Microsoft.OData.Client.DataServiceContext context, string path, bool isComposable)
            : base(context, path, isComposable) { }

        /// <summary>
        /// Initialize a new OrderSingle object.
        /// </summary>
        public OrderSingle(global::Microsoft.OData.Client.DataServiceQuerySingle<Order> query)
            : base(query) { }

        /// <summary>
        /// There are no comments for Customer in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual global::SampleServiceV4.Models.CustomerSingle Customer
        {
            get
            {
                if (!this.IsComposable)
                {
                    throw new global::System.NotSupportedException("The previous function is not composable.");
                }
                if ((this._Customer == null))
                {
                    this._Customer = new global::SampleServiceV4.Models.CustomerSingle(this.Context, GetPath("Customer"));
                }
                return this._Customer;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::SampleServiceV4.Models.CustomerSingle _Customer;
    }
    /// <summary>
    /// There are no comments for Order in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Id
    /// </KeyProperties>
    [global::Microsoft.OData.Client.Key("Id")]
    public partial class Order : global::Microsoft.OData.Client.BaseEntityType, global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new Order object.
        /// </summary>
        /// <param name="ID">Initial value of Id.</param>
        /// <param name="amount">Initial value of Amount.</param>
        /// <param name="customer">Initial value of Customer.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static Order CreateOrder(int ID, decimal amount, global::SampleServiceV4.Models.Customer customer)
        {
            Order order = new Order();
            order.Id = ID;
            order.Amount = amount;
            if ((customer == null))
            {
                throw new global::System.ArgumentNullException("customer");
            }
            order.Customer = customer;
            return order;
        }
        /// <summary>
        /// There are no comments for Property Id in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Id is required.")]
        public virtual int Id
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
                this.OnPropertyChanged("Id");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private int _Id;
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        /// <summary>
        /// There are no comments for Property Amount in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Amount is required.")]
        public virtual decimal Amount
        {
            get
            {
                return this._Amount;
            }
            set
            {
                this.OnAmountChanging(value);
                this._Amount = value;
                this.OnAmountChanged();
                this.OnPropertyChanged("Amount");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private decimal _Amount;
        partial void OnAmountChanging(decimal value);
        partial void OnAmountChanged();
        /// <summary>
        /// There are no comments for Property Customer in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Customer is required.")]
        public virtual global::SampleServiceV4.Models.Customer Customer
        {
            get
            {
                return this._Customer;
            }
            set
            {
                this.OnCustomerChanging(value);
                this._Customer = value;
                this.OnCustomerChanged();
                this.OnPropertyChanged("Customer");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::SampleServiceV4.Models.Customer _Customer;
        partial void OnCustomerChanging(global::SampleServiceV4.Models.Customer value);
        partial void OnCustomerChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for Address in the schema.
    /// </summary>
    public partial class Address : global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new Address object.
        /// </summary>
        /// <param name="street">Initial value of Street.</param>
        /// <param name="city">Initial value of City.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static Address CreateAddress(string street, global::SampleServiceV4.Models.City city)
        {
            Address address = new Address();
            address.Street = street;
            if ((city == null))
            {
                throw new global::System.ArgumentNullException("city");
            }
            address.City = city;
            return address;
        }
        /// <summary>
        /// There are no comments for Property Street in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Street is required.")]
        public virtual string Street
        {
            get
            {
                return this._Street;
            }
            set
            {
                this.OnStreetChanging(value);
                this._Street = value;
                this.OnStreetChanged();
                this.OnPropertyChanged("Street");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _Street;
        partial void OnStreetChanging(string value);
        partial void OnStreetChanged();
        /// <summary>
        /// There are no comments for Property City in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "City is required.")]
        public virtual global::SampleServiceV4.Models.City City
        {
            get
            {
                return this._City;
            }
            set
            {
                this.OnCityChanging(value);
                this._City = value;
                this.OnCityChanged();
                this.OnPropertyChanged("City");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::SampleServiceV4.Models.City _City;
        partial void OnCityChanging(global::SampleServiceV4.Models.City value);
        partial void OnCityChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// There are no comments for City in the schema.
    /// </summary>
    public partial class City : global::System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Create a new City object.
        /// </summary>
        /// <param name="name">Initial value of Name.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public static City CreateCity(string name)
        {
            City city = new City();
            city.Name = name;
            return city;
        }
        /// <summary>
        /// There are no comments for Property Name in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        [global::System.ComponentModel.DataAnnotations.RequiredAttribute(ErrorMessage = "Name is required.")]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this.OnNameChanging(value);
                this._Name = value;
                this.OnNameChanged();
                this.OnPropertyChanged("Name");
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private string _Name;
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        /// <summary>
        /// This event is raised when the value of the property is changed
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// The value of the property is changed
        /// </summary>
        /// <param name="property">property name</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        protected virtual void OnPropertyChanged(string property)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get an entity of type global::SampleServiceV4.Models.Customer as global::SampleServiceV4.Models.CustomerSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::SampleServiceV4.Models.CustomerSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Customer> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::SampleServiceV4.Models.CustomerSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::SampleServiceV4.Models.Customer as global::SampleServiceV4.Models.CustomerSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::SampleServiceV4.Models.CustomerSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Customer> _source,
            int id)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::SampleServiceV4.Models.CustomerSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::SampleServiceV4.Models.Order as global::SampleServiceV4.Models.OrderSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="_keys">dictionary with the names and values of keys</param>
        public static global::SampleServiceV4.Models.OrderSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Order> _source, global::System.Collections.Generic.IDictionary<string, object> _keys)
        {
            return new global::SampleServiceV4.Models.OrderSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
        /// <summary>
        /// Get an entity of type global::SampleServiceV4.Models.Order as global::SampleServiceV4.Models.OrderSingle specified by key from an entity set
        /// </summary>
        /// <param name="_source">source entity set</param>
        /// <param name="id">The value of id</param>
        public static global::SampleServiceV4.Models.OrderSingle ByKey(this global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Order> _source,
            int id)
        {
            global::System.Collections.Generic.IDictionary<string, object> _keys = new global::System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", id }
            };
            return new global::SampleServiceV4.Models.OrderSingle(_source.Context, _source.GetKeyPath(global::Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)));
        }
    }
}
namespace SampleServiceV4.Default
{
    /// <summary>
    /// There are no comments for Container in the schema.
    /// </summary>
    public partial class Container : global::Microsoft.OData.Client.DataServiceContext
    {
        /// <summary>
        /// Initialize a new Container object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public Container(global::System.Uri serviceRoot) :
                this(serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion.V4)
        {
        }

        /// <summary>
        /// Initialize a new Container object.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public Container(global::System.Uri serviceRoot, global::Microsoft.OData.Client.ODataProtocolVersion protocolVersion) :
                base(serviceRoot, protocolVersion)
        {
            this.OnContextCreated();
            this.Format.LoadServiceModel = GeneratedEdmModel.GetInstance;
            this.Format.UseJson();
        }
        partial void OnContextCreated();
        /// <summary>
        /// There are no comments for Customers in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Customer> Customers
        {
            get
            {
                if ((this._Customers == null))
                {
                    this._Customers = base.CreateQuery<global::SampleServiceV4.Models.Customer>("Customers");
                }
                return this._Customers;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Customer> _Customers;
        /// <summary>
        /// There are no comments for Orders in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Order> Orders
        {
            get
            {
                if ((this._Orders == null))
                {
                    this._Orders = base.CreateQuery<global::SampleServiceV4.Models.Order>("Orders");
                }
                return this._Orders;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Order> _Orders;
        /// <summary>
        /// There are no comments for Customers in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual void AddToCustomers(global::SampleServiceV4.Models.Customer customer)
        {
            base.AddObject("Customers", customer);
        }
        /// <summary>
        /// There are no comments for Orders in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        public virtual void AddToOrders(global::SampleServiceV4.Models.Order order)
        {
            base.AddObject("Orders", order);
        }
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
        private abstract class GeneratedEdmModel
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::Microsoft.OData.Edm.IEdmModel ParsedModel = LoadModelFromString();

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private const string filePath = @"OData ServiceCsdl.xml";

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            public static global::Microsoft.OData.Edm.IEdmModel GetInstance()
            {
                return ParsedModel;
            }
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::Microsoft.OData.Edm.IEdmModel LoadModelFromString()
            {
                global::System.Xml.XmlReader reader = CreateXmlReader();
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
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::System.Xml.XmlReader CreateXmlReader(string edmxToParse)
            {
                return global::System.Xml.XmlReader.Create(new global::System.IO.StringReader(edmxToParse));
            }

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")]
            private static global::System.Xml.XmlReader CreateXmlReader()
            {
                try
                {
                    var assembly = global::System.Reflection.Assembly.GetExecutingAssembly();
                    // If multiple resource names end with the file name, select the shortest one.
                    var resourcePath = global::System.Linq.Enumerable.First(
                        global::System.Linq.Enumerable.OrderBy(assembly.GetManifestResourceNames(), n => n.Length),
                        str => str.EndsWith(filePath));
                    global::System.IO.Stream stream = assembly.GetManifestResourceStream(resourcePath);
                    return global::System.Xml.XmlReader.Create(new global::System.IO.StreamReader(stream));
                }
                catch (global::System.Xml.XmlException e)
                {
                    throw new global::System.Xml.XmlException("Failed to create an XmlReader from the stream. Check if the resource exists.", e);
                }
            }
        }
        /// <summary>
        /// There are no comments for BackupDataSource in the schema.
        /// </summary>
        public virtual global::Microsoft.OData.Client.DataServiceActionQuery BackupDataSource()
        {
            return new global::Microsoft.OData.Client.DataServiceActionQuery(this, this.BaseUri.OriginalString.Trim('/') + "/BackupDataSource");
        }
        /// <summary>
        /// There are no comments for ResetDataSource in the schema.
        /// </summary>
        public virtual global::Microsoft.OData.Client.DataServiceActionQuery ResetDataSource()
        {
            return new global::Microsoft.OData.Client.DataServiceActionQuery(this, this.BaseUri.OriginalString.Trim('/') + "/ResetDataSource");
        }
        /// <summary>
        /// There are no comments for RepairDataSource in the schema.
        /// </summary>
        public virtual global::Microsoft.OData.Client.DataServiceActionQuery RepairDataSource()
        {
            return new global::Microsoft.OData.Client.DataServiceActionQuery(this, this.BaseUri.OriginalString.Trim('/') + "/RepairDataSource");
        }
    }
    /// <summary>
    /// Class containing all extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// There are no comments for GetTopCustomers in the schema.
        /// </summary>
        public static global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Customer> GetTopCustomers(this global::Microsoft.OData.Client.DataServiceQuery<global::SampleServiceV4.Models.Customer> _source)
        {
            if (!_source.IsComposable)
            {
                throw new global::System.NotSupportedException("The previous function is not composable.");
            }

            return _source.CreateFunctionQuery<global::SampleServiceV4.Models.Customer>("SampleServiceV4.Default.GetTopCustomers", false);
        }
        /// <summary>
        /// There are no comments for RateCustomer in the schema.
        /// </summary>
        public static global::Microsoft.OData.Client.DataServiceActionQuery RateCustomer(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::SampleServiceV4.Models.Customer> _source, int rating)
        {
            if (!_source.IsComposable)
            {
                throw new global::System.NotSupportedException("The previous function is not composable.");
            }

            return new global::Microsoft.OData.Client.DataServiceActionQuery(_source.Context, _source.AppendRequestUri("SampleServiceV4.Default.RateCustomer"), new global::Microsoft.OData.Client.BodyOperationParameter("rating", rating));
        }
        /// <summary>
        /// There are no comments for ProcessOrder in the schema.
        /// </summary>
        public static global::Microsoft.OData.Client.DataServiceActionQuery ProcessOrder(this global::Microsoft.OData.Client.DataServiceQuerySingle<global::SampleServiceV4.Models.Order> _source)
        {
            if (!_source.IsComposable)
            {
                throw new global::System.NotSupportedException("The previous function is not composable.");
            }

            return new global::Microsoft.OData.Client.DataServiceActionQuery(_source.Context, _source.AppendRequestUri("SampleServiceV4.Default.ProcessOrder"));
        }
    }
}
