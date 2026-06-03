namespace RestaurantePedidos.Models;

public class RestauranteState
{
    public List<Mesa> Mesas { get; set; } = new();
    public int ProximoNumero { get; set; } = 1;
}
