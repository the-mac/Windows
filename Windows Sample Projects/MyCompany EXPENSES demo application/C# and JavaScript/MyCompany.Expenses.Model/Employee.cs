﻿namespace MyCompany.Expenses.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Employee entity
    /// </summary>
    public class Employee 
    {
        /// <summary>
        /// Employee constructor.
        /// </summary>
        public Employee() {
            ManagedTeams = new List<Team>();
        }

        /// <summary>
        /// UniqueId
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Job Title
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// TeamId
        /// </summary>
        public int? TeamId { get; set; }

        /// <summary>
        /// Team
        /// </summary>
        public Team Team { get; set; }

        /// <summary>
        /// ManagedT eams
        /// </summary>
        public ICollection<Team> ManagedTeams { get; set; }

        /// <summary>
        /// Expenses
        /// </summary>
        public ICollection<Expense> Expenses { get; set; }

        /// <summary>
        /// Employee Pictures
        /// </summary>
        public ICollection<EmployeePicture> EmployeePictures { get; set; }

    }
}
