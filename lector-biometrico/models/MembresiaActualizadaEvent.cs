using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lector_biometrico.models
{
    public class MembresiaActualizadaEvent
    {
        public string tipo { get; set; } = "";
        public Cliente data { get; set; }
    }
}
