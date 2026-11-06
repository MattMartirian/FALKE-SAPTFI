using System.Collections.Generic;
using System.Text;

namespace TLL
{
    public static class DetalleBitacora_TLL
    {
        public const int LARGO_MAXIMO = 1000;

        public static ParteBitacora_TLL Clave(string clave, params object[] parametros)
        {
            var texto = new StringBuilder("{");

            texto.Append(clave);

            if (parametros != null)
            {
                foreach (object parametro in parametros)
                {
                    texto.Append('|');
                    texto.Append(Contenido(parametro));
                }
            }

            texto.Append('}');

            return new ParteBitacora_TLL(texto.ToString());
        }

        public static ParteBitacora_TLL Partes(params object[] partes)
        {
            var texto = new StringBuilder();

            foreach (object parte in partes) texto.Append(Contenido(parte));

            return new ParteBitacora_TLL(texto.ToString());
        }

        public static string Armar(params object[] partes)
        {
            string texto = Partes(partes).Valor;

            if (texto.Length == 0) return null;

            return texto.Length > LARGO_MAXIMO ? texto.Substring(0, LARGO_MAXIMO) : texto;
        }

        public static ParteBitacora_TLL Lista(IEnumerable<ParteBitacora_TLL> elementos, string separador = "; ")
        {
            var texto = new StringBuilder();
            bool primero = true;

            foreach (ParteBitacora_TLL elemento in elementos)
            {
                if (!primero) texto.Append(Escapar(separador));

                texto.Append(elemento.Valor);
                primero = false;
            }

            return new ParteBitacora_TLL(texto.ToString());
        }

        public static ParteBitacora_TLL Motivo(string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo)) return new ParteBitacora_TLL(string.Empty);

            return Partes(" · ", Clave("bitacora_det_motivo", motivo));
        }

        public static string Traducir(string detalle, string codigoIdioma)
        {
            if (string.IsNullOrEmpty(detalle)) return string.Empty;

            int posicion = 0;

            return LeerTexto(detalle, ref posicion, codigoIdioma, string.Empty);
        }

        private static string Contenido(object parte)
        {
            if (parte == null) return string.Empty;

            var armada = parte as ParteBitacora_TLL;

            return armada != null ? armada.Valor : Escapar(parte.ToString());
        }

        private static string Escapar(string texto)
        {
            return texto.Replace("\\", "\\\\").Replace("{", "\\{").Replace("}", "\\}").Replace("|", "\\|");
        }

        private static string LeerTexto(string texto, ref int posicion, string codigoIdioma, string cortes)
        {
            var resultado = new StringBuilder();

            while (posicion < texto.Length)
            {
                char c = texto[posicion];

                if (c == '\\' && posicion + 1 < texto.Length)
                {
                    resultado.Append(texto[posicion + 1]);
                    posicion += 2;
                    continue;
                }

                if (c == '{')
                {
                    int inicio = posicion;
                    posicion++;

                    string traducido = LeerClave(texto, ref posicion, codigoIdioma);

                    if (traducido != null)
                    {
                        resultado.Append(traducido);
                        continue;
                    }

                    posicion = inicio + 1;
                    resultado.Append('{');
                    continue;
                }

                if (cortes.IndexOf(c) >= 0) break;

                resultado.Append(c);
                posicion++;
            }

            return resultado.ToString();
        }

        private static string LeerClave(string texto, ref int posicion, string codigoIdioma)
        {
            string clave = LeerTexto(texto, ref posicion, codigoIdioma, "|}");
            var parametros = new List<object>();

            while (posicion < texto.Length && texto[posicion] == '|')
            {
                posicion++;
                parametros.Add(LeerTexto(texto, ref posicion, codigoIdioma, "|}"));
            }

            if (posicion >= texto.Length || texto[posicion] != '}') return null;

            posicion++;

            return Traductor_TLL.Traducir(clave, codigoIdioma, parametros.ToArray());
        }
    }
}
