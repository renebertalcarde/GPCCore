﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommonAPI
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="commonapi")]
	public partial class dalDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void Insert_brgy(_brgy instance);
    partial void Update_brgy(_brgy instance);
    partial void Delete_brgy(_brgy instance);
    partial void Insert_region(_region instance);
    partial void Update_region(_region instance);
    partial void Delete_region(_region instance);
    partial void Insert_country(_country instance);
    partial void Update_country(_country instance);
    partial void Delete_country(_country instance);
    partial void Insert_mun_city(_mun_city instance);
    partial void Update_mun_city(_mun_city instance);
    partial void Delete_mun_city(_mun_city instance);
    partial void Insert_province(_province instance);
    partial void Update_province(_province instance);
    partial void Delete_province(_province instance);
    #endregion
		
		public dalDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["commonapiConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public dalDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public dalDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public dalDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public dalDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<_brgy> _brgies
		{
			get
			{
				return this.GetTable<_brgy>();
			}
		}
		
		public System.Data.Linq.Table<_region> _regions
		{
			get
			{
				return this.GetTable<_region>();
			}
		}
		
		public System.Data.Linq.Table<_country> _countries
		{
			get
			{
				return this.GetTable<_country>();
			}
		}
		
		public System.Data.Linq.Table<_mun_city> _mun_cities
		{
			get
			{
				return this.GetTable<_mun_city>();
			}
		}
		
		public System.Data.Linq.Table<_province> _provinces
		{
			get
			{
				return this.GetTable<_province>();
			}
		}
		
		public System.Data.Linq.Table<vwCity> vwCities
		{
			get
			{
				return this.GetTable<vwCity>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[_brgy]")]
	public partial class _brgy : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _CityId;
		
		private string _Brgy;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnCityIdChanging(int value);
    partial void OnCityIdChanged();
    partial void OnBrgyChanging(string value);
    partial void OnBrgyChanged();
    #endregion
		
		public _brgy()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CityId", DbType="Int NOT NULL")]
		public int CityId
		{
			get
			{
				return this._CityId;
			}
			set
			{
				if ((this._CityId != value))
				{
					this.OnCityIdChanging(value);
					this.SendPropertyChanging();
					this._CityId = value;
					this.SendPropertyChanged("CityId");
					this.OnCityIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Brgy", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Brgy
		{
			get
			{
				return this._Brgy;
			}
			set
			{
				if ((this._Brgy != value))
				{
					this.OnBrgyChanging(value);
					this.SendPropertyChanging();
					this._Brgy = value;
					this.SendPropertyChanged("Brgy");
					this.OnBrgyChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[_regions]")]
	public partial class _region : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _CountryId;
		
		private string _Region;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnCountryIdChanging(int value);
    partial void OnCountryIdChanged();
    partial void OnRegionChanging(string value);
    partial void OnRegionChanged();
    #endregion
		
		public _region()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CountryId", DbType="Int NOT NULL")]
		public int CountryId
		{
			get
			{
				return this._CountryId;
			}
			set
			{
				if ((this._CountryId != value))
				{
					this.OnCountryIdChanging(value);
					this.SendPropertyChanging();
					this._CountryId = value;
					this.SendPropertyChanged("CountryId");
					this.OnCountryIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Region", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				if ((this._Region != value))
				{
					this.OnRegionChanging(value);
					this.SendPropertyChanging();
					this._Region = value;
					this.SendPropertyChanged("Region");
					this.OnRegionChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[_countries]")]
	public partial class _country : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Country;
		
		private string _Nationality;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnCountryChanging(string value);
    partial void OnCountryChanged();
    partial void OnNationalityChanging(string value);
    partial void OnNationalityChanged();
    #endregion
		
		public _country()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Country", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Country
		{
			get
			{
				return this._Country;
			}
			set
			{
				if ((this._Country != value))
				{
					this.OnCountryChanging(value);
					this.SendPropertyChanging();
					this._Country = value;
					this.SendPropertyChanged("Country");
					this.OnCountryChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Nationality", DbType="NVarChar(50)")]
		public string Nationality
		{
			get
			{
				return this._Nationality;
			}
			set
			{
				if ((this._Nationality != value))
				{
					this.OnNationalityChanging(value);
					this.SendPropertyChanging();
					this._Nationality = value;
					this.SendPropertyChanged("Nationality");
					this.OnNationalityChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[_mun_cities]")]
	public partial class _mun_city : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _ProvinceId;
		
		private string _City;
		
		private bool _IsCity;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnProvinceIdChanging(int value);
    partial void OnProvinceIdChanged();
    partial void OnCityChanging(string value);
    partial void OnCityChanged();
    partial void OnIsCityChanging(bool value);
    partial void OnIsCityChanged();
    #endregion
		
		public _mun_city()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProvinceId", DbType="Int NOT NULL")]
		public int ProvinceId
		{
			get
			{
				return this._ProvinceId;
			}
			set
			{
				if ((this._ProvinceId != value))
				{
					this.OnProvinceIdChanging(value);
					this.SendPropertyChanging();
					this._ProvinceId = value;
					this.SendPropertyChanged("ProvinceId");
					this.OnProvinceIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_City", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string City
		{
			get
			{
				return this._City;
			}
			set
			{
				if ((this._City != value))
				{
					this.OnCityChanging(value);
					this.SendPropertyChanging();
					this._City = value;
					this.SendPropertyChanged("City");
					this.OnCityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsCity", DbType="Bit NOT NULL")]
		public bool IsCity
		{
			get
			{
				return this._IsCity;
			}
			set
			{
				if ((this._IsCity != value))
				{
					this.OnIsCityChanging(value);
					this.SendPropertyChanging();
					this._IsCity = value;
					this.SendPropertyChanged("IsCity");
					this.OnIsCityChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.[_provinces]")]
	public partial class _province : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private int _RegionId;
		
		private string _Province;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnRegionIdChanging(int value);
    partial void OnRegionIdChanged();
    partial void OnProvinceChanging(string value);
    partial void OnProvinceChanged();
    #endregion
		
		public _province()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RegionId", DbType="Int NOT NULL")]
		public int RegionId
		{
			get
			{
				return this._RegionId;
			}
			set
			{
				if ((this._RegionId != value))
				{
					this.OnRegionIdChanging(value);
					this.SendPropertyChanging();
					this._RegionId = value;
					this.SendPropertyChanged("RegionId");
					this.OnRegionIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Province", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Province
		{
			get
			{
				return this._Province;
			}
			set
			{
				if ((this._Province != value))
				{
					this.OnProvinceChanging(value);
					this.SendPropertyChanging();
					this._Province = value;
					this.SendPropertyChanged("Province");
					this.OnProvinceChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.vwCities")]
	public partial class vwCity
	{
		
		private int _Id;
		
		private string _City;
		
		private bool _IsCity;
		
		private int _ProvinceId;
		
		private string _Province;
		
		private int _RegionId;
		
		private string _Region;
		
		private int _CountryId;
		
		private string _Country;
		
		private string _Nationality;
		
		public vwCity()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL")]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this._Id = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_City", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string City
		{
			get
			{
				return this._City;
			}
			set
			{
				if ((this._City != value))
				{
					this._City = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsCity", DbType="Bit NOT NULL")]
		public bool IsCity
		{
			get
			{
				return this._IsCity;
			}
			set
			{
				if ((this._IsCity != value))
				{
					this._IsCity = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProvinceId", DbType="Int NOT NULL")]
		public int ProvinceId
		{
			get
			{
				return this._ProvinceId;
			}
			set
			{
				if ((this._ProvinceId != value))
				{
					this._ProvinceId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Province", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Province
		{
			get
			{
				return this._Province;
			}
			set
			{
				if ((this._Province != value))
				{
					this._Province = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RegionId", DbType="Int NOT NULL")]
		public int RegionId
		{
			get
			{
				return this._RegionId;
			}
			set
			{
				if ((this._RegionId != value))
				{
					this._RegionId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Region", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				if ((this._Region != value))
				{
					this._Region = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CountryId", DbType="Int NOT NULL")]
		public int CountryId
		{
			get
			{
				return this._CountryId;
			}
			set
			{
				if ((this._CountryId != value))
				{
					this._CountryId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Country", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Country
		{
			get
			{
				return this._Country;
			}
			set
			{
				if ((this._Country != value))
				{
					this._Country = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Nationality", DbType="NVarChar(50)")]
		public string Nationality
		{
			get
			{
				return this._Nationality;
			}
			set
			{
				if ((this._Nationality != value))
				{
					this._Nationality = value;
				}
			}
		}
	}
}
#pragma warning restore 1591