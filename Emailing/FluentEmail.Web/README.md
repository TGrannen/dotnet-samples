# FluentEmail

This project shows an example of using [FluentEmail](https://github.com/lukencode/FluentEmail). FluentEmail is an easy way to send email from .NET and .NET Core. using Razor for email templates and send via SendGrid, MailGun, SMTP and more.

Examples with the project:

* Simple email building
* SMTP Email delivery
* SendGrid Email delivery
* Razor email templating
* Dependency Injection setup
* Sample Configuration setup

## Testing

[PapercutSMTP](https://github.com/ChangemakerStudios/Papercut-SMTP) can be used to run a local test SMTP server to see the contents of emails sent via code without worrying about any emails being sent out to actual users.

To run inside of a Docker container, run the command below and then navigate to this address [`http://localhost:37408`](http://localhost:37408)

``` txt
docker run --name=papercut -p 25:25 -p 37408:37408 jijiechen/papercut:latest
```
