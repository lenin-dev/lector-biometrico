using lector_biometrico.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lector_biometrico.services
{
    internal class clienteService
    {
        private readonly HttpClient _httpClient;
        private string conn;

        public clienteService()
        {
            this.conn = Properties.Settings.Default.conn;
            var env = new env();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(env.entorno().ToString())
            };
        }

        public async Task<List<Cliente>> ObtenerClientes()
        {
            // ENVIAR DATO DE LA DB INTERNAMENTE
            var response = await _httpClient.GetAsync(
                $"/api/v1/membresia/all/clientes?con={this.conn}"
            );
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var clientes = System.Text.Json.JsonSerializer.Deserialize<List<Cliente>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            ClienteStore.Clientes = null;
            ClienteStore.Clientes = clientes ?? new List<Cliente>();

            return clientes ?? new List<Cliente>();
        }

        public async Task<Cliente> GuardarClientes(Cliente clientes)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(clientes);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );


            // ENVIAR DATO DE LA DB INTERNAMENTE


            var response = await _httpClient.PostAsync(
                $"/api/v1/membresia/clientes/crear?con={this.conn}",
                content
            );

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            return System.Text.Json.JsonSerializer.Deserialize<Cliente>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        public async Task<Cliente> AsignarHuellaCliente(string idusuario, string idcliente, byte[] huella)
        {
            var cliente = new { idcliente = idcliente, idusuario = idusuario, huella = huella };
            var json = System.Text.Json.JsonSerializer.Serialize(cliente);
            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"/api/v1/membresia/clientes/asignar/huella?con={this.conn}",
                content
            );

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();

            Cliente clienteResp = null;
            if (!string.IsNullOrWhiteSpace(idcliente))
            {
                clienteResp = ClienteStore.Clientes
                    .FirstOrDefault(c => c.idcliente == idcliente);
            }
            else if (!string.IsNullOrWhiteSpace(idusuario))
            {
                clienteResp = ClienteStore.Clientes
                    .FirstOrDefault(c => c.idusuario == idusuario);
            }

            clienteResp.huella = huella;
            return clienteResp;
        }
    }
}
