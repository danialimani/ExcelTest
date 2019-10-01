using System;
using System.Collections.Generic;
using System.Text;
using ExcelExam.Data.DatabaseModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExcelExam.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<City> Cities { get; set; }
        public DbSet<Sale> Sales { get; set; }
    }
}
