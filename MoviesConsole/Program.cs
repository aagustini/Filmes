using Microsoft.EntityFrameworkCore;
using Persistencia.Entidades;
using Persistencia.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoviesConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            MovieContext _context = new MovieContext();

            Genre g1 = new Genre()
            {
                Name = "Comedia",
                Description = "Genero tipo diversao para familila"
            };
            Genre g2 = new Genre()
            {
                Name = "Ficcao",
                Description = "Filmes de ficcao cientifica"
            };

            _context.Genres.Add(g1);
            _context.Genres.Add(g2);

            Console.WriteLine("id de g1: " + g1.GenreId);

            _context.SaveChanges();

            Console.WriteLine("id de g1: " + g1.GenreId);

            g1.Description = "Alterado";

            _context.SaveChanges();

            List<Genre> genres = _context.Genres.ToList();
            foreach (Genre g in genres)
            {
                Console.WriteLine(String.Format("{0,2} {1,-10} {2}",
                     g.GenreId, g.Name, g.Description));
            }

            Movie m = new Movie()
            {
                Title = "Pulp Fiction",
                Genre = g2
            };

            _context.Add(m);


            g2.Description = "alterado em " + DateTime.Now.ToLongTimeString();

            _context.Update(g2);

            _context.SaveChanges();

            Console.WriteLine("Filme...");
            Console.WriteLine(String.Format("Titulo: {0} Genero: {1} {2}",
                  m.Title, m.Genre.Name, m.Genre.Description));

            Console.ReadLine();
            _context.Movies.Remove(m);
            _context.SaveChanges();


        }




        static void MainConsultas(String[] args)
        {
            MovieContext db = new MovieContext();

            #region # LINQ - consultas
            Console.WriteLine();
            Console.WriteLine("Todos os gêneros da base de dados:");
            foreach (Genre genero in db.Genres)
            {
                Console.WriteLine("{0} \t {1}", genero.GenreId, genero.Name);
            }

            //listar todos os filmes de acao
            Console.WriteLine();
            Console.WriteLine("Todos os filmes de acao:");

            List<Movie> filmes = new List<Movie>();
            foreach (Movie f in db.Movies)
            {
                if (f.GenreID == 1)
                    filmes.Add(f);
            }

            //var filmes = from f in db.Movies
            //             where f.GenreId == 1
            //             select f;


            //Console.WriteLine("Todos os filmes do genero 'Action':");
            //var filmesAction = db.Movies.Where(m => m.GenreId == 1);
            //foreach (Movie filme in filmesAction)
            //{
            //    Console.WriteLine("\t{0}", filme.Title);
            //}

            Console.WriteLine();
            Console.WriteLine("Todos os filmes do genero 'Action':");
            var filmesAction2 = from m in db.Movies
                                where m.GenreID == 1
                                select m;
            foreach (Movie filme in filmesAction2)
            {
                Console.WriteLine("\t{0}", filme.Title);
            }

            Console.WriteLine();
            Console.WriteLine("Todos os filmes de cada genero:");
            var generosFilmes = from g in db.Genres.Include(gen => gen.Movies)
                                select g;
            //var generosFilmes2 = db.Genres.Include(gen => gen.Movies).ToList();

            foreach (var gf in generosFilmes)
            {
                Console.WriteLine("Filmes do genero: " + gf.Name);
                foreach (var f in gf.Movies)
                {
                    Console.WriteLine("\t{0}", f.Title);
                }
            }



            Console.WriteLine();
            Console.WriteLine("Nomes dos filmes do diretor Quentin Tarantino:");
            var q1 = from f in db.Movies
                     where f.Director == "Quentin Tarantino"
                     select new
                     {
                         Ano = f.ReleaseDate.Year,
                         Titulo = f.Title
                     };

            var q2 = db.Movies.Where(f => f.Director == "Quentin Tarantino").Select(f => f.Title);

            foreach (var item in q1)
            {
                Console.WriteLine("{0} - {1}", item.Ano, item.Titulo);
            }


            Console.WriteLine();
            Console.WriteLine("Nomes e data dos filmes do diretor Quentin Tarantino:");
            var q3 = from f in db.Movies
                     where f.Director == "Quentin Tarantino"
                     select new { f.Title, f.ReleaseDate };
            foreach (var f in q3)
            {
                Console.WriteLine("{0}\t {1}", f.ReleaseDate.ToShortDateString(), f.Title);
            }

            Console.WriteLine();
            Console.WriteLine("Todos os gêneros ordenados pelo nome:");
            var q4 = db.Genres.OrderByDescending(g => g.Name);
            foreach (var genero in q4)
            {
                Console.WriteLine("{0, 20}\t {1}", genero.Name, genero.Description.Substring(0, 30));
            }
            Console.WriteLine();
            Console.WriteLine("Numero de filmes agrupados pelo anos de lançamento:");
            var q5 = from f in db.Movies
                     group f by f.ReleaseDate.Year into grupo
                     select new
                     {
                         Chave = grupo.Key,
                         NroFilmes = grupo.Count()
                     };

            foreach (var ano in q5.OrderByDescending(g => g.NroFilmes))
            {
                Console.WriteLine("Ano: {0}  Numero de filmes: {1}", ano.Chave, ano.NroFilmes);

            }
            Console.WriteLine("tecle algo para continuar");
            Console.ReadKey();

            Console.WriteLine();
            Console.WriteLine("Projeção do faturamento total, quantidade de filmes e avaliação média agrupadas por gênero:");
            var q6 = from f in db.Movies
                     group f by f.Genre.Name into grpGen
                     select new
                     {
                         Categoria = grpGen.Key,
                         Faturamento = grpGen.Sum(e => e.Gross),
                         Avaliacao = grpGen.Average(e => e.Rating),
                         Quantidade = grpGen.Count()
                     };

            foreach (var genero in q6)
            {
                Console.WriteLine("Genero: {0}", genero.Categoria);
                Console.WriteLine("\tFaturamento total: {0}\n\t Avaliação média: {1}\n\tNumero de filmes: {2}",
                                genero.Faturamento, genero.Avaliacao, genero.Quantidade);
            }
            #endregion



        }

    }
}
