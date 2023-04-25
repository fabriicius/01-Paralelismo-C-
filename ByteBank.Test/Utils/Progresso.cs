using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteBank.Test.Utils
{
    public class Progresso<T> : IProgress<T>
    {
        private readonly Action<T> _handler;

        public Progresso(Action<T> handler)
        {
            _handler = handler;
        }

        public void Report(T value)
        {
            Task.Factory.StartNew(() => { _handler(value); });
        }
    }
}
