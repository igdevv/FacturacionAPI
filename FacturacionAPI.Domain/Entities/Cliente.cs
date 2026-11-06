using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public string RncCedula { get; private set; } = string.Empty;
        public string? Correo { get; private set; }
        public string? Telefono { get; private set; }

        public Cliente(string nombre, string rncCedula, string? correo, string? telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(rncCedula))
            {
                throw new ArgumentException("El RNC o Cédula es obligatorio.");
            }

            if (rncCedula.Trim().Length > 450)
                throw new ArgumentException("El RNC o Cédula no puede superar los 450 caracteres.");

            Nombre = nombre.Trim();
            RncCedula = rncCedula.Trim();
            Correo = correo?.Trim().ToLowerInvariant();
            Telefono = telefono;
        }
    }
}
