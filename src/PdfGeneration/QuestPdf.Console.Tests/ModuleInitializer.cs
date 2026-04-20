using System.Runtime.CompilerServices;
using QuestPDF.Infrastructure;

namespace QuestPdf.Console.Tests;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        VerifyImageMagick.RegisterComparers(0.015);
        VerifyQuestPdf.Initialize();
    }
}
