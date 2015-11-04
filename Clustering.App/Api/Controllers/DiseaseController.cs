using Clustering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/diseases")]
    public class DiseaseController : BaseController
    {
        [Route("")]
        public IHttpActionResult GetDiseases()
        {
            var diseases = Db.Diseases
                .Select(s => new
                {
                    DiseaseId = s.DiseaseId,
                    Name = s.Name,
                    CanDelete = !Db.People.Where(a => a.PersonDiseaseProperties.Where(b => b.DiseaseProperty.DiseaseId == s.DiseaseId).Any()).Any()
                })
                .ToList();

            return ApiOk(diseases);
        }

        [Route("")]
        public IHttpActionResult PostDisease(PostDiseaseApiModel newDisease)
        {
            if (newDisease == null)
            {
                return ApiBadRequest();
            }

            if (newDisease.DiseaseName == null || newDisease.Properties == null)
            {
                return ApiBadRequest();
            }

            var disease = new Disease
            {
                Name = newDisease.DiseaseName
            };

            Db.Diseases.Add(disease);
            Db.SaveChanges();

            var properties = new List<DiseaseProperty>();

            foreach (var property in newDisease.Properties)
            {
                properties.Add(new DiseaseProperty
                {
                    DiseaseId = disease.DiseaseId,
                    Name = property.Name
                });
            }

            Db.DiseaseProperties.AddRange(properties);
            Db.SaveChanges();

            return ApiOk();
        }

        [Route("{diseaseId}/properties")]
        public IHttpActionResult GetDiseaseProperties(int diseaseId)
        {
            var diseaseProperties = Db.DiseaseProperties
                .Where(s => s.DiseaseId == diseaseId);

            if (!diseaseProperties.Any())
            {
                return ApiNotFound();
            }

            return ApiOk(diseaseProperties);
        }

        [Route("{diseaseId}")]
        public IHttpActionResult DeleteDisease(int diseaseId)
        {
            var disease = Db.Diseases
                .Where(s => s.DiseaseId == diseaseId)
                .SingleOrDefault();

            if (disease == null)
            {
                return ApiBadRequest();
            }

            var peopleExist = Db.People
                .Where(s => s.PersonDiseaseProperties
                    .Where(a => a.DiseaseProperty.DiseaseId == diseaseId)
                    .Any())
                .Any();

            if (peopleExist)
            {
                return ApiBadRequest();
            }

            var properties = Db.DiseaseProperties
                .Where(s => s.DiseaseId == diseaseId);

            Db.DiseaseProperties.RemoveRange(properties);
            Db.SaveChanges();

            Db.Diseases.Remove(disease);
            Db.SaveChanges();

            return ApiOk();
        }
    }
}