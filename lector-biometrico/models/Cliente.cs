using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lector_biometrico.models
{
    public class Cliente
    {
        public string idcliente { get; set; } = string.Empty;
        public string idusuario { get; set; } = string.Empty;
        public string nombre { get; set; } = "";
        public string sexo { get; set; } = "";
        public string telefono { get; set; } = "";
        public string email { get; set; } = "";
        public string direccion { get; set; } = "";
        public byte[] huella { get; set; }
        public DateTime? createdAt { get; set; }
        public Membresia membresia { get; set; }
    }

    public class Membresia
    {
        public string _id { get; set; }
        public string idcliente { get; set; }
        public DatosVenta idventa { get; set; }
        public DatosInventario inventario_id { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }
        public string estado { get; set; } = "activa"; // activa, congelada, vencida, cancelada
        public int visitasUsadas { get; set; } = 0;
        public HistorialCongelamiento historialCongelamiento { get; set; }
    }

    public class DatosInventario
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string tipo { get; set; }
        public Object configMembresia { get; set; }
    }

    public class DatosVenta
    {
        public string num_venta { get; set; }
        public DateTime fecha { get; set; }
        public decimal suma_total { get; set; }
        public string estado { get; set; }
        public string tipo { get; set; }
        public string observaciones { get; set; }
    }

    public class HistorialCongelamiento
    {
        public DateTime? desde { get; set; }
        public DateTime? hasta { get; set; }
    }
}
