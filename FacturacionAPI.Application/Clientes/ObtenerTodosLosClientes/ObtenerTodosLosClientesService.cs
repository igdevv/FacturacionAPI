using FacturacionAPI.Application.Clientes.DTOs;
using FacturacionAPI.Application.Interfaces;
using FacturacionAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Clientes.ObtenerTodosLosClientes
{
    public class ObtenerTodosLosClientesService
    {
        private readonly IClienteRepository _clienteRepository;

        public ObtenerTodosLosClientesService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<List<ClienteResponse>> ObtenerTodosAsync()
        {
            var clientes = await _clienteRepository.ObtenerTodosAsync();

            return clientes.Select(c => new ClienteResponse
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Correo = c.Correo,
                RncCedula = c.RncCedula,
                Telefono = c.Telefono
            }).ToList();
        }
    }
}
