using lector_biometrico.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lector_biometrico.services
{
    public class VerificarMembresia
    {
        public async Task<MembresiaVerificada> VerificarMembresiaValida(string idcliente)
        {
            return await Task.Run(() =>
            {
                // Buscar solamente el cliente solicitado
                var cliente = ClienteStore.Clientes
                    .FirstOrDefault(c => c.idcliente == idcliente);

                if (cliente == null)
                    return null;

                // Verificar si tiene membresía
                var membresia = cliente.membresia;

                if (membresia == null)
                    return null;

                var zonaMexico = TimeZoneInfo.FindSystemTimeZoneById(
                    "Central Standard Time (Mexico)"
                );

                var ahora = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    zonaMexico
                );

                // Fecha de inicio y fin solamente como fecha
                var fechaInicio = membresia.fechaInicio;
                var fechaFin = membresia.fechaFin;

                // La membresía vence a las 18:00
                var vencimiento = fechaFin.AddHours(18);
                // Días restantes
                var diasRestantes = (int)(vencimiento.Date - ahora.Date).TotalDays;
                // Días vencida
                var diasVencida = (int)(ahora.Date - vencimiento.Date).TotalDays;
                var resultado = new MembresiaVerificada
                {
                    idmembresia = membresia._id,
                    idcliente = membresia.idcliente,
                    idventa = membresia.idcliente,
                    inventario_id = membresia.inventario_id,
                    fechaInicio = fechaInicio,
                    fechaFin = fechaFin,
                    visitasUsadas = membresia.visitasUsadas,
                    fechaVencimiento = vencimiento,
                    diaVencimiento = vencimiento.ToString("dddd"),
                    horaVencimiento = vencimiento.ToString("HH:mm:ss"),
                    cliente = cliente,
                    //historialCongelamiento = membresia.historialCongelamiento
                };

                if(membresia.estado == "cancelada")
                {
                    resultado.estado = "cancelada";
                    resultado.diasRestantes = 0;
                    resultado.diasVencida = diasVencida;
                } else if (membresia.estado == "congelada")
                {
                    resultado.estado = "congelada";
                    resultado.diasRestantes = 0;
                    resultado.diasVencida = diasVencida;
                } else if (ahora > vencimiento)
                {
                    resultado.estado = "vencida";
                    resultado.diasRestantes = 0;
                    resultado.diasVencida = diasVencida;
                }
                else
                {
                    resultado.estado = "vigente";
                    resultado.diasRestantes = diasRestantes;
                    resultado.diasVencida = 0;
                }

                return resultado;
            });
        }

        public class MembresiaVerificada
        {
            public string idmembresia { get; set; } = string.Empty;
            public string idcliente { get; set; } = string.Empty;
            public object idventa { get; set; } = string.Empty;
            public object inventario_id { get; set; } = string.Empty;
            public DateTime fechaInicio { get; set; }
            public DateTime fechaFin { get; set; }
            public string estado { get; set; } = string.Empty;
            public int visitasUsadas { get; set; }
            public List<HistorialCongelamiento> historialCongelamiento { get; set; }
            public DateTime fechaVencimiento { get; set; }
            public string diaVencimiento { get; set; } = string.Empty;
            public string horaVencimiento { get; set; } = string.Empty;
            public int diasRestantes { get; set; }
            public int diasVencida { get; set; }
            public Cliente cliente { get; set; }
        }
    }
}
