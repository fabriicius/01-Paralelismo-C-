using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ByteBank.Test.Thread_6
{
    public class MainWindow : Window
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;
        private bool botaoProcessar = true;

        public MainWindow()
        {
            
            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
        }


        public async void BtnProcessar_Click()
        {
            botaoProcessar = false;

            var contas = r_Repositorio.GetContaClientes();

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;

            var resultado = await ConsolidarContas(contas);

            var fim = DateTime.Now;
            AtualizarView(resultado, fim - inicio);
            botaoProcessar = true;
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

        private void AtualizarView(IEnumerable<String> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{elapsedTime.Seconds}.{elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count()} clientes em {tempoDecorrido}";


            Console.WriteLine("--LstResultados.ItemsSource--");
            Console.WriteLine(result);


            Console.WriteLine("--TxtTempo.Text--");
            Console.WriteLine(mensagem);

             //LstResultados.ItemsSource = result;
             //TxtTempo.Text = mensagem;
        }
    }
}
