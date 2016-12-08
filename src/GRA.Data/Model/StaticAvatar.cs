using GRA.Data.Abstract;

namespace GRA.Data.Model
{
    public class StaticAvatar : BaseDbEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
