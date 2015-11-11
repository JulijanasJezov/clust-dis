using Clustering.App.Api.Models;
using Clustering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/people")]
    public class PeopleController : BaseController
    {
        [Route("")]
        public IHttpActionResult GetPeople(
            int pageNumber = 1,
            int pageSize = 15,
            string orderBy = null,
            string direction = null,
            string filterQuery = null)
        {
            var people = Db.People.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                filterQuery = filterQuery.Trim().ToLower();
                people = people.Where(s => (s.FirstName + " " + s.LastName).ToLower().StartsWith(filterQuery) ||
                    (s.LastName + " " + s.FirstName).ToLower().StartsWith(filterQuery));
            }

            people = people.OrderBy(s => s.LastName).ThenBy(s => s.FirstName);

            var pagedModel = new PagedListModel<PersonApiModel>
            {
                Total = people.Count(),
                PageNumber = pageNumber,
                ItemsPerPage = pageSize,
                FilterQuery = filterQuery
            };

            pagedModel.Results = people
                .Skip(pagedModel.StartIndex)
                .Take(pagedModel.ItemsPerPage)
                .ToList()
                .Select(s => new PersonApiModel
                {
                    PersonId = s.PersonId,
                    Title = s.Title,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender.ToString()
                });

            return ApiOk(pagedModel);
        }

        [Route("")]
        public IHttpActionResult PostPerson(PersonApiModel person)
        {
            if (person == null)
            {
                return ApiBadRequest();
            }

            if (person.FirstName == null || person.LastName == null || person.DateOfBirth == null)
            {
                return ApiBadRequest("Required fields missing");
            }

            var personToAdd = new Person
            {
                Title = person.Title,
                FirstName = person.FirstName,
                LastName = person.LastName,
                DateOfBirth = person.DateOfBirth.Value,
                Gender = (GenderType)Enum.Parse(typeof(GenderType), person.Gender)
            };

            Db.People.Add(personToAdd);
            Db.SaveChanges();

            foreach (var diseaseProperty in person.DiseaseProperties)
            {
                Db.PersonDiseasePropertyAssociations.Add(new PersonDiseasePropertyAssociation
                {
                    DiseasePropertyId = diseaseProperty.DiseasePropertyId,
                    Score = diseaseProperty.Score,
                    PersonId = personToAdd.PersonId
                });
            }

            Db.SaveChanges();

            return ApiOk();
        }
    }
}
