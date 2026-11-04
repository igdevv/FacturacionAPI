using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.DTOs
{
    public class ProductoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
