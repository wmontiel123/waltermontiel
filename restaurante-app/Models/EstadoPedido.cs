namespace RestaurantePedidos.Models;

public enum EstadoPedido
{
    Pendiente,
    EnPreparacion,
    Listo,
    Entregado,
    Pagado
}

public static class EstadoPedidoExtensions
{
    public static string Label(this EstadoPedido e) => e switch
    {
        EstadoPedido.Pendiente => "Pendiente",
        EstadoPedido.EnPreparacion => "En preparación",
        EstadoPedido.Listo => "Listo",
        EstadoPedido.Entregado => "Entregado",
        EstadoPedido.Pagado => "Pagado",
        _ => "—"
    };

    public static string Color(this EstadoPedido e) => e switch
    {
        EstadoPedido.Pendiente => "#e0b24a",
        EstadoPedido.EnPreparacion => "#5b9bd5",
        EstadoPedido.Listo => "#5cb85c",
        EstadoPedido.Entregado => "#4aa3a3",
        EstadoPedido.Pagado => "#8a8478",
        _ => "#8a8478"
    };

    public static EstadoPedido? Siguiente(this EstadoPedido e) => e switch
    {
        EstadoPedido.Pendiente => EstadoPedido.EnPreparacion,
        EstadoPedido.EnPreparacion => EstadoPedido.Listo,
        EstadoPedido.Listo => EstadoPedido.Entregado,
        EstadoPedido.Entregado => EstadoPedido.Pagado,
        _ => null
    };
}
