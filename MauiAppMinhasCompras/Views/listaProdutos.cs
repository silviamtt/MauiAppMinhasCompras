using System.Collections;

namespace MauiAppMinhasCompras.Views
{
    internal class listaProdutos : IEnumerable<object>
    {
        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}