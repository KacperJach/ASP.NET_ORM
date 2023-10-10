using Microsoft.AspNetCore.Mvc;
using NHibernate;



namespace RESTful_API
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        public const string DbFile = "DataBase.db";
        private readonly ISessionFactory _sessionFactory;

        public PeopleController(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }
        
        
        // GET: 
        [HttpGet]
        public ActionResult<IEnumerable<Person>> Get()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var people = session.Query<Person>().ToList();
                // var result = people.Where(x => x.Age >= 10);  //Wykorzystanie LINQ Where
                var result = people.Skip(3).Take(2);            //Wykorzysanie LINQ Skip i Take
                return Ok(result);
            }
        }
        // GET:
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

        // POST: 
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

        // PUT: 
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

        // DELETE: 
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
