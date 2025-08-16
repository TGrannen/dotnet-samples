# Streaming

## Kafka

[Apache Kafka](https://kafka.apache.org/) is an open-source distributed event streaming platform used by thousands of companies for high-performance data pipelines, streaming
analytics, data integration, and mission-critical applications.

## Sample projects

This solution includes sample projects to illustrate basic streaming scenarios:

- Producer: a simple API to publish messages/events to Kafka.
- Consumer: a background service to consume and process messages.

These samples focus on common patterns (producing, consuming, error handling) and can be adapted to different client libraries such as KafkaFlow or Silverback.

### KafkaFlow

[KafkaFlow](https://farfetch.github.io/kafkaflow/) was designed to be easy to use and to quickly develop applications that work with Apache Kafka.

#### Links

* [Aspire](http://localhost:18888/structuredlogs)
* [Producer](https://localhost:7149/scalar/v1)
* [Consumer](https://localhost:7244/kafkaflow/)

### Silverback

[Silverback](https://silverback-messaging.net/) is a .NET messaging framework that simplifies working with message brokers like Kafka. It provides straightforward configuration,
consumer/producer abstractions, and integrations with ASP.NET and dependency injection.

#### Links

* [Aspire](http://localhost:18888/structuredlogs)
* [Producer](https://localhost:7115/scalar/v1)