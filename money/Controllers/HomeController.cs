﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace money.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() =>
            View();
    }
}
