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
        /// <summary>
        /// Returns a list of of people
        /// </summary>
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
                var filter = filterQuery.Trim().ToLower();
                people = people.Where(s => (s.FirstName + " " + s.LastName).ToLower().StartsWith(filter) ||
                    (s.LastName + " " + s.FirstName).ToLower().StartsWith(filter));
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

        /// <summary>
        /// Creates a new person
        /// </summary>
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

            if (person.DiseaseProperties != null)
            {
                foreach (var diseaseProperty in person.DiseaseProperties)
                {
                    Db.PersonDiseasePropertyAssociations.Add(new PersonDiseasePropertyAssociation
                    {
                        DiseasePropertyId = diseaseProperty.DiseasePropertyId,
                        Score = diseaseProperty.Score,
                        PersonId = personToAdd.PersonId
                    });
                }
            }

            Db.SaveChanges();

            return ApiOk();
        }

        /// <summary>
        /// Updates an existing person
        /// </summary>
        [Route("{personId}")]
        public IHttpActionResult PatchPerson(int personId, PersonApiModel updatePerson)
        {
            if (updatePerson == null)
            {
                return ApiBadRequest();
            }

            if (updatePerson.FirstName == null || updatePerson.LastName == null || updatePerson.DateOfBirth == null)
            {
                return ApiBadRequest("Required fields missing");
            }

            var person = Db.People.Where(s => s.PersonId == personId).SingleOrDefault();

            if (person == null)
            {
                return ApiNotFound();
            }

            person.Title = updatePerson.Title;
            person.FirstName = updatePerson.FirstName;
            person.LastName = updatePerson.LastName;
            person.DateOfBirth = updatePerson.DateOfBirth.Value;
            person.Gender = (GenderType)Enum.Parse(typeof(GenderType), updatePerson.Gender);
            
            Db.SaveChanges();

            if (updatePerson.RemoveDiseases != null)
            {
                foreach (var diseaseId in updatePerson.RemoveDiseases)
                {
                    var diseaseProperties = Db.PersonDiseasePropertyAssociations
                        .Where(s => s.PersonId == personId)
                        .Where(s => s.DiseaseProperty.DiseaseId == diseaseId);

                    Db.PersonDiseasePropertyAssociations.RemoveRange(diseaseProperties);
                    Db.SaveChanges();
                }
            }

            if (updatePerson.DiseaseProperties != null)
            {
                foreach (var diseaseProperty in updatePerson.DiseaseProperties)
                {
                    Db.PersonDiseasePropertyAssociations.Add(new PersonDiseasePropertyAssociation
                    {
                        DiseasePropertyId = diseaseProperty.DiseasePropertyId,
                        Score = diseaseProperty.Score,
                        PersonId = personId
                    });
                }
            }

            Db.SaveChanges();

            return ApiOk();
        }

        /// <summary>
        /// Returns a single person
        /// </summary>
        [Route("{personId}")]
        public IHttpActionResult GetPerson(int personId)
        {
            var person = Db.People.Where(s => s.PersonId == personId)
                .SingleOrDefault();

            if (person == null)
            {
                return ApiNotFound();
            }

            var diseases = Db.PersonDiseasePropertyAssociations
                .Where(s => s.PersonId == personId)
                .Select(s => s.DiseaseProperty.Disease)
                .Distinct();


            return ApiOk(new
            {
                Title = person.Title,
                FirstName = person.FirstName,
                LastName = person.LastName,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender.ToString(),
                Diseases = diseases
            });
        }
    }
}
