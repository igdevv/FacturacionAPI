using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Clientes.CrearCliente
{
    public class CrearClienteDto
    {
        public string Nombre { get; set; } = string.Empty;
        [Required, StringLength(450)]
        public string RncCedula { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
    }
}
