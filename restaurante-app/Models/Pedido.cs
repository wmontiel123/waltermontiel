namespace RestaurantePedidos.Models;

public class Pedido
{
    public int Numero { get; set; }
    public string MesaId { get; set; } = string.Empty;
    public string MesaNombre { get; set; } = string.Empty;
    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
    public DateTime Creado { get; set; } = DateTime.Now;
    public DateTime? Cerrado { get; set; }
    public List<ItemPedido> Items { get; set; } = new();
    public List<RegistroEstado> Historial { get; set; } = new();

    public bool Activo => Cerrado is null;

    /// <summary>Momento del último cambio de estado.</summary>
    public DateTime UltimoCambio => Historial.Count > 0 ? Historial[^1].Momento : Creado;

    public int TotalPlatos => Items.Sum(i => i.Cantidad);

    public string ResumenItems =>
        Items.Count == 0 ? "Sin platos" : string.Join(", ", Items.Select(i => $"{i.Cantidad}× {i.Nombre}"));
}
