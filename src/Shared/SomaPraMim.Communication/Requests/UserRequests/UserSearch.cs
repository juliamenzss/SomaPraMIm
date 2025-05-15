namespace SomaPraMim.Communication.Requests.UserRequests
{
    public class UserSearch
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string? Term { get; set; }
    }
}
