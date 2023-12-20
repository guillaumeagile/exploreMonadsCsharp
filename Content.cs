namespace Sample.Core.Models
{
    public class Content
    {

        public string Uid { get; set; } = Guid.NewGuid().ToString();

        //  a pattern to express a Content that is Void (null whithout NPE)
        public static Content Void()
        {
            return new Content() { Uid = Guid.Empty.ToString() };           
        }
    }
}
