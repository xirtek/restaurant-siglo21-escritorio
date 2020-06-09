using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaPresentacion
{
    public class Conexion
    {
        public static string obtenerCadena()
        {
            string cadenaConexion = "DATA SOURCE = XE ; PASSWORD=oracle ; USER ID=restaurante21;";
            return cadenaConexion;
        }
    }
}
