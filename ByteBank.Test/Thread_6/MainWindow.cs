using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;

namespace ByteBank.Test.Thread_6
{
    public class MainWindow
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;
        private bool botaoProcessar = true;

        public MainWindow()
        {
            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
        }


        public async Task<IEnumerable<string>> BtnProcessar_Click()
        {
            botaoProcessar = false;

            var contas = r_Repositorio.GetContaClientes();

            var inicio = DateTime.Now;
            var resultado = await ConsolidarContas(contas);
            var fim = DateTime.Now;

            return AtualizarView(resultado, fim - inicio);
            
        }

        private async Task<string[]> ConsolidarContas(IEnumerable<ContaCliente> contas)
        {
            try
            {
                var contasTask = contas.Select(c =>
                 Task.Factory.StartNew(() => r_Servico.ConsolidarMovimentacao(c)));

                return await Task.WhenAll(contasTask);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private IEnumerable<string> AtualizarView(IEnumerable<string> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{elapsedTime.Seconds}.{elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count()} clientes em {tempoDecorrido}";

            Console.WriteLine("--LstResultados.ItemsSource--");
            Console.WriteLine("--TxtTempo.Text--");
            Console.WriteLine(mensagem);

            return result;
            
        }
    }
}
