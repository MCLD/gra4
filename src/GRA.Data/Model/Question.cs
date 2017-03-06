using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Question : Abstract.BaseDbEntity
    {
        [Required]
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
        public int SortOrder { get; set; }

        [MaxLength(1500)]
        public string Text { get; set; }
        public int CorrectAnswerId { get; set; }

        public ICollection<Answer> Answers;
    }
}
