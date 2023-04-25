using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;
using ByteBank.Test.Utils;

namespace ByteBank.Test.Thread_7
{
    public class MainWindow 
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;
        public List<string> listaProgresso;

        public MainWindow()
        {
            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
            listaProgresso = new();
        }


        public async Task<IEnumerable<string>> BtnProcessar_Click()
        {
            var contas = r_Repositorio.GetContaClientes();

            var inicio = DateTime.Now;

            var taskProgresso = new Progress<string>((resultado) =>
            {
                listaProgresso.Add(resultado);
            }); 

            //var taskProgresso = new Progresso<string>((resultado) =>
            //{
            //    listaProgresso.Add(resultado);
            //});

            var resultado = await ConsolidarContas(contas, taskProgresso);
            var fim = DateTime.Now;

            return AtualizarView(resultado, fim - inicio);

        }

        private async Task<string[]> ConsolidarContas(IEnumerable<ContaCliente> contas,
            IProgress<string> reportadorProgresso)
        {
            try
            {
                var contasTask = contas.Select(c =>
                 Task.Factory.StartNew(() =>
                 {
                     var resultado = r_Servico.ConsolidarMovimentacao(c);
                     reportadorProgresso.Report(resultado);
                     
                     return resultado;
                 }));

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
