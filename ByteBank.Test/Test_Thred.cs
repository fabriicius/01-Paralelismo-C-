using System.Diagnostics;
using System.Threading.Tasks;
using ByteBank.Test.Thread_1;
using Xunit.Abstractions;


namespace ByteBank.Test;

public class Test_Thred : IDisposable
{
    public ITestOutputHelper _output { get; } 

    public Test_Thred(ITestOutputHelper output)
    {
         _output = output;
    }

    [Fact]
    public void Thread_1()
    {
        var temp = Stopwatch.StartNew();
        temp.Start();

        MainWindow main = new MainWindow();
        main.Procesar();

        temp.Stop();

        _output.WriteLine("Tempo de Duracao ---> " + temp.Elapsed.Duration().ToString());

        Assert.True(temp.Elapsed.Duration() > TimeSpan.FromSeconds(50) , "Tempo de Duracao Maior q 30s");
    }

    [Fact]
    public void Thread_2()
    {
        var temp = Stopwatch.StartNew();
        temp.Start();

        Thread_2.MainWindow main = new Thread_2.MainWindow();

        main.Procesar();

        temp.Stop();

        _output.WriteLine("Tempo de Duracao ---> " + temp.Elapsed.Duration().ToString());

        Assert.True(temp.Elapsed.Duration() < TimeSpan.FromSeconds(50) , "Tempo de Duracao Maior q 30s");
    }
    
    [Fact]
    public void Thread_3()
    {
        var temp = Stopwatch.StartNew();
        temp.Start();

        Thread_3.MainWindow main = new Thread_3.MainWindow();

        main.Procesar();

        temp.Stop();

        _output.WriteLine("Tempo de Duracao ---> " + temp.Elapsed.Duration().ToString());

        Assert.True(temp.Elapsed.Duration() < TimeSpan.FromSeconds(50) , "Tempo de Duracao Maior q 30s");
    }

     [Fact]
    public void Thread_4()
    {
        var temp = Stopwatch.StartNew();
        temp.Start();

        Thread_4.MainWindow main = new Thread_4.MainWindow();

        main.Procesar();

        temp.Stop();

        _output.WriteLine("Tempo de Duracao ---> " + temp.Elapsed.Duration().ToString());

        Assert.True(temp.Elapsed.Duration() < TimeSpan.FromSeconds(50) , "Tempo de Duracao Maior q 30s");
    }

     [Fact]
    public void Thread_5()
    {
        var temp = Stopwatch.StartNew();
        temp.Start();

        Thread_5.MainWindow main = new Thread_5.MainWindow();

        main.Procesar();

        temp.Stop();

        _output.WriteLine("Tempo de Duracao ---> " + temp.Elapsed.Duration().ToString());

        Assert.True(main.resultado.Count > 0);
    }

    [Fact]
    public async void Thread_6()
    {
        var tasksScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        var temp = Stopwatch.StartNew();
        temp.Start();

        Thread_6.MainWindow main = new Thread_6.MainWindow();

        var task = Task.Run(() => main.BtnProcessar_Click());
        var retorno = await Task.WhenAll(task);

        temp.Stop();
        _output.WriteLine("Tempo de Duracao ---> " + temp.Elapsed.Duration().ToString());

        Assert.NotEmpty(retorno);
    }

    [Fact]
    public async void Thread_7()
    {
        var temp = Stopwatch.StartNew();
        temp.Start();

        Thread_7.MainWindow main = new Thread_7.MainWindow();

        var task = await Task.Factory.StartNew(() => main.BtnProcessar_Click()).Result;
        //var retorno = await Task.WhenAll(task);

        temp.Stop();
        _output.WriteLine("Tempo de Duracao ---> " + temp.Elapsed.Duration().ToString());
     
        Assert.True(main.listaProgresso.Count == task.Count());
    }

    [Fact]
    public async void Thread_8()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancelando o token
        var cancellationToken = cts.Token;

        try
        {
            var temp = Stopwatch.StartNew();
            temp.Start();

           Thread_8.MainWindow main = new Thread_8.MainWindow();
           
           await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await main.BtnProcessar_Click(cts);
            });

        }
        catch (OperationCanceledException ex)
        {
            Assert.Equal(cancellationToken, ex.CancellationToken);
        }
    }

    public void Dispose()
    {
        _output.WriteLine("Dispose invocado");
    }
}