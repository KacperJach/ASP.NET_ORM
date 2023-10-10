using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.IO;


namespace RESTful_API
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        public const string DbFile = "DataBase.db";
        private readonly ISessionFactory _sessionFactory = Program.CreateSessionFactory();

        public PeopleController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        
        
        // GET: api/People
        [HttpGet]
        public ActionResult<IEnumerable<Person>> Get()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var people = session.Query<Person>().ToList();
                return Ok(people);
            }
        }
        // GET: api/People/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult<Person> Get(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var person = session.Get<Person>(id);
                if (person == null)
                {
                    return NotFound();
                }
                return Ok(person);
            }
        }

        // POST: api/People
        [HttpPost]
        public ActionResult<Person> Post([FromBody] Person person)
        {
            try
            {
                using (var session = _sessionFactory.OpenSession())
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(person);
                    transaction.Commit();
                    return CreatedAtAction(nameof(Get), new { id = person.Id }, person);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // PUT: api/People/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Person person)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var existingPerson = session.Get<Person>(id);
                if (existingPerson == null)
                {
                    return NotFound();
                }

                existingPerson.Name = person.Name;
                existingPerson.Age = person.Age;
                session.Update(existingPerson);
                transaction.Commit();

                return NoContent();
            }
        }

        // DELETE: api/People/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var person = session.Get<Person>(id);
                if (person == null)
                {
                    return NotFound();
                }

                session.Delete(person);
                transaction.Commit();
                return NoContent();
            }
        }
    }
}
