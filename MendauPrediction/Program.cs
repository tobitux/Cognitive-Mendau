using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Prediction.Models;
using Microsoft.Cognitive.CustomVision.Training;
using Microsoft.Cognitive.CustomVision.Training.Models;
using Microsoft.Rest;

namespace MendauPrediction
{
    internal class Program
    {
        private static IList<Project> _projects;
        private static string _predictionKey;

        private static void Main()
        {
            var trainingKey = ConfigurationManager.AppSettings["TrainingKey"];
            var hotfolder = ConfigurationManager.AppSettings["Hotfolder"];

            _predictionKey = ConfigurationManager.AppSettings["PredictionKey"];

            var trainingApi = new TrainingApi {ApiKey = trainingKey};

            _projects = trainingApi.GetProjects();

            var watcher = new FileSystemWatcher
            {
                Path = hotfolder,
                Filter = "*.jpg"
            };

            watcher.Created += File_Created;
            watcher.EnableRaisingEvents = true;

            Console.ReadKey();
        }

        private static void File_Created(object sender, FileSystemEventArgs e)
        {
            var file = e.FullPath;

            var predictions = new List<ImagePredictionResultModel>();
            var endpoint = new PredictionEndpoint {ApiKey = _predictionKey};


            foreach (var project in _projects.Where(x => x.Name.StartsWith("mendau")))
            {
                Console.WriteLine($"Making a prediction in project {project.Name}");
                try
                {
                    using (var stream = new MemoryStream(File.ReadAllBytes(file)))
                    {
                        predictions.Add(endpoint.PredictImageWithNoStore(project.Id, stream));
                    }
                }
                catch (HttpOperationException ex)
                {
                    Console.WriteLine(ex.Response.Content);
                }
            }

            File.Delete(file);

            var results = new Dictionary<string, string>();
            foreach (var prediction in predictions)
            {
                var project = _projects.SingleOrDefault(x => x.Id == prediction.Project);
                var i = 0;
                foreach (var c in prediction.Predictions)
                {
                    Console.WriteLine($"\t{project.Name} {c.Tag}: {c.Probability:P1}");
                    if(++i > 2) break;
                }
            }
        }
    }
}