using System.Collections.Generic;

namespace MendauPrediction.Web.Models
{
    public class UploadDocumentViewModel
    {
        public List<Prediction> Predictions { get; set; }
        public string OriginalFilename { get; set; }
    }
}