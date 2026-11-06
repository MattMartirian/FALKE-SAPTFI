using System;
using System.Collections.Generic;
using TE;
using TLL;

namespace GUI
{
    public static class Informe_GUI
    {
        public static string IdiomaValido(string codigo)
        {
            foreach (Idioma_TE idioma in new Idioma_TLL().ListarActivos())
            {
                if (string.Equals(idioma.CodigoIdioma, codigo, StringComparison.OrdinalIgnoreCase)) return idioma.CodigoIdioma;
            }

            return IdiomaActual_GUI.Codigo;
        }

        public static Dictionary<string, string> Textos(IEnumerable<string> claves, string codigo)
        {
            var textos = new Dictionary<string, string>();

            foreach (string clave in claves) textos[clave] = Traductor_TLL.Traducir(clave, codigo);

            return textos;
        }

        public static string EtiquetaTipo(string tipo, string codigo)
        {
            switch (tipo)
            {
                case "AppWeb": return Traductor_TLL.Traducir("analisismultiple_app_web", codigo);
                case "AppMovil": return Traductor_TLL.Traducir("analisismultiple_app_movil", codigo);
                case "Videojuego": return Traductor_TLL.Traducir("analisismultiple_videojuego", codigo);
                case "Publicidad": return Traductor_TLL.Traducir("categorias_publicidad", codigo);
                case "Software": return Traductor_TLL.Traducir("analisismultiple_software", codigo);
                default: return tipo;
            }
        }
    }
}
