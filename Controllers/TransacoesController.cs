using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeuProjeto.Controllers
{
    public class Transacao
    {
        public decimal Valor { get; set; }
        public string Data { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
    }

    [Route("api")]
    [ApiController]
    public class MeuController : ControllerBase
    {
        [HttpGet]
        [Route("transacoes")]
        [AllowAnonymous]
        public async Task<IActionResult> get()
        {
            var transacoes = await getTransacoes();

            var transacoesValidadas = transacoes.Select(lancamento =>{   
                                                    RecategorizarTransacoes(lancamento);
                                                    RecategorizarTransacoes2(lancamento);
                                                    return lancamento;
                                                })
                                                .Where(lancamento => RemoverTransacoes(lancamento))
                                                .OrderByDescending(lancamento => lancamento.Data);

            return Ok(transacoesValidadas);
        }

        public bool RemoverTransacoes(Transacao lancamento)
        {
            if (lancamento.Categoria == ">>>> saldo_historico <<<<" && lancamento.Data == "2023-05-28")
                return false;

            return true;
        }

        public Transacao RecategorizarTransacoes(Transacao lancamento)
        {
            var _lancamento = lancamento;

            return _lancamento;
        }

        public Transacao RecategorizarTransacoes2(Transacao lancamento)
        {
            var _lancamento = lancamento;

            return _lancamento;
        }

        public async Task<List<Transacao>> getTransacoes()
        {
            List<Transacao> transacoes = new List<Transacao>();

            // Adicionar transações à lista
            transacoes.Add(new Transacao { Valor = 1100m, Data = "2023-05-28", Tipo = "saldo_historico", Categoria = ">>>> saldo_historico <<<<" });
            transacoes.Add(new Transacao { Valor = 100m, Data = "2023-05-28", Tipo = "Debito", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 200m, Data = "2023-05-28", Tipo = "Credito", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 300m, Data = "2023-05-28", Tipo = "Pix", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 400m, Data = "2023-05-28", Tipo = "Debito", Categoria = "lancamento" });

            transacoes.Add(new Transacao { Valor = 1100m, Data = "2023-05-27", Tipo = "saldo_historico", Categoria = ">>>> saldo_historico <<<<" });
            transacoes.Add(new Transacao { Valor = 100m, Data = "2023-05-27", Tipo = "Pix", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 200m, Data = "2023-05-27", Tipo = "Credito", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 300m, Data = "2023-05-27", Tipo = "Debito", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 400m, Data = "2023-05-27", Tipo = "Pix", Categoria = "lancamento" });

            transacoes.Add(new Transacao { Valor = 1100m, Data = "2023-05-26", Tipo = "saldo_historico", Categoria = ">>>> saldo_historico <<<<" });
            transacoes.Add(new Transacao { Valor = 51m, Data = "2023-05-26", Tipo = "Pix", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 52m, Data = "2023-05-26", Tipo = "Debito", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 53m, Data = "2023-05-26", Tipo = "Pix", Categoria = "lancamento" });
            transacoes.Add(new Transacao { Valor = 54m, Data = "2023-05-26", Tipo = "Debito", Categoria = "lancamento" });

            return transacoes;
        }
    }
}
