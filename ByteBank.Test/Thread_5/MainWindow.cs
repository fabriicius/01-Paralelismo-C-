using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;

namespace ByteBank.Test.Thread_5
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
        public List<String> resultado = new();

        private void BtnProcessar_Click()
        {
            var tasksScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            botaoProcessar = false;

            var contas = r_Repositorio.GetContaClientes();

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;
            
            ConsolidarContas(contas)
            .ContinueWith((task) => {
                    var fim = DateTime.Now;
                    var resultado = task.Result;
                    AtualizarView(resultado, fim - inicio);
            } ,tasksScheduler)
            .ContinueWith((task) => {
                botaoProcessar = true;
            } ,tasksScheduler);
 
        }

        private Task<List<string>> ConsolidarContas(IEnumerable<ContaCliente> contas)
        {
            var resultado = new List<string>();

            var contasTask = contas.Select( c => {

               return Task.Factory.StartNew(() => 
                {
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(c);
                    resultado.Add(resultadoConta);
                });
            });
            
            return Task.WhenAll(contasTask).ContinueWith((t) => {
                return resultado;
            });
        }

        private void AtualizarView(List<String> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count} clientes em {tempoDecorrido}";


            Console.WriteLine("--LstResultados.ItemsSource--");
            Console.WriteLine(result);

            Console.WriteLine("--TxtTempo.Text--");
            Console.WriteLine(mensagem);
            resultado = resultado;

            // LstResultados.ItemsSource = result;
            // TxtTempo.Text = mensagem;
        }
    }
}
