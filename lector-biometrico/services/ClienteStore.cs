using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using lector_biometrico.models;

namespace lector_biometrico.services
{
    internal class ClienteStore
    {
        public static List<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}
