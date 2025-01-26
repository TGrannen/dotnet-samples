# Messaging

This set of projects shows examples of how to use [MassTransit](https://masstransit-project.com/) for distributed messaging between dotnet applications.

MassTransit is a free, open-source distributed application framework for .NET. MassTransit makes it easy to create applications and services that leverage message-based, loosely-coupled asynchronous communication for higher availability, reliability, and scalability.

## Example Services

### Amazon Web Services

Amazon [Simple Queue Service(SQS)](https://aws.amazon.com/sqs/) & [Simple Notification Service(SNS)](https://aws.amazon.com/sns/) services are used within the example projects as a mechanism to deliver messages to awaiting consumers. A producer will send a message to an SNS topic and then that message will get delivered to the configured subscribers. See image below.

![SNS/SQS Architecture](https://miro.medium.com/max/500/1*mdUPKzrfJFuXa4d43KhKUQ.png)

[^ Image Source](https://medium.com/awesome-cloud/aws-difference-between-sqs-and-sns-61a397bf76c5)

## Packages

* [MassTransit](https://masstransit-project.com/) - [GitHub](https://github.com/MassTransit/MassTransit)
* [MassTransit.AmazonSQS](https://www.nuget.org/packages/MassTransit.AmazonSQS)