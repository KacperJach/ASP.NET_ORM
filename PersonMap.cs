using FluentNHibernate.Mapping;

namespace RESTful_API
{

    public class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Table("People");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.Age);
        }
    }
}
