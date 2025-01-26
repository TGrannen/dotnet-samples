namespace KafkaFlowSample.MessageContracts;

public record BadDataMessage(bool ThrowException, DateTime Time);