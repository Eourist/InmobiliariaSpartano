using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaSpartano.Models
{
    public enum Estados
    {
        Abierto = 1,
        Terminado = 2,
        Renovado = 3,
        Roto = 4
    }
    public class Contrato : Entidad
    {
        [Required(ErrorMessage = "Campo obligatorio"),
            ForeignKey("InmuebleId"),
            Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            ForeignKey("InquilinoId"),
            Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            Display(Name = "Fecha inicial")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            Display(Name = "Fecha final")]
        public DateTime FechaHasta { get; set; }

        [Display(Name = "Inquilino")]
        public Inquilino Inquilino { get; set; }

        [Display(Name = "Inmueble")]
        public Inmueble Inmueble { get; set; }

        public int Estado { get; set; }

        public int TotalMeses => Math.Max((FechaHasta.Year - FechaDesde.Year) * 12 + FechaHasta.Month - FechaDesde.Month, 1);
        public int TotalImporte => TotalMeses * Inmueble.Precio;
        public int CantidadPagos { get; set; }
        public DateTime ProximoPago => new DateTime(Math.Min(FechaDesde.AddMonths(CantidadPagos).Ticks, FechaHasta.Ticks));
        public string ProximoPagoTexto => ProximoPago == FechaHasta ? "N/A" : ProximoPago.ToShortDateString();
        public string EstadoPagos => Estado == 1 ? ProximoPago > DateTime.Now ? "Al día" : ProximoPago.Month == DateTime.Now.Month ? "Pendiente" : "Atrasado" : "N/A";
        public string NombreEstado => ((Estados)Estado).ToString();
        public string ResumenPagos => $"{CantidadPagos}/{TotalMeses} mes{(TotalMeses > 1 ? "es" : "")} pagado{(TotalMeses > 1 ? "s" : "")}";
        public bool MitadContratoCumplida => CantidadPagos > (TotalMeses / 2);

        public override string ToString()
        {
            return $"#{Id} - {Inmueble.Direccion} ({FechaDesde.Month}/{FechaDesde.Year} - {FechaHasta.Month}/{FechaHasta.Year})";
        }
    }
}
