namespace RestaurantePedidos.Models;

public class Mesa
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Nombre { get; set; } = "Mesa";

    /// <summary>Posición horizontal sobre el plano, en % (0-100).</summary>
    public double X { get; set; }

    /// <summary>Posición vertical sobre el plano, en % (0-100).</summary>
    public double Y { get; set; }

    public Pedido? Pedido { get; set; }

    public bool Ocupada => Pedido is not null;
}
