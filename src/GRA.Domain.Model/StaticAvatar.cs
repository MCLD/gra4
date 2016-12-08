using GRA.Domain.Model.Abstract;

namespace GRA.Domain.Model
{
    public class StaticAvatar : BaseDomainEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
