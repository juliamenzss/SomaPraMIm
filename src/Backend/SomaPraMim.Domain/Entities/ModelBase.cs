namespace SomaPraMim.Domain.Entities
{
    public class ModelBase
    {
        public bool Active { get; set; }  = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}