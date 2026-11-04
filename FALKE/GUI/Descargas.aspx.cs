using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TLL;

namespace GUI
{
    public partial class Descargas : PaginaConAviso_GUI
    {
        private static readonly bool MODO_PRUEBA = true;

        private const string CARPETA = "~/App_Data/Descargas";
        private const string NOMBRE_ZIP = "Falke-instalacion.zip";

        public class Instalador
        {
            public string Archivo { get; set; }
            public string ClaveTitulo { get; set; }
            public string ClaveDescripcion { get; set; }
            public string Enlace { get; set; }
            public long Bytes { get; set; }
            public bool Disponible { get; set; }
        }

        private List<Instalador> instaladores;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SesionActual_GUI.ExigirPermiso(Patentes_TLL.OPERAR_ANALISIS)) return;

            instaladores = Armar();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (instaladores == null) return;

            List<Instalador> disponibles = Armar().Where(i => i.Disponible).ToList();

            btnTodo.Enabled = disponibles.Count > 0 || MODO_PRUEBA;
            litIncluye.Text = Server.HtmlEncode(string.Join(" · ", disponibles.Select(i => Traducir(i.ClaveTitulo))));

            if (disponibles.Count == 0) Avisar("aviso-alerta", Traducir("descargas_sin_instaladores"));
        }

        protected void btnTodo_Click(object sender, EventArgs e)
        {
            List<Instalador> disponibles = Armar().Where(i => i.Disponible).ToList();

            if (disponibles.Count == 0)
            {
                if (MODO_PRUEBA) descargaPasos.Attributes.Remove("hidden");

                return;
            }

            Intentar(() => EnviarZip(disponibles), "Descargas.Zip", mensaje => Avisar("aviso-peligro", mensaje));
        }

        private List<Instalador> Armar()
        {
            string carpeta = Server.MapPath(CARPETA);

            var lista = new List<Instalador>
            {
                new Instalador { Archivo = "TobiiExperience.exe", ClaveTitulo = "descargas_tobii_titulo", ClaveDescripcion = "descargas_tobii_descripcion", Enlace = "https://gaming.tobii.com/getstarted/" },
                new Instalador { Archivo = "AgenteMirada.zip", ClaveTitulo = "descargas_agente_titulo", ClaveDescripcion = "descargas_agente_descripcion" }
            };

            foreach (Instalador instalador in lista)
            {
                var info = new FileInfo(Path.Combine(carpeta, instalador.Archivo));

                instalador.Disponible = info.Exists;
                instalador.Bytes = info.Exists ? info.Length : 0;
            }

            return lista;
        }

        private void EnviarZip(List<Instalador> disponibles)
        {
            string carpeta = Server.MapPath(CARPETA);

            Response.Clear();
            Response.BufferOutput = false;
            Response.ContentType = "application/zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + NOMBRE_ZIP + "\"");

            using (var zip = new ZipArchive(Response.OutputStream, ZipArchiveMode.Create, true))
            {
                foreach (Instalador instalador in disponibles)
                {
                    ZipArchiveEntry entrada = zip.CreateEntry(instalador.Archivo, CompressionLevel.NoCompression);

                    using (Stream destino = entrada.Open())
                    using (FileStream origen = File.OpenRead(Path.Combine(carpeta, instalador.Archivo)))
                    {
                        origen.CopyTo(destino);
                    }
                }
            }

            Response.Flush();
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
