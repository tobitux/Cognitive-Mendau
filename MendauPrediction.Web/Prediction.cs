using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Prediction.Models;
using Microsoft.Cognitive.CustomVision.Training;
using Microsoft.Rest;

namespace MendauPrediction.Web
{
    public class Prediction
    {
        public List<Models.Prediction> Predict(string file)
        {
            var trainingKey = ConfigurationManager.AppSettings["TrainingKey"];
            var predictionKey = ConfigurationManager.AppSettings["PredictionKey"];

            var trainingApi = new TrainingApi { ApiKey = trainingKey };

            var projects = trainingApi.GetProjects();

            var predictions = new List<ImagePredictionResultModel>();
            var endpoint = new PredictionEndpoint { ApiKey = predictionKey };


            foreach (var project in projects.Where(x => x.Name.StartsWith("mendau")))
            {
                Console.WriteLine($"Making a prediction in project {project.Name}");
                try
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(file))) {
                        predictions.Add(endpoint.PredictImageWithNoStore(project.Id, stream));
                    }
                }
                catch (HttpOperationException ex)
                {
                    Console.WriteLine(ex.Response.Content);
                }
            }

            var result = new List<Models.Prediction>();

            var files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Images/original"));

            foreach (var prediction in predictions)
            {
                var project = projects.SingleOrDefault(x => x.Id == prediction.Project);
                var i = 0;
                foreach (var c in prediction.Predictions)
                {
                    //Console.WriteLine($"\t{project.Name} {c.Tag}: {c.Probability:P1}");

                    result.Add(new Models.Prediction()
                    {
                        Probability = c.Probability,
                        Projectname = project.Name,
                        Tag = c.Tag,
                        File = "~/Images/original/" + Path.GetFileName(files.FirstOrDefault(x => x.Contains(c.Tag)))
                    });

                    if (++i > 3) break;
                }
            }
            return result.OrderByDescending(x => x.Probability).ToList();
        }
    }
}