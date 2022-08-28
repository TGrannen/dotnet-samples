﻿using Moq;

namespace SnapshotTesting.Verify;

[Collection("Moq Collection")]
[UsesVerify]
public class MoqTests
{
    [Fact]
    public Task Test()
    {
        var mock = new Mock<ITarget>();

        mock.Setup(_ => _.Method(It.IsAny<int>(), It.IsAny<int>()))
            .Returns("response");

        var target = mock.Object;
        target.Method(1, 2);
        return Verifier.Verify(mock);
    }
}

public interface ITarget
{
    string Method(int a, int b);
}