using RestSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using LuduStack.Infra.CrossCutting.Abstractions;
using LuduStack.Infra.CrossCutting.External.LuduStackGear;
using System.Text.Json;
using LuduStack.Domain.Models;
using LuduStack.Domain.Core.Enums;
using System.Globalization;

namespace LuduStack.Infra.CrossCutting.External
{
    public class LuduStackGearStore : IExternalStore
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private CultureInfo cultureInfo;
        private const string APIKEYPARAMETER = "BLING_APIKEY";
        private const string APIBASEURL = "https://bling.com.br/Api/v2";

        private string apiKey;
        RestClient client;

        public LuduStackGearStore(IConfiguration configuration, ILogger<LuduStackGearStore> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            cultureInfo = new CultureInfo("en-US");



            apiKey = Environment.GetEnvironmentVariable(APIKEYPARAMETER) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = configuration.GetSection(APIKEYPARAMETER).Value;
            }

            client = new RestClient(APIBASEURL);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            List<Product> resultList = new List<Product>();

            int errorCount = 0;
            int currentPage = 1;

            try
            {
                do
                {
                    RestRequest request = CreateRequest("/produtos/page={page}/json/", Method.Get, currentPage);
                    request.RequestFormat = DataFormat.Json;
                    RestResponse? response = await client.ExecuteAsync(request);

                    if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
                    {
                        BlingProductResponse blingResponse = JsonSerializer.Deserialize<BlingProductResponse>(response.Content) ?? new BlingProductResponse();

                        errorCount = blingResponse.Retorno.Erros.Count;

                        if (errorCount == 0)
                        {
                            List<BlingProdutoInterno>? externalStoreProducts = blingResponse.Retorno.Produtos.Select(x => x.ProdutoInterno).ToList();

                            foreach (BlingProdutoInterno product in externalStoreProducts)
                            {
                                Product newProduct = new Product
                                {
                                    Code = product.Codigo,
                                    ParentCode = product.CodigoPai,
                                    Name = product.Descricao,
                                    Price = float.Parse(product.Preco, cultureInfo),
                                    CreateDate = DateTime.ParseExact(product.DataInclusao, "yyyy-MM-dd", null),
                                    Origin = ProductOrigin.ExternalStore
                                };

                                if (product.Variacoes != null)
                                {
                                    foreach (var variant in product.Variacoes)
                                    {
                                        newProduct.Variants.Add(new ProductVariant
                                        {
                                            Code = variant.Variacao.Codigo,
                                            Name = variant.Variacao.Nome
                                        });
                                    }
                                }

                                resultList.Add(newProduct);
                            }
                        }
                    }

                    currentPage++;
                } while (errorCount == 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on synching products");
            }

            return resultList;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            List<Order> resultList = new List<Order>();

            int errorCount = 0;
            int currentPage = 1;

            try
            {
                do
                {
                    RestRequest request = CreateRequest("/pedidos/page={page}/json/", Method.Get, currentPage);
                    request.RequestFormat = DataFormat.Json;
                    RestResponse? response = await client.ExecuteAsync(request);

                    if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
                    {
                        BlingOrderResponse blingResponse = JsonSerializer.Deserialize<BlingOrderResponse>(response.Content) ?? new BlingOrderResponse();

                        errorCount = blingResponse.Retorno.Erros.Count;

                        if (errorCount == 0)
                        {
                            List<BlingPedidoInterno>? externalStoreOrders = blingResponse.Retorno.Pedidos.Select(x => x.PedidoInterno).ToList();

                            foreach (BlingPedidoInterno pedido in externalStoreOrders)
                            {
                                Order newOrder = new Order
                                {
                                    Number = pedido.Numero,
                                    StoreOrderNumber = pedido.NumeroPedidoLoja,
                                    TotalProductsValue = float.Parse(pedido.TotalProdutos, cultureInfo),
                                    FreightValue = float.Parse(pedido.ValorFrete, cultureInfo),
                                    TotalOrderValue = float.Parse(pedido.TotalVenda, cultureInfo),
                                    CreateDate = DateTime.ParseExact(pedido.Data, "yyyy-MM-dd", null),
                                    Origin = OrderOrigin.ExternalStore,
                                    Situation = FormatOrderSituation(pedido.Situacao)
                                };

                                if (pedido.Itens != null)
                                {
                                    foreach (var item in pedido.Itens)
                                    {
                                        newOrder.Items.Add(new OrderProduct
                                        {
                                            Code = item.Item.Codigo,
                                            Quantity = float.Parse(item.Item.Quantidade, cultureInfo),
                                            Description = item.Item.Descricao,
                                            UnitValue = float.Parse(item.Item.ValorUnidade, cultureInfo),
                                            ItemDiscount = float.Parse(item.Item.DescontoItem, cultureInfo),
                                        });
                                    }
                                }

                                resultList.Add(newOrder);
                            }
                        }
                    }

                    currentPage++;
                } while (errorCount == 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on synching products");
            }

            return resultList;
        }

        private RestRequest CreateRequest(string url, Method method, int currentPage)
        {
            return new RestRequest(url, method)
                .AddUrlSegment("page", currentPage.ToString())
                .AddParameter("apikey", apiKey, ParameterType.QueryString);
        }

        private OrderSituation FormatOrderSituation(string situacao)
        {
            switch (situacao)
            {
                case "Cancelado":
                    return OrderSituation.Canceled;
                case "Atendido":
                    return OrderSituation.Fulfilled;
                case "Em andamento":
                    return OrderSituation.Pending;
                default:
                    return OrderSituation.Unknown;
            }
        }
    }
}