// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.12.0+8c27801dc8d42ccc00997f25c0b8f45f8d4a233e
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace _4oito6.informacoes_cliente_cadastrado
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("avrogen", "1.12.0+8c27801dc8d42ccc00997f25c0b8f45f8d4a233e")]
	public partial class Endereco : global::Avro.Specific.ISpecificRecord
	{
		public static global::Avro.Schema _SCHEMA = global::Avro.Schema.Parse(@"{""type"":""record"",""name"":""Endereco"",""namespace"":""_4oito6.informacoes_cliente_cadastrado"",""fields"":[{""name"":""logradouro"",""type"":""string""},{""name"":""numero"",""type"":[""string"",""null""]},{""name"":""bairro"",""type"":""string""},{""name"":""cidade"",""type"":""string""},{""name"":""estado"",""type"":""string""},{""name"":""cep"",""type"":""string""}]}");
		private string _logradouro;
		private string _numero;
		private string _bairro;
		private string _cidade;
		private string _estado;
		private string _cep;
		public virtual global::Avro.Schema Schema
		{
			get
			{
				return Endereco._SCHEMA;
			}
		}
		public string logradouro
		{
			get
			{
				return this._logradouro;
			}
			set
			{
				this._logradouro = value;
			}
		}
		public string numero
		{
			get
			{
				return this._numero;
			}
			set
			{
				this._numero = value;
			}
		}
		public string bairro
		{
			get
			{
				return this._bairro;
			}
			set
			{
				this._bairro = value;
			}
		}
		public string cidade
		{
			get
			{
				return this._cidade;
			}
			set
			{
				this._cidade = value;
			}
		}
		public string estado
		{
			get
			{
				return this._estado;
			}
			set
			{
				this._estado = value;
			}
		}
		public string cep
		{
			get
			{
				return this._cep;
			}
			set
			{
				this._cep = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.logradouro;
			case 1: return this.numero;
			case 2: return this.bairro;
			case 3: return this.cidade;
			case 4: return this.estado;
			case 5: return this.cep;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.logradouro = (System.String)fieldValue; break;
			case 1: this.numero = (System.String)fieldValue; break;
			case 2: this.bairro = (System.String)fieldValue; break;
			case 3: this.cidade = (System.String)fieldValue; break;
			case 4: this.estado = (System.String)fieldValue; break;
			case 5: this.cep = (System.String)fieldValue; break;
			default: throw new global::Avro.AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
