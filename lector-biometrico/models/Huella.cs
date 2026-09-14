using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lector_biometrico.models
{
    internal class Huella
    {
        public string idusuario { get; set; } = "";
        public string idcliente { get; set; } = "";
        public byte[] huella { get; set; }
    }
}
