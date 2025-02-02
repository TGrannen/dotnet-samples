namespace FluentEmail.Web.Configuration;

public class EmailConfig
{
    public string FromAddress { get; set; }
    public SendGrid SendGrid { get; set; }
    public SMTPConfig SMTPConfig { get; set; }
}

public class SendGrid
{
    public bool UseSandbox { get; set; }
    public string APIKey { get; set; }
}

public class SMTPConfig
{
    public int Port { get; set; }
    public string Server { get; set; }
}