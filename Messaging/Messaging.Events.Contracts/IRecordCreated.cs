using System;

namespace Messaging.Events.Contracts;

public interface IRecordCreated
{
    DateTime CreatedAt { get; set; }
}