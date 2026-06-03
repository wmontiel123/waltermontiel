namespace RestaurantePedidos.Models;

public class RestauranteState
{
    public List<Mesa> Mesas { get; set; } = new();
    public int ProximoNumero { get; set; } = 1;

    /// <summary>Pedidos cerrados/cancelados, conservados para estadísticas.</summary>
    public List<Pedido> Historial { get; set; } = new();
}
