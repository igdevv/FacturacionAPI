using FacturacionAPI.Application.Clientes.DTOs;
using FacturacionAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FacturacionAPI.Application.Clientes.ObtenerClientesPorId
{
    public class ObtenerClientePorIdService
    {
        private readonly IClienteRepository _clienteRepository;

        public ObtenerClientePorIdService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ClienteResponse?> ObtenerPorIdAsync(int id)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);

            if (cliente is null)
                return null;

            return new ClienteResponse
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Correo = cliente.Correo,
                RncCedula = cliente.RncCedula,
                Telefono = cliente.Telefono
            }; 

        } 
    }
}
