using System.ComponentModel.DataAnnotations;

namespace SymptomsAppMVC.Models
{
    public class Symptom
    {     
        public string? Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public PainTypes PainType { get; set; }

     
        public int SeverityScale { get; set; }

       
        public decimal? SymptomDurationHours { get; set; }

        public int OccurrenceCounter { get; set; }


        public enum PainTypes
        {
            Headache,
            Stomachache,
            BackPain,
            AbdominalPain,
            ChestPain,
            MusclePain
        }
    }
}
