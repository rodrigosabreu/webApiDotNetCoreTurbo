using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Dtos;

namespace WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class NotificacaoController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;

        // uma coleção de conexões SSE
        private static ConcurrentDictionary<Guid, HttpResponse> _clients = new ConcurrentDictionary<Guid, HttpResponse>();
        private static ConcurrentDictionary<string, HttpResponse> _subscribers = new ConcurrentDictionary<string, HttpResponse>();

        public NotificacaoController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [HttpGet]
        [Route("notificacao")]
        [AllowAnonymous]
        public async Task GetNotificacao(CancellationToken cancellationToken)
        {          
            string[] nomes = { "João", "Maria", "Pedro", "Ana", "Luiza" };
            string[] lacamentos = { "PIX", "TED", "TEF", "PAGAMENTO"};
            decimal[] valores = { 10.5m, 20.75m, 30.0m, 40.15m, 50.50m };
            string[] sinal = { "POSITIVO", "NEGATIVO"};

            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "Keep-Alive");
            Response.Headers.Add("Content-Type", "text/event-stream; charset=utf-8");            
            Response.Headers.Add("Proxy-Connection", "Keep-Alive");
            Response.Headers.Add("Server", "mroth/sseserver");
            //Response.Headers.Add("Transfer-Encoding", "chunked");

            int indice = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Random random = new Random();
                int indiceNome = random.Next(nomes.Length);
                int indiceLacamento = random.Next(lacamentos.Length);
                int indiceValores = random.Next(valores.Length);
                int indiceSinal = random.Next(sinal.Length);
                indice++;
                Transacao transacao = new Transacao()
                {
                    nome = nomes[indiceNome],
                    tipo_lancamento = lacamentos[indiceLacamento],
                    valor = valores[indiceValores],
                    sinal = sinal[indiceSinal]
                };

                var responseJson = System.Text.Json.JsonSerializer.Serialize(transacao);

                /*byte[] data = Encoding.UTF8.GetBytes(responseJson);
                //await Response.Body.WriteAsync(data, 0, data.Length);
                await Response.WriteAsync(responseJson);
                await Response.Body.FlushAsync();

                await Task.Delay(5000);*/

                var json = JsonConvert.SerializeObject(transacao);

                var db = _redis.GetDatabase();
                await db.StringSetAsync($"cliente_{indice}", json, TimeSpan.FromSeconds(60));

                var message = $"data: {json}\n\n";

                //var message = $"data: Cliente: {transacao.nome} - Tipo: {transacao.tipo_lancamento} - {transacao.valor} - {transacao.sinal}\n\n";
                await Response.WriteAsync(message);
                await Response.Body.FlushAsync();
                await Task.Delay(1000);

            }            
        }

        [HttpGet("subscribe")]
        [AllowAnonymous]
        public IActionResult Subscribe()
        {
            var response = HttpContext.Response;
            response.Headers.Add("Content-Type", "text/event-stream");
            response.Headers.Add("Cache-Control", "no-cache");
            response.Headers.Add("Connection", "keep-alive");

            var subscriberId = Guid.NewGuid().ToString();

            _subscribers.TryAdd(subscriberId, response);

            return new JsonResult(new { SubscriberId = subscriberId });
        }

        [HttpGet("notify/{subscriberId}")]
        [AllowAnonymous]
        public async Task Notify(string subscriberId, CancellationToken cancellationToken)
        {
            if (_subscribers.TryGetValue(subscriberId, out var response))
            {
                Response.Headers.Add("Content-Type", "text/event-stream");
                Response.Headers.Add("Cache-Control", "no-cache");
                Response.Headers.Add("Connection", "keep-alive");

                string[] nomes = { "João", "Maria", "Pedro", "Ana", "Luiza" };
                string[] lacamentos = { "PIX", "TED", "TEF", "PAGAMENTO" };
                decimal[] valores = { 10.5m, 20.75m, 30.0m, 40.15m, 50.50m };
                string[] sinal = { "POSITIVO", "NEGATIVO" };

                while (!cancellationToken.IsCancellationRequested)
                {

                    Random random = new Random();
                    int indiceNome = random.Next(nomes.Length);
                    int indiceLacamento = random.Next(lacamentos.Length);
                    int indiceValores = random.Next(valores.Length);
                    int indiceSinal = random.Next(sinal.Length);

                    Transacao transacao = new Transacao()
                    {
                        nome = nomes[indiceNome],
                        tipo_lancamento = lacamentos[indiceLacamento],
                        valor = valores[indiceValores],
                        sinal = sinal[indiceSinal]
                    };

                    var json = JsonConvert.SerializeObject(transacao);
                    var message = $"data: {json}\n\n";
                    //var message = "data: " + json + "\n\n";
                    await Response.WriteAsync(message);
                    await Response.Body.FlushAsync();
                    await Task.Delay(1000);
                }
                //return Ok();
            }
            else
            {
                //return NotFound();
            }
        }


    } 
}
