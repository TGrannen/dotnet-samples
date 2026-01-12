using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Stateless.Graph;

namespace StateMachines.Stateless.ExampleAPI.Services;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class StateMachineExporter
{
    public static string ExportToJson<T, T2>(StateMachine<T, T2> stateMachine)
    {
        return UmlDotGraph.Format(stateMachine.GetInfo());
    }

    public byte[] ExportToImage<T, T2>(StateMachine<T, T2> stateMachine)
    {
        var image = CreateStateMachineImage(ExportToJson(stateMachine));
        return ImageToByteArray(image);
    }

    public string ExportToSvg<T, T2>(StateMachine<T, T2> stateMachine)
    {
        return CreateStateMachineSvgImage(ExportToJson(stateMachine));
    }

    private Image CreateStateMachineImage(string stateMachineJson)
    {
        var processStartInfo = new ProcessStartInfo
        {
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = "docker",
            Arguments = "run --rm -i nshine/dot",
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        var process = Process.Start(processStartInfo);
        process!.StandardInput.Write(stateMachineJson);
        process.StandardInput.Close();
        var image = Image.FromStream(process.StandardOutput.BaseStream);
        return image;
    }

    private string CreateStateMachineSvgImage(string stateMachineJson)
    {
        var processStartInfo = new ProcessStartInfo
        {
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = "docker",
            Arguments = "run --rm -i nshine/dot dot -Tsvg",
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        var process = Process.Start(processStartInfo);
        process!.StandardInput.Write(stateMachineJson);
        process.StandardInput.Close();
        return process.StandardOutput.ReadToEnd();
    }

    private byte[] ImageToByteArray(Image imageIn)
    {
        using var ms = new MemoryStream();
        imageIn.Save(ms, imageIn.RawFormat);
        return ms.ToArray();
    }
}