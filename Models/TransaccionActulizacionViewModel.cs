namespace ManejoPresupuesto.Models;

public class TransaccionActulizacionViewModel : TransaccionCreacionViewModel
{
    public decimal MontoAnterior { get; set; }
    public int CuentaAnteriorId { get; set; }
}