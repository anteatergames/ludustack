using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.External.LuduStackGear
{
    public class BlingErro
    {
        [JsonPropertyName("cod")]
        public string Codigo { get; set; }

        [JsonPropertyName("msg")]
        public string Mensagem { get; set; }
    }

    public class BlingRetorno
    {

        [JsonPropertyName("erros")]
        public List<BlingErro> Erros { get; set; }

        public BlingRetorno()
        {
            Erros = new List<BlingErro>();
        }
    }

    public class BlingApiResponse
    {
    }
}
