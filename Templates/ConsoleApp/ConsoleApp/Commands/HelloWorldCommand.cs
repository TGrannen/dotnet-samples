using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Commands
{
    internal class HelloWorldCommand : CommandLineApplication, ICommand
    {
        private readonly ILogger<HelloWorldCommand> _logger;

        public HelloWorldCommand(ILogger<HelloWorldCommand> logger)
        {
            _logger = logger;
            Name = "HW";
            Description = "Prints Hello World or Hello <SUBJECT> a certain number of times";
            _subjectOption = Option<string>("-s|--subject <SUBJECT>", "Subject to be added to 'Hello'", CommandOptionType.SingleOrNoValue, option => _subject = option.Value(), false);
            _numberOption = Option<int>("-n|--number <COUNT>", "Number of Times to repeat", CommandOptionType.SingleOrNoValue, option => { _count = 1; }, false);
            HelpOption("-? | -h | --help");
            OnExecute(Execute);
        }

        private int _count;
        private string _subject;
        private readonly CommandOption<string> _subjectOption;
        private readonly CommandOption<int> _numberOption;

        private int Execute()
        {
            _count = _numberOption.ParsedValue;
            _subject = _subjectOption.ParsedValue;
            if (_count > 1)
            {
                var continueWithMultiple = Prompt.GetYesNo($"Are you sure that you want to run this {_count} times?", false);
                if (!continueWithMultiple)
                {
                    _logger.LogInformation("Cancelling Hello World");
                    return 0;
                }
            }

            for (var i = 0; i < _count; i++)
            {
                _logger.LogInformation($"Hello {(_subject ?? "world")}");
            }

            return 0;
        }
    }
}