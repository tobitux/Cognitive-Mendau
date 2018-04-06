using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MendauPrediction.Web.Models
{
    public class Prediction
    {
        public string Projectname { get; set; }
        public string Tag { get; set; }
        public double Probability { get; set; }
        public string File { get; set; }
    }
}