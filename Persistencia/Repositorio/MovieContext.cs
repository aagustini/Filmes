using Microsoft.EntityFrameworkCore;
using Persistencia.Entidades;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.Repositorio
{
    public class MovieContext : DbContext
    {
        public MovieContext() : base()
        {
        }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
           .UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=MoviesDB;Trusted_Connection=True;");

            //optionsBuilder.LogTo(message => Console.WriteLine(message));
        }
    }
}
