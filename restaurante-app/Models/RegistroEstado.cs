namespace RestaurantePedidos.Models;

/// <summary>Marca de tiempo de cada cambio de estado de un pedido.</summary>
public class RegistroEstado
{
    public EstadoPedido Estado { get; set; }
    public DateTime Momento { get; set; }

    public RegistroEstado() { }
    public RegistroEstado(EstadoPedido estado, DateTime momento)
    {
        Estado = estado;
        Momento = momento;
    }
}
