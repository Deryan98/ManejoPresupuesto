using System;
namespace ManejoPresupuesto.Models
{
	public class TransaccionActulizacionViewModel: TransaccionCreacionViewModel
    {
		public int CuentaAnteriorId { get; set; }
		public decimal MontoAnterior { get; set; }

	}
}

