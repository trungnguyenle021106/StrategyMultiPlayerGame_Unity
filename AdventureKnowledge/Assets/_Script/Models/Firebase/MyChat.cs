public class MyChat
{
    public string Sender { get; set; }
    public string Message { get; set; }
    public MyChat(string Sender, string Message)
    {
        this.Sender = Sender;
        this.Message = Message;
    }
}