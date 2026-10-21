using CompanySalesAgent.Api.Models;
using Microsoft.EntityFrameworkCore;
using CompanySalesAgent.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanySalesAgent.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Lead> Leads { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
    }
}