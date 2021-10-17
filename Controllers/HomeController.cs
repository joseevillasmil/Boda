using Joseevillasmil.Boda.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System.Web;

namespace Joseevillasmil.Boda.Controllers
{
    public class HomeController : Controller
    {
        const string prefix = "bodajoseyyohe";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private string conString = "AQUI EL URL STRING.";
        private string _tablaInvitados = prefix + "invitados";
        private string _tablaAsientos = prefix + "asientos";
        public IActionResult Index(string familia)
        {
            var serviceClient = new TableServiceClient(conString);
            var tablaAsientos = serviceClient.GetTableClient(_tablaAsientos);
            var tablaInvitados = serviceClient.GetTableClient(_tablaInvitados);
            var Invitado = tablaInvitados.Query<Models.Invitado>(filter: $"PartitionKey eq '{prefix}' and RowKey eq '{familia}'")
                            .ToList<Models.Invitado>();

            if(Invitado.Count == 0)
            {
                ViewBag.Invitado = null;
                ViewBag.Sillas = null;
                ViewBag.InvitadoExiste = false;
            } else
            {
                var Sillas = tablaAsientos.Query<Models.Asiento>(filter: $"PartitionKey eq '{prefix}'").ToList<Models.Asiento>();

                // verificamos si hay algunos de nuestros asientos tomados.

                ViewBag.Invitado = Invitado[0];
                ViewBag.Sillas = Sillas;
                ViewBag.InvitadoExiste = true;
            }
            return View();
        }

        [HttpPost]
        public JsonResult Confirmar([FromBody] Models.RequestConfirmar data)
        {
            data.RowKey = HttpUtility.HtmlDecode(data.RowKey);
            var serviceClient = new TableServiceClient(conString);
            var tablaAsientos = serviceClient.GetTableClient(_tablaAsientos);
            var tablaInvitados = serviceClient.GetTableClient(_tablaInvitados);
            var BusquedaInvitado = tablaInvitados.Query<Models.Invitado>(filter: $"PartitionKey eq '{prefix}' and RowKey eq '{data.RowKey}'")
                            .ToList<Models.Invitado>();

            var BusquedaAsientos = tablaAsientos.Query<Models.Asiento>(filter: $"PartitionKey eq '{prefix}' ")
                            .ToList<Models.Asiento>();

            if (BusquedaInvitado.Count == 0)
            {
                throw new Exception("Invitado no válido");
            }

            if(!String.Equals(BusquedaInvitado[0].Estado.ToUpper(), "PENDIENTE"))
            {
                throw new Exception("Invitado no válido");
            }

            var invitado = BusquedaInvitado[0];
            invitado.Estado = "CONFIRMADO";

           

            // ACTUALIZAMOS LOS asientos.
            var asientos = data.Asientos.Split('|');
            List<Models.Asiento> aReservar = new List<Models.Asiento>();
            foreach(Models.Asiento asiento in BusquedaAsientos.Where(w => asientos.Contains(w.RowKey) ).ToList<Models.Asiento>())
            {
                // validamos el asiento.
                if(!String.Equals(asiento.Estado, "DISPONIBLE"))
                {
                    throw new Exception($"EL ASIENTO {asiento.RowKey} no esta disponible");
                }

                asiento.ReservadoPor = data.RowKey;
                asiento.Estado = "RESERVADO";
                aReservar.Add(asiento);
            }

            foreach(Models.Asiento asiento in aReservar)
            {
                tablaAsientos.UpdateEntity(asiento, asiento.ETag);
            }

            // SI TODO VA BIEN, actualizamos al usuario.
            tablaInvitados.UpdateEntity(invitado, invitado.ETag);

            return new JsonResult("OK");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
