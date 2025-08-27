namespace backend_sk_chat_tcs.Models
{
    public record class Message
    {
        public string Id { get; set; }
        public string MessageT {  get; set; }
        public IFormFile? Image { get; set; }

        public string Author { get; set; }
    }
}
