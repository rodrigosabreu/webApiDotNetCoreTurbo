namespace WebApi.Dtos
{
    public class Transacao
    {
        public string nome { get; set; }
        public string tipo_lancamento { get; set; }
        public decimal valor { get; set; }
        public string sinal { get; set; }
    }
}
