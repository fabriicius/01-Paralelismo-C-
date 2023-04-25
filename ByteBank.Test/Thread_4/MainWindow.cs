using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;

namespace ByteBank.Test.Thread_4
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

        public void Procesar() => BtnProcessar_Click();

        private void BtnProcessar_Click()
        {
            var tasksScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            botaoProcessar = false;

            var contas = r_Repositorio.GetContaClientes();

            var resultado = new List<string>();

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;
            
            var contasTask = contas.Select( c => {

               return Task.Factory.StartNew(() => 
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(c);
                    resultado.Add(resultadoConta);
                });
            }).ToArray();

            Task.WhenAll(contasTask)
            .ContinueWith((task) => {
                    var fim = DateTime.Now;
                    AtualizarView(resultado, fim - inicio);
            } ,tasksScheduler)
            .ContinueWith((task) => {
                botaoProcessar = true;
            } ,tasksScheduler);
 
        }

        private void AtualizarView(List<String> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count} clientes em {tempoDecorrido}";


            Console.WriteLine("--LstResultados.ItemsSource--");
            Console.WriteLine(result);

            Console.WriteLine("--TxtTempo.Text--");
            Console.WriteLine(mensagem);

            // LstResultados.ItemsSource = result;
            // TxtTempo.Text = mensagem;
        }
    }
}
