using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Productos.CrearProducto
{
    public class CrearProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
