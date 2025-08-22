namespace backend_sk_chat_tcs
{
    public record class Message
    {
        public string Id { get; set; }
        public string MessageText {  get; set; }
        public string Image { get; set; }

        public string Author { get; set; }
    }
}
