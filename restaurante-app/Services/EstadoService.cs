using System.Text.Json;
using Microsoft.JSInterop;
using RestaurantePedidos.Models;

namespace RestaurantePedidos.Services;

/// <summary>
/// Estado del restaurante (mesas + pedidos), persistido en el localStorage
/// del navegador. Sin backend: ideal para demo.
/// </summary>
public class EstadoService
{
    private const string StorageKey = "restaurante_estado_v1";
    private readonly IJSRuntime _js;

    public RestauranteState State { get; private set; } = new();

    public EstadoService(IJSRuntime js) => _js = js;

    public async Task LoadAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("appStorage.get", StorageKey);
            if (!string.IsNullOrWhiteSpace(json))
                State = JsonSerializer.Deserialize<RestauranteState>(json) ?? new();
        }
        catch { State = new(); }

        if (State.Mesas.Count == 0) SeedEjemplo();
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(State);
        await _js.InvokeVoidAsync("appStorage.set", StorageKey, json);
    }

    public Mesa AgregarMesa(double x, double y)
    {
        var mesa = new Mesa { Nombre = "Mesa " + (State.Mesas.Count + 1), X = Math.Round(x, 2), Y = Math.Round(y, 2) };
        State.Mesas.Add(mesa);
        return mesa;
    }

    public void EliminarMesa(Mesa mesa) => State.Mesas.Remove(mesa);

    public Pedido CrearPedido(Mesa mesa)
    {
        mesa.Pedido = new Pedido { Numero = State.ProximoNumero++ };
        return mesa.Pedido;
    }

    public void AvanzarEstado(Mesa mesa)
    {
        if (mesa.Pedido?.Estado.Siguiente() is { } siguiente)
            mesa.Pedido.Estado = siguiente;
    }

    public void CambiarEstado(Mesa mesa, EstadoPedido estado)
    {
        if (mesa.Pedido is not null) mesa.Pedido.Estado = estado;
    }

    public void CerrarMesa(Mesa mesa) => mesa.Pedido = null;

    private void SeedEjemplo()
    {
        State.Mesas.AddRange(new[]
        {
            new Mesa { Nombre = "Mesa 1", X = 31, Y = 36 },
            new Mesa { Nombre = "Mesa 2", X = 41, Y = 36 },
            new Mesa { Nombre = "Mesa 3", X = 62, Y = 36 },
            new Mesa { Nombre = "Mesa 4", X = 72, Y = 36 },
            new Mesa { Nombre = "Mesa 5", X = 31, Y = 52 },
            new Mesa { Nombre = "Mesa 6", X = 41, Y = 52 },
            new Mesa { Nombre = "VIP 1",  X = 11, Y = 36 },
            new Mesa { Nombre = "Barra 1", X = 89, Y = 36 },
        });
    }
}
