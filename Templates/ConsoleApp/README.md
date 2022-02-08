# Command based Console App

This project showcases the simplicity of the `Cocona` package for creating command line commands as well as using
`CommandLineUtils` for prompting for user input. These packages allow you to easily create expansive command line user
interfaces with automatic parameter reading and help menu generation.

To view this demo, build the project and invoke the CLI help menu with `.\ConsoleApp.exe -h` to see the available
commands.

Packages:

* [Cocona](https://github.com/mayuki/Cocona)
* [McMaster.Extensions.CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)

Examples:
```text
.\ConsoleApp.exe -h
Usage: ConsoleApp [command]

ConsoleApp

Commands:
  test

Options:
  -h, --help    Show help message
  --version     Show version
```

```text
.\ConsoleApp.exe test -h
Usage: ConsoleApp test [--name <String>] [--count <Int32>] [--help]

Options:
  --name <String>     (Required)
  --count <Int32>     (Required)
  -h, --help         Show help message
```
```text
.\ConsoleApp.exe --version
ConsoleApp 1.0.0
```