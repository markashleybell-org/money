﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace money.web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}