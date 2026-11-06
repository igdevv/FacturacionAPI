using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Facturas
{
    public class CrearFacturaDto
    {
        [Range(1, int.MaxValue)]
        public int ClienteId { get; set; }
        [Required, MinLength(1)]
        public List<ProductoFacturaDto> Productos { get; set; } = new();
    }

    public class ProductoFacturaDto
    {
        [Range(1, int.MaxValue)]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}
