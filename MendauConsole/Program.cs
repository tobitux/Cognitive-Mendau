using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Cognitive.CustomVision.Training;

namespace MendauConsole
{
    static class Program
    {
        static void Main()
        {
            var trainingKey = ConfigurationManager.AppSettings["TrainingKey"];
            var projectName = ConfigurationManager.AppSettings["ProjectName"]; 
            var imageFolder = ConfigurationManager.AppSettings["ImageFolder"];
     
            var trainingApi = new TrainingApi() {ApiKey = trainingKey };

            var projects = trainingApi.GetProjects();
            var project = projects.SingleOrDefault(x => x.Name == projectName) ?? trainingApi.CreateProject(projectName);

            var files = Directory.GetFiles(imageFolder);

            var tags = trainingApi.GetTags(project.Id);

            foreach (var file in files)
            {
                var tagName = Path.GetFileNameWithoutExtension(file);
                tagName = tagName.Substring(tagName.LastIndexOf("IMG", StringComparison.Ordinal) + 4, 4);

                try
                {
                    var tag = tags.Tags.FirstOrDefault(x => x.Name == tagName);
                    if (tag == null)
                    {
                        Console.WriteLine($"Create Tag {tagName}");
                        tag = trainingApi.CreateTag(project.Id, tagName);
                        tags.Tags.Add(tag);
                        Console.WriteLine($"Tag {tag.Name} created");
                    }

                    using (var stream = new MemoryStream(File.ReadAllBytes(file)))
                    {
                        Console.WriteLine($"Start File Upload {file}");
                        trainingApi.CreateImagesFromData(project.Id, stream, new List<string>() { tag.Id.ToString() });
                        Console.WriteLine($"Upload Complete.");
                    }
                }
                catch (Microsoft.Rest.HttpOperationException ex)
                {
                    Console.WriteLine(ex.Response.Content);
                }
            }
        
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }
}
