namespace SomaPraMim.Domain.Entities
{
    public class ModelBase
    {
        public long Id { get; set; }
        
        public bool Active { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}