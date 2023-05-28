using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

public class IncluirReservaCommand : IRequest<ReservaResponseViewModel>
{
    public string CpfCliente { get; set; }
    public int QuantidadePessoas { get; set; }
    public DateTime DataReserva { get; set; }
}

public class IncluirReservaCommandHandler : IRequestHandler<IncluirReservaCommand, ReservaResponseViewModel>
{
    private IClienteApiClient ClienteCli { get; }
    private IReservaRepository Repo { get; }

    public IncluirReservaCommandHandler(IClienteApiClient cli, IReservaRepository repo)
    {
        ClienteCli = cli;
        Repo = repo;
    }

    public async Task<ReservaResponseViewModel> Handle(IncluirReservaCommand request, CancellationToken cancellationToken)
    {
        var dadosCli = await ClienteCli.ObterCliente(request.CpfCliente);

        if(dadosCli is null)
        {
            return new ReservaResponseViewModel(false, "");
        }
        else
        {
            var novaReserva = Reserva.CriarNova();

            var sucesso = await Repo.Incluir(novaReserva);

            return new ReservaResponseViewModel(sucesso, novaReserva);

        }        
    }
}









public interface IClienteApiClient
{
    Task<Cliente> ObterCliente(string cpfCliente);
}

public class ClienteApiClient : IClienteApiClient
{
    public Task<Cliente> ObterCliente(string cpfCliente)
    {
        return Task.FromResult(new Cliente());
    }
}

public class Cliente
{
}


public interface IReservaRepository
{
    Task<bool> Incluir(string novaReserva);
}





public class ReservaRepository : IReservaRepository
{    

    public Task<bool> Incluir(string novaReserva)
    {
        return Task.FromResult(true);
    }
}

public static class Reserva
{
    public static string CriarNova()
    {
        return "Reserva Criada";
    }
}

public class ReservaResponseViewModel
{
    public ReservaResponseViewModel(Boolean flag, string texto)
    {
            
    }
}
