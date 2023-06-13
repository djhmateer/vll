namespace VLL.Web
{
    public class Db
    {
    }
    public record AAEmail(
       string ToEmailAddress,
       string Subject,
       string TextBody,
       string HtmlBody
   );
}
