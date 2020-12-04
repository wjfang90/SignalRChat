using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using Chat.Business;
using Chat.SignalRChat.Models;
namespace Chat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBusiness _dataBusiness;
        public HomeController(ILogger<HomeController> logger,DataBusiness dataBusiness)
        {
            _logger = logger;
            _dataBusiness = dataBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var groups =  await _dataBusiness.InitGroups();
            return View(groups);
        }


        public IActionResult GetGroups()
        {
            return Json(_dataBusiness.GetGroups());
        }


        public IActionResult Privacy()
        {
            var msg = new MessageInfo() { Message = "aaa", Sender = "bbb" };
            ViewBag.MessageInfo = msg;
            ViewBag.MessageInfoJson = Json(msg, new System.Text.Json.JsonSerializerOptions());

            return View(Json(msg, new System.Text.Json.JsonSerializerOptions()));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
