using System.Text.Json;
using Microsoft.JSInterop;
using RestaurantePedidos.Models;

namespace RestaurantePedidos.Services;

/// <summary>
/// Estado del restaurante (mesas, pedidos e historial), persistido en el
/// localStorage del navegador. Sin backend.
/// </summary>
public class EstadoService
{
    private const string StorageKey = "restaurante_estado_v2";
    private readonly IJSRuntime _js;

    public RestauranteState State { get; private set; } = new();
    public bool Loaded { get; private set; }

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
        Loaded = true;
    }

    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(State);
        await _js.InvokeVoidAsync("appStorage.set", StorageKey, json);
    }

    // ---- Mesas ----
    public Mesa AgregarMesa(double x, double y)
    {
        var mesa = new Mesa { Nombre = "Mesa " + (State.Mesas.Count + 1), X = Math.Round(x, 2), Y = Math.Round(y, 2) };
        State.Mesas.Add(mesa);
        return mesa;
    }

    public void EliminarMesa(Mesa mesa) => State.Mesas.Remove(mesa);

    // ---- Pedidos ----
    public Pedido CrearPedido(Mesa mesa)
    {
        var p = new Pedido
        {
            Numero = State.ProximoNumero++,
            MesaId = mesa.Id,
            MesaNombre = mesa.Nombre,
            Estado = EstadoPedido.Pendiente,
            Creado = DateTime.Now
        };
        p.Historial.Add(new RegistroEstado(EstadoPedido.Pendiente, p.Creado));
        mesa.Pedido = p;
        return p;
    }

    public void AgregarItem(Pedido pedido, string nombre, int cantidad)
    {
        nombre = nombre.Trim();
        if (string.IsNullOrEmpty(nombre) || cantidad < 1) return;
        var existente = pedido.Items.FirstOrDefault(i => string.Equals(i.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
        if (existente is not null) existente.Cantidad += cantidad;
        else pedido.Items.Add(new ItemPedido { Nombre = nombre, Cantidad = cantidad });
    }

    public void QuitarItem(Pedido pedido, ItemPedido item) => pedido.Items.Remove(item);

    public void CambiarEstado(Mesa mesa, EstadoPedido estado)
    {
        if (mesa.Pedido is null || mesa.Pedido.Estado == estado) return;
        mesa.Pedido.Estado = estado;
        mesa.Pedido.Historial.Add(new RegistroEstado(estado, DateTime.Now));
    }

    public void AvanzarEstado(Mesa mesa)
    {
        if (mesa.Pedido?.Estado.Siguiente() is { } siguiente)
            CambiarEstado(mesa, siguiente);
    }

    /// <summary>Cierra el pedido (lo archiva para estadísticas) y libera la mesa.</summary>
    public void CerrarMesa(Mesa mesa)
    {
        if (mesa.Pedido is null) return;
        if (mesa.Pedido.Estado is not EstadoPedido.Pagado and not EstadoPedido.Cancelado)
            CambiarEstado(mesa, EstadoPedido.Pagado);
        Archivar(mesa.Pedido);
        mesa.Pedido = null;
    }

    public void CancelarPedido(Mesa mesa)
    {
        if (mesa.Pedido is null) return;
        CambiarEstado(mesa, EstadoPedido.Cancelado);
        Archivar(mesa.Pedido);
        mesa.Pedido = null;
    }

    private void Archivar(Pedido pedido)
    {
        pedido.Cerrado = DateTime.Now;
        State.Historial.Add(pedido);
    }

    // ---- Consultas / estadísticas ----
    public IEnumerable<Pedido> Activos() =>
        State.Mesas.Where(m => m.Pedido is not null).Select(m => m.Pedido!);

    public IEnumerable<Pedido> Todos() => Activos().Concat(State.Historial);

    public List<Pedido> Completados() =>
        State.Historial.Where(p => p.Estado != EstadoPedido.Cancelado && p.Cerrado is not null).ToList();

    public int Cancelados() => State.Historial.Count(p => p.Estado == EstadoPedido.Cancelado);

    public TimeSpan? PromedioTiempoPedido()
    {
        var c = Completados();
        if (c.Count == 0) return null;
        var seg = c.Average(p => (p.Cerrado!.Value - p.Creado).TotalSeconds);
        return TimeSpan.FromSeconds(seg);
    }

    public List<(string Nombre, int Cantidad)> PlatosMasPedidos(int top = 8)
    {
        return Todos()
            .SelectMany(p => p.Items)
            .GroupBy(i => i.Nombre, StringComparer.OrdinalIgnoreCase)
            .Select(g => (Nombre: g.Key, Cantidad: g.Sum(i => i.Cantidad)))
            .OrderByDescending(x => x.Cantidad)
            .Take(top)
            .ToList();
    }

    public double PromedioMesasPorDia()
    {
        var dias = State.Historial
            .Where(p => p.Cerrado is not null)
            .GroupBy(p => p.Cerrado!.Value.Date)
            .ToList();
        return dias.Count == 0 ? 0 : dias.Average(g => g.Count());
    }

    public void ReiniciarTodo()
    {
        State = new RestauranteState();
        SeedEjemplo();
    }

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
