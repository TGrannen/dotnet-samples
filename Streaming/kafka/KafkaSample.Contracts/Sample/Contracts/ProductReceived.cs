// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.11.3
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Sample.Contracts
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("avrogen", "1.11.3")]
	public partial class ProductReceived : global::Avro.Specific.ISpecificRecord
	{
		public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse(@"{""type"":""record"",""name"":""ProductReceived"",""namespace"":""Sample.Contracts"",""fields"":[{""name"":""PurchaseOrderNumber"",""type"":""string"",""displayName"":""Purchase Order Number"",""maxLength"":100},{""name"":""Sku"",""type"":""string"",""displayName"":""Sku"",""maxLength"":100},{""name"":""SerialNumber"",""type"":""string"",""displayName"":""SerialNumber"",""maxLength"":100}],""displayName"":""Product Received""}");
		private string _PurchaseOrderNumber;
		private string _Sku;
		private string _SerialNumber;
		public virtual global::Avro.Schema Schema
		{
			get
			{
				return ProductReceived._SCHEMA;
			}
		}
		public string PurchaseOrderNumber
		{
			get
			{
				return this._PurchaseOrderNumber;
			}
			set
			{
				this._PurchaseOrderNumber = value;
			}
		}
		public string Sku
		{
			get
			{
				return this._Sku;
			}
			set
			{
				this._Sku = value;
			}
		}
		public string SerialNumber
		{
			get
			{
				return this._SerialNumber;
			}
			set
			{
				this._SerialNumber = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.PurchaseOrderNumber;
			case 1: return this.Sku;
			case 2: return this.SerialNumber;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.PurchaseOrderNumber = (System.String)fieldValue; break;
			case 1: this.Sku = (System.String)fieldValue; break;
			case 2: this.SerialNumber = (System.String)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}