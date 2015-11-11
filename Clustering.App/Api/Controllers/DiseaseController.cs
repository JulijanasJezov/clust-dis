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

        [Route("{diseaseId}")]
        public IHttpActionResult PatchDisease(int diseaseId, PostDiseaseApiModel updateDisease)
        {
            if (updateDisease == null)
            {
                return ApiBadRequest();
            }

            if (updateDisease.DiseaseName == null || updateDisease.Properties == null)
            {
                return ApiBadRequest();
            }

            var disease = Db.Diseases
                .Where(s => s.DiseaseId == diseaseId)
                .SingleOrDefault();

            if (disease == null)
            {
                return ApiNotFound();
            }

            disease.Name = updateDisease.DiseaseName;

            var existingPropertiesIds = updateDisease.Properties.Select(s => s.DiseasePropertyId);

            var propertiesToDelete = Db.DiseaseProperties
                .Where(s => s.DiseaseId == diseaseId)
                .Where(s => !existingPropertiesIds.Contains(s.DiseasePropertyId))
                .ToList();

            var propertiesToDeleteIds = propertiesToDelete.Select(s => s.DiseasePropertyId);

            var hasAssociations = Db.PersonDiseasePropertyAssociations
                .Where(s => propertiesToDeleteIds.Contains(s.DiseasePropertyId))
                .Any();

            if (hasAssociations)
            {
                return ApiBadRequest("Properties that have associations with people cannot be deleted");
            }

            Db.DiseaseProperties.RemoveRange(propertiesToDelete);
            Db.SaveChanges();

            var properties = new List<DiseaseProperty>();

            foreach (var property in updateDisease.Properties)
            {
                if (property.DiseasePropertyId == null)
                {
                    properties.Add(new DiseaseProperty
                    {
                        DiseaseId = disease.DiseaseId,
                        Name = property.Name
                    });
                }
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
        public IHttpActionResult GetDisease(int diseaseId)
        {
            var disease = Db.Diseases
                .Where(s => s.DiseaseId == diseaseId)
                .SingleOrDefault();

            if (disease == null)
            {
                return ApiNotFound();
            }

            var diseaseProperties = Db.DiseaseProperties
                .Where(s => s.DiseaseId == diseaseId)
                .Select(s => new {
                    DiseasePropertyId = s.DiseasePropertyId,
                    Name = s.Name,
                    CanDelete = !Db.PersonDiseasePropertyAssociations.Where(a => a.DiseasePropertyId == s.DiseasePropertyId).Any()
                });

            if (!diseaseProperties.Any())
            {
                return ApiNotFound();
            }

            return ApiOk(new
            {
                DiseaseName = disease.Name,
                DiseaseProperties = diseaseProperties
            });
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