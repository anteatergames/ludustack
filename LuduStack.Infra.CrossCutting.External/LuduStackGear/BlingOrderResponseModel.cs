using System.Text.Json.Serialization;

namespace LuduStack.Infra.CrossCutting.External.LuduStackGear
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class BlingCliente
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }

        [JsonPropertyName("ie")]
        public string Ie { get; set; }

        [JsonPropertyName("rg")]
        public string Rg { get; set; }

        [JsonPropertyName("endereco")]
        public string Endereco { get; set; }

        [JsonPropertyName("numero")]
        public string Numero { get; set; }

        [JsonPropertyName("complemento")]
        public string Complemento { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }

        [JsonPropertyName("cep")]
        public string Cep { get; set; }

        [JsonPropertyName("uf")]
        public string Uf { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("celular")]
        public string Celular { get; set; }

        [JsonPropertyName("fone")]
        public string Fone { get; set; }
    }

    public class BlingEnderecoEntrega
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("endereco")]
        public string Endereco { get; set; }

        [JsonPropertyName("numero")]
        public string Numero { get; set; }

        [JsonPropertyName("complemento")]
        public string Complemento { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }

        [JsonPropertyName("cep")]
        public string Cep { get; set; }

        [JsonPropertyName("uf")]
        public string Uf { get; set; }
    }

    public class BlingTransporte
    {
        [JsonPropertyName("enderecoEntrega")]
        public BlingEnderecoEntrega EnderecoEntrega { get; set; }
    }

    public class BlingItemInterno
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("quantidade")]
        public string Quantidade { get; set; }

        [JsonPropertyName("valorunidade")]
        public string ValorUnidade { get; set; }

        [JsonPropertyName("precocusto")]
        public string PrecoCusto { get; set; }

        [JsonPropertyName("descontoItem")]
        public string DescontoItem { get; set; }

        [JsonPropertyName("un")]
        public string Un { get; set; }

        [JsonPropertyName("pesoBruto")]
        public string PesoBruto { get; set; }

        [JsonPropertyName("largura")]
        public string Largura { get; set; }

        [JsonPropertyName("altura")]
        public string Altura { get; set; }

        [JsonPropertyName("profundidade")]
        public string Profundidade { get; set; }

        [JsonPropertyName("descricaoDetalhada")]
        public string DescricaoDetalhada { get; set; }

        [JsonPropertyName("unidadeMedida")]
        public string UnidadeMedida { get; set; }

        [JsonPropertyName("gtin")]
        public string Gtin { get; set; }
    }

    public class BlingItem
    {
        [JsonPropertyName("item")]
        public BlingItemInterno Item { get; set; }
    }

    public class BlingFormaPagamento
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("codigoFiscal")]
        public string CodigoFiscal { get; set; }
    }

    public class BlingParcelaInterno
    {
        [JsonPropertyName("idLancamento")]
        public string IdLancamento { get; set; }

        [JsonPropertyName("valor")]
        public string Valor { get; set; }

        [JsonPropertyName("dataVencimento")]
        public string DataVencimento { get; set; }

        [JsonPropertyName("obs")]
        public string Obs { get; set; }

        [JsonPropertyName("destino")]
        public string Destino { get; set; }

        [JsonPropertyName("forma_pagamento")]
        public BlingFormaPagamento FormaPagamento { get; set; }
    }

    public class BlingParcela
    {
        [JsonPropertyName("parcela")]
        public BlingParcela Parcela { get; set; }
    }

    public class BlingPagamento
    {
        [JsonPropertyName("categoria")]
        public string Categoria { get; set; }
    }

    public class BlingNota
    {
        [JsonPropertyName("serie")]
        public string Serie { get; set; }

        [JsonPropertyName("numero")]
        public string Numero { get; set; }

        [JsonPropertyName("dataEmissao")]
        public string DataEmissao { get; set; }

        [JsonPropertyName("situacao")]
        public string Situacao { get; set; }

        [JsonPropertyName("valorNota")]
        public string ValorNota { get; set; }

        [JsonPropertyName("chaveAcesso")]
        public string ChaveAcesso { get; set; }
    }

    public class BlingPedidoInterno
    {
        [JsonPropertyName("desconto")]
        public string Desconto { get; set; }

        [JsonPropertyName("observacoes")]
        public string Observacoes { get; set; }

        [JsonPropertyName("observacaointerna")]
        public string Observacaointerna { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("numero")]
        public string Numero { get; set; }

        [JsonPropertyName("numeroOrdemCompra")]
        public string NumeroOrdemCompra { get; set; }

        [JsonPropertyName("vendedor")]
        public string Vendedor { get; set; }

        [JsonPropertyName("valorfrete")]
        public string ValorFrete { get; set; }

        [JsonPropertyName("outrasdespesas")]
        public string Outrasdespesas { get; set; }

        [JsonPropertyName("totalprodutos")]
        public string TotalProdutos { get; set; }

        [JsonPropertyName("totalvenda")]
        public string TotalVenda { get; set; }

        [JsonPropertyName("situacao")]
        public string Situacao { get; set; }

        [JsonPropertyName("dataSaida")]
        public string DataSaida { get; set; }

        [JsonPropertyName("loja")]
        public string Loja { get; set; }

        [JsonPropertyName("numeroPedidoLoja")]
        public string NumeroPedidoLoja { get; set; }

        [JsonPropertyName("tipoIntegracao")]
        public string TipoIntegracao { get; set; }

        [JsonPropertyName("cliente")]
        public BlingCliente Cliente { get; set; }

        [JsonPropertyName("transporte")]
        public BlingTransporte Transporte { get; set; }

        [JsonPropertyName("itens")]
        public List<BlingItem> Itens { get; set; }

        [JsonPropertyName("parcelas")]
        public List<BlingParcela> Parcelas { get; set; }

        [JsonPropertyName("pagamento")]
        public BlingPagamento Pagamento { get; set; }

        [JsonPropertyName("nota")]
        public BlingNota Nota { get; set; }
    }

    public class BlingPedido
    {
        [JsonPropertyName("pedido")]
        public BlingPedidoInterno PedidoInterno { get; set; }
    }

    public class BlingOrderRetorno : BlingRetorno
    {
        [JsonPropertyName("pedidos")]
        public List<BlingPedido> Pedidos { get; set; }

        public BlingOrderRetorno()
        {
            Pedidos = new List<BlingPedido>();
        }
    }

    public class BlingOrderResponse : BlingApiResponse
    {
        [JsonPropertyName("retorno")]
        public BlingOrderRetorno Retorno { get; set; }

        public BlingOrderResponse()
        {
            Retorno = new BlingOrderRetorno();
        }
    }

}
