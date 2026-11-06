using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Clientes.DTOs
{
    public class ClienteResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string RncCedula { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
    }
}
