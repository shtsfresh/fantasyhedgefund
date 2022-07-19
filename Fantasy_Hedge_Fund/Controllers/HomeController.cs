﻿using Fantasy_Hedge_Fund.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BLL.Interfaces;

namespace Fantasy_Hedge_Fund.Controllers
{
    public class HomeController : Controller
    {
        private IUtilitiesService _utilitiesService;

        public HomeController(IUtilitiesService utilitiesService)
        {
            _utilitiesService = utilitiesService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public async Task<IActionResult> Leaderboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}