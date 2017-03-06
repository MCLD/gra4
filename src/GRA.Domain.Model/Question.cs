using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Question : Abstract.BaseDomainEntity
    {
        public int QuestionnaireId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(1500)]
        public string Text { get; set; }
        public int CorrectAnswerId { get; set; }

        public ICollection<Answer> Answers;
    }
}
