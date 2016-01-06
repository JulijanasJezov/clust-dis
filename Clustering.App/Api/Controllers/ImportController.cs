using Clustering.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Clustering.App.Api.Controllers
{
    [RoutePrefix("api/import")]
    public class ImportController : BaseController
    {
        [Route("")]
        public async Task<IHttpActionResult> PostImport()
        {
            var csvType = new MediaTypeHeaderValue("text/csv");
            var excelType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            var excelType2 = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            var provider = await Request.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider());
            string disease = null;

            var newDisease = Db.Diseases.Create();

            foreach (var item in provider.Contents)
            {
                if (item.Headers.ContentDisposition.DispositionType == "form-data" && item.Headers.ContentDisposition.Name == "\"diseaseName\"")
                {
                    disease = await item.ReadAsStringAsync();

                    if (disease == null)
                    {
                        return ApiBadRequest("disease is required");
                    }
                    else
                    {
                        if (Db.Diseases.Where(s => s.Name == disease).Any())
                        {
                            return ApiBadRequest("Disease already exists");
                        }
                    }

                    newDisease.Name = disease;

                    Db.Diseases.Add(newDisease);

                    Db.SaveChanges();

                    continue;
                }

                if (!item.Headers.ContentType.Equals(csvType) && !item.Headers.ContentType.Equals(excelType) && !item.Headers.ContentType.Equals(excelType2))
                {
                    return ApiBadRequest("File format is not supported");
                }

                var data = await item.ReadAsStreamAsync();

                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(data, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    string text;

                    var properties = new Dictionary<int, string>();

                    var people = Db.People.ToArray();

                    var propertyAssociations = new List<PersonDiseasePropertyAssociation>();

                    int rowCount = 0;
                    foreach (Row r in sheetData.Elements<Row>())
                    {
                        int cellPos = 0;
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            cellPos++;
                            text = null;

                            var value = c.InnerText;

                            if (c.DataType != null)
                            {
                                switch (c.DataType.Value)
                                {
                                    case CellValues.SharedString:
                                        var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                        if (stringTable != null)
                                        {
                                            text =
                                                stringTable.SharedStringTable
                                                .ElementAt(int.Parse(value)).InnerText;
                                        }

                                        if (text != null && r.RowIndex == 1 && !Db.DiseaseProperties.Where(s => s.Name == text).Any())
                                        {
                                            var diseaseProperty = Db.DiseaseProperties.Create();

                                            diseaseProperty.DiseaseId = newDisease.DiseaseId;
                                            diseaseProperty.Name = text;

                                            Db.DiseaseProperties.Add(diseaseProperty);
                                            Db.SaveChanges();
                                            properties.Add(cellPos, text);
                                        }
                                        else
                                        {
                                            text = null;
                                        }
                                        break;
                                }
                            } 
                            else if (c.CellValue != null)
                            {
                                var propertyName = properties[cellPos];
                                var diseasePropertyId = Db.DiseaseProperties.Where(s => s.Name == propertyName).Select(s => s.DiseasePropertyId).SingleOrDefault();

                                double scoreDouble = scoreDouble = double.Parse(c.CellValue.Text);

                                int score = (int)Math.Round(scoreDouble, MidpointRounding.AwayFromZero);

                                propertyAssociations.Add(new PersonDiseasePropertyAssociation {
                                    DiseasePropertyId = diseasePropertyId,
                                    PersonId = people[rowCount].PersonId,
                                    Score = score
                                });

                            }
                        }

                        rowCount++;
                    }

                    Db.PersonDiseasePropertyAssociations.AddRange(propertyAssociations);
                    Db.SaveChanges();
                }
            }

            return ApiOk();
        }
    }
}