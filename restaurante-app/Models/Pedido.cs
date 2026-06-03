namespace RestaurantePedidos.Models;

public class Pedido
{
    public int Numero { get; set; }
    public EstadoPedido Estado { get; set; } = EstadoPedido.Pendiente;
    public DateTime Creado { get; set; } = DateTime.Now;
    public string Notas { get; set; } = string.Empty;
}
