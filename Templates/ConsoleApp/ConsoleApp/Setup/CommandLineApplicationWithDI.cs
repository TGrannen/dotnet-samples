using ConsoleApp.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsoleApp.Setup
{
    public class CommandLineApplicationWithDI : CommandLineApplication
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandLineApplicationWithDI(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            RegisterCommands();
            HelpOption("-? | -h | --help");
        }

        private void RegisterCommands()
        {
            foreach (var command in _serviceProvider.GetServices<ICommand>())
            {
                if (command is CommandLineApplication app)
                {
                    Commands.Add(app);
                }
                else
                {
                    throw new InvalidCastException("Commands must inherit from ICommand and CommandLineApplication");
                }
            }
        }
    }
}