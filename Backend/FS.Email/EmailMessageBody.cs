using MimeKit;

namespace FS.Email;

public sealed record EmailMessageBody(string TextBody, string? HtmlBody = null)
{
    public MimeEntity ToMimeEntity()
    {
        if (string.IsNullOrWhiteSpace(HtmlBody))
        {
            return new TextPart("plain")
            {
                Text = TextBody
            };
        }

        return new BodyBuilder
        {
            TextBody = TextBody,
            HtmlBody = HtmlBody
        }.ToMessageBody();
    }
}
