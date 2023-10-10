using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;


namespace RESTful_API
{

    public class Program
    {
        public const string DbFile = "DataBase.db";

        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            ISessionFactory sessionFactory = CreateSessionFactory(); // Utworzenie sesji Bazy Danych
            builder.Services.AddSingleton(sessionFactory); //Dependency Injection
            builder.Services.AddControllers(); //Dodanie kontrolerow do mapowania

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.MapControllers();

            app.Run();

           


        }
        public static void BuildSchema(Configuration config)
        {
            // create new DB File if not existing yet
            if (!File.Exists(DbFile))
                new SchemaExport(config).Create(false, true);
        }
        public static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
              .Database(SQLiteConfiguration.Standard.UsingFile(DbFile))
              .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<Program>())
              .ExposeConfiguration(BuildSchema)
              .BuildSessionFactory();
        }
        static void AddPerson(ISessionFactory sessionFactory, Person p)
        {
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Save(p);
                transaction.Commit();
                Console.WriteLine("Dodano osobê do bazy danych.");
            }
        }

        static void ReadPeople(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var people = session.QueryOver<Person>().List();
                Console.WriteLine("Lista osób w bazie danych:");
                foreach (var person in people)
                {
                    Console.WriteLine($"ID: {person.Id}, Name: {person.Name}, Age: {person.Age}");
                }
            }
        }

        static void UpdatePerson(ISessionFactory sessionFactory, int id)
        {
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var person = session.Get<Person>(id);
                if (person != null)
                {
                    person.Age = 31;
                    session.Update(person);
                    transaction.Commit();
                    Console.WriteLine("Zaktualizowano dane osoby.");
                }
            }
        }
        static void DeletePerson(ISessionFactory sessionFactory, int id)
        {
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var person = session.Get<Person>(id);
                if (person != null)
                {
                    session.Delete(person);
                    transaction.Commit();
                    Console.WriteLine("Usuniêto osobê z bazy danych.");
                }
            }
        }
        

    }
}
