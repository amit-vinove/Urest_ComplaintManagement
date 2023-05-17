using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UrestComplaintManagement.Controllers
{
    public class TicketController : Controller
    {
        // GET: Ticket
        public ActionResult Tickets()
        {
            return View();
        }
    }
}