using MauiAppMinhasCompras.Models;
using SQLite;


namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();

            // Migração automática para o novo campo Categoria (caso o banco já exista)
            try
            {
                _conn.ExecuteAsync("ALTER TABLE Produto ADD COLUMN Categoria TEXT").Wait();
            }
            catch { /* coluna já existe */ }
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }

        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=?, Categoria=? WHERE Id=?";

            return _conn.QueryAsync<Produto>(sql, p.Descricao, p.Quantidade, p.Preco, p.Categoria, p.Id);
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE descricao LIKE '%" + q + "%'";

            return _conn.QueryAsync<Produto>(sql);
        }

        // Novo método: filtro por categoria específica
        public Task<List<Produto>> GetByCategoria(string categoria)
        {
            if (string.IsNullOrEmpty(categoria) || categoria == "Todas")
                return GetAll();

            string sql = "SELECT * FROM Produto WHERE Categoria LIKE '%" + categoria + "%'";
            return _conn.QueryAsync<Produto>(sql);
        }

        // Novo método: retorna todas as categorias cadastradas
        public async Task<List<string>> GetCategoriasAsync()
        {
            // 1. Buscamos a lista completa de produtos que já sabemos mapear
            var produtos = await _conn.Table<Produto>().ToListAsync();

            // 2. Usamos o LINQ (Select) para pegar apenas o campo Categoria de cada produto
            return produtos
                    .Select(p => p.Categoria)              // "De cada produto, me dê só o texto da categoria"
                    .Where(c => !string.IsNullOrEmpty(c))  // Remove se houver alguma categoria vazia
                    .Distinct()                            // Se tiver 10 itens "Alimentos", deixa só 1
                    .OrderBy(c => c)                       // Coloca em ordem A-Z
                    .ToList();                             // Transforma o resultado em uma lista de textos (strings)
        }
    }
}
