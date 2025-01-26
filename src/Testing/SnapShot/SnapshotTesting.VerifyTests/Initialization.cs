using System.Runtime.CompilerServices;

namespace SnapshotTesting.VerifyTests;

public static class Initialization
{
    [ModuleInitializer]
    public static void EnableVerifyExtensions()
    {
        VerifyMicrosoftLogging.Enable();
        VerifyHttp.Enable();
        VerifyMoq.Enable();
        VerifyEntityFramework.Enable();
    }
}