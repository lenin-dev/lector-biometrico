using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lector_biometrico
{
    internal class env
    {
        public String entorno() {
            var local = "http://localhost:3006";
            var produccion = "https://posxcodx.duckdns.org";
            return produccion;
        }
    }
}
