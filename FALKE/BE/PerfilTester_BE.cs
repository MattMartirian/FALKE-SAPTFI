using System;

namespace BE
{
    public static class PerfilTester_BE
    {
        public static readonly string[] RangosEdad = { "-18", "18-24", "25-34", "35-44", "45-54", "55-64", "65+" };
        public static readonly string[] Generos = { "femenino", "masculino", "no_binario", "no_informa" };
        public static readonly string[] Familiaridades = { "baja", "media", "alta" };

        public static bool EsValido(string rangoEdad, string genero, string familiaridad)
        {
            return Array.IndexOf(RangosEdad, rangoEdad) >= 0
                && Array.IndexOf(Generos, genero) >= 0
                && Array.IndexOf(Familiaridades, familiaridad) >= 0;
        }
    }
}
