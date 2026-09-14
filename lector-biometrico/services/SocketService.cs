using lector_biometrico.models;
using SocketIOClient;
using SocketIOClient.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace lector_biometrico.services
{
    public class SocketService
    {
        private readonly SocketIO _socket;
        private readonly string _databaseName;

        public bool EstaConectado => _socket.Connected;

        public SocketService(string url, string databaseName)
        {
            _databaseName = databaseName;

            _socket = new SocketIO(
                new Uri(url),
                new SocketIOOptions
                {
                    Reconnection = true,
                    Transport = TransportProtocol.WebSocket
                }
            );

            ConfigurarEventos();
        }

        private void ConfigurarEventos()
        {
            _socket.OnConnected += async (sender, e) =>
            {
                Console.WriteLine("================================");
                Console.WriteLine("SOCKET CONECTADO");
                Console.WriteLine("================================");

                await RegistrarGimnasioAsync();
            };

            _socket.OnDisconnected += (sender, e) =>
            {
                Console.WriteLine(
                    $"Socket desconectado: {e}"
                );
            };

            _socket.OnError += (sender, e) =>
            {
                Console.WriteLine(
                    $"Socket error: {e}"
                );
            };

            _socket.On("usuario_nuevo",
                async context =>
                {
                    Console.WriteLine("Membresía actualizada");

                    // si ya existe el usuario actualizar el usuario de la lista Cliente y si no existe agregarlo a la lista

                    await Task.CompletedTask;
                }
            );

            _socket.On("cliente_nuevo",
                async context =>
                {
                    try
                    {
                        var datos = context.GetValue<MembresiaActualizadaEvent>(0);

                        if (datos == null)
                            return;

                        Console.WriteLine(
                            $"Evento cliente_nuevo: {datos.tipo}"
                        );

                        switch (datos.tipo)
                        {
                            case "nuevo":
                                AgregarCliente(datos.data);
                                break;

                            case "editar":
                                ActualizarCliente(datos.data);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error procesando cliente_nuevo:");
                        Console.WriteLine(ex.ToString());
                    }

                    await Task.CompletedTask;
                }
            );

        }

        public async Task ConectarAsync()
        {
            try
            {
                Console.WriteLine("Intentando conectar Socket.IO...");
                await _socket.ConnectAsync();

                Console.WriteLine(
                    $"Conectado: {_socket.Connected}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "ERROR SOCKET.IO"
                );

                Console.WriteLine(
                    ex.ToString()
                );

                throw;
            }
        }

        private async Task RegistrarGimnasioAsync()
        {
            if (!_socket.Connected)
                return;

            await _socket.EmitAsync("biplo_appLectorHuella",
                new object[]
                {
                    _databaseName
                }
            );

            Console.WriteLine(
                $"Registrado en room: {_databaseName}"
            );
        }

        public async Task DesconectarAsync()
        {
            if (!_socket.Connected)
                return;

            await _socket.DisconnectAsync();
        }

        /*
         * ========================================
         * CLIENTES
         * ========================================
         */

        private void AgregarCliente(Cliente cliente)
        {
            if (cliente == null)
                return;

            if (string.IsNullOrEmpty(
                cliente.idcliente
            ))
                return;

            var existe = ClienteStore.Clientes
                .Any(x =>
                    x.idcliente ==
                    cliente.idcliente
                );

            if (existe)
            {
                /*
                 * Si ya existe, actualizamos.
                 */
                ActualizarCliente(cliente);

                return;
            }

            ClienteStore.Clientes.Add(
                cliente
            );

            Console.WriteLine(
                $"Cliente agregado: {cliente.nombre}"
            );
        }

        private void ActualizarCliente(
            Cliente cliente
        )
        {
            if (cliente == null)
                return;

            if (string.IsNullOrEmpty(
                cliente.idcliente
            ))
                return;

            var clienteExistente =
                ClienteStore.Clientes
                    .FirstOrDefault(x =>
                        x.idcliente ==
                        cliente.idcliente
                    );

            if (clienteExistente == null)
            {
                /*
                 * Si no existe, lo agregamos.
                 */
                ClienteStore.Clientes.Add(cliente);

                Console.WriteLine($"Cliente agregado: {cliente.nombre}");

                return;
            }

            /*
             * Actualizar propiedades.
             */
            clienteExistente.idusuario = cliente.idusuario;
            clienteExistente.nombre = cliente.nombre;
            clienteExistente.sexo = cliente.sexo;
            clienteExistente.telefono = cliente.telefono;
            clienteExistente.email = cliente.email;
            clienteExistente.direccion = cliente.direccion;
            clienteExistente.huella = cliente.huella;
            clienteExistente.createdAt = cliente.createdAt;
            clienteExistente.membresia = cliente.membresia;

            Console.WriteLine($"Cliente actualizado: {cliente.nombre}");
        }
    }
}