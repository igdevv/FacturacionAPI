using FacturacionAPI.Application.Clientes.DTOs;
using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using FacturacionAPI.Domain.Excepciones;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Clientes.CrearCliente
{
    public class CrearClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public CrearClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ClienteResponse> EjecutarAsync(CrearClienteDto dto)
        {
            var documento = dto.RncCedula?.Trim();

            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("La cédula o RNC es obligatorio.");

            bool existe = await _clienteRepository.ExistePorDocumentoAsync(documento);

            if (existe)
                throw new ConflictoNegocioException("Ya existe un cliente con ese documento.");

            var cliente = new Cliente(
                dto.Nombre,
                documento,
                dto.Correo,
                dto.Telefono);

            await _clienteRepository.AgregarAsync(cliente);

            return new ClienteResponse
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                RncCedula = cliente.RncCedula,
                Correo = cliente.Correo,
                Telefono = cliente.Telefono
            };
        }  
        
    }
}
