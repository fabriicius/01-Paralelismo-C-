using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;
using ByteBank.Test.Utils;
using System.Threading;

namespace ByteBank.Test.Thread_8
{
    public class MainWindow
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;
        public List<string> _listaProgresso;
        
        public MainWindow()
        {
            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
            _listaProgresso = new();
        }

        public async Task<IEnumerable<string>> BtnProcessar_Click()
        {
            return await BtnProcessar_Click(new CancellationTokenSource()); 
        }


        public async Task<IEnumerable<string>> BtnProcessar_Click(CancellationTokenSource _cancellationTokenSource)
        {
            var contas = r_Repositorio.GetContaClientes();

            var inicio = DateTime.Now;

            var taskProgresso = new Progress<string>((resultado) =>
            {
                _listaProgresso.Add(resultado);
            });


            var resultado = await ConsolidarContas(contas, taskProgresso, _cancellationTokenSource.Token);
            var fim = DateTime.Now;

            return AtualizarView(resultado, fim - inicio);

        }


        private async Task<string[]> ConsolidarContas(IEnumerable<ContaCliente> contas
            , IProgress<string> reportadorProgresso
            , CancellationToken ct)
        {
            try
            {
                var contasTask = contas.Select(c =>
                 Task.Factory.StartNew(() =>
                 {
                     //ct.ThrowIfCancellationRequested();
                     if (ct.IsCancellationRequested)
                         throw new OperationCanceledException(ct);

                     var resultado = r_Servico.ConsolidarMovimentacao(c, ct);
                     reportadorProgresso.Report(resultado);

                     //ct.ThrowIfCancellationRequested();
                     if (ct.IsCancellationRequested)
                         throw new OperationCanceledException(ct);

                     return resultado;
                 },ct));

                return await Task.WhenAll(contasTask);
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException(ct);
            }
            finally
            {
                _listaProgresso = new();
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
