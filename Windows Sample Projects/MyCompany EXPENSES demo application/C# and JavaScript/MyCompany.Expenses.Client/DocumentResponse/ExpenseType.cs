﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.Expenses.Client
{
    /// <summary>
    /// Expense Type
    /// </summary>
    public enum ExpenseType 
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Travel
        /// </summary>
        Travel = 1,
        /// <summary>
        /// Food
        /// </summary>
        Food = 2,
        /// <summary>
        /// Accommodation
        /// </summary>
        Accommodation = 4,
        /// <summary>
        /// Other
        /// </summary>
        Other = 8
    }
}
