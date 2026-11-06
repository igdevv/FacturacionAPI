using FacturacionAPI.Application.Clientes.DTOs;
using FacturacionAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Clientes.ObtenerClientes
{
    public class ObtenerClientesService
    {
        private readonly IClienteRepository _clienteRepository;

        public ObtenerClientesService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<List<ClienteResponse>> ObtenerTodosAsync()
        {
            var cliente = await _clienteRepository.ObtenerTodosAsync();

            return cliente.Select(c => new ClienteResponse
            {
                Id = c.Id,
                Nombre = c.Nombre,
                RncCedula = c.RncCedula,
                Correo = c.Correo,
                Telefono = c.Telefono
            }).ToList();
        }
    }
}
