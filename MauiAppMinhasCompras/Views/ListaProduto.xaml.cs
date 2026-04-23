using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    // Método para carregar produtos
    private async Task LoadProdutos(string categoria = "Todas")
    {
        lista.Clear();
        List<Produto> tmp = await App.Db.GetByCategoria(categoria);
        tmp.ForEach(i => lista.Add(i));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await LoadProdutos();

            // Buscamos as categorias do banco
            var cats = await App.Db.GetCategoriasAsync();
            var catList = new List<string> { "Todas" };
            catList.AddRange(cats);

            // Agora o C# reconhece pck_filtro_categoria como um Picker real
            pck_filtro_categoria.ItemsSource = catList;
            pck_filtro_categoria.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void pck_filtro_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (pck_filtro_categoria.SelectedItem != null)
            {
                string categoriaSelecionada = pck_filtro_categoria.SelectedItem.ToString();
                await LoadProdutos(categoriaSelecionada);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Relatorio_Clicked(object sender, EventArgs e)
    {
        try
        {
            var grupos = lista.GroupBy(i => string.IsNullOrEmpty(i.Categoria) ? "Sem categoria" : i.Categoria)
                              .Select(g => new
                              {
                                  Categoria = g.Key,
                                  Total = g.Sum(i => i.Total)
                              })
                              .OrderBy(g => g.Categoria)
                              .ToList();

            string msg = "📊 Relatório de Gastos por Categoria\n\n";
            double totalGeral = 0;

            foreach (var g in grupos)
            {
                msg += $"{g.Categoria}: {g.Total:C}\n";
                totalGeral += g.Total;
            }

            msg += $"\n────────────────────\nTotal Geral: {totalGeral:C}";

            await DisplayAlert("Relatório por Categoria", msg, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue;
            lista.Clear();
            List<Produto> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;
            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert("Tem certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem != null)
            {
                Produto p = e.SelectedItem as Produto;
                await Navigation.PushAsync(new Views.EditarProduto { BindingContext = p });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {

    }
}