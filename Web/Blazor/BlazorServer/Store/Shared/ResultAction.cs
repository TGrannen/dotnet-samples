namespace BlazorServer.Store.Shared
{
    public abstract class ResultAction
    {
        protected ResultAction(string? errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string? ErrorMessage { get; }
        public bool HasCurrentError => !string.IsNullOrWhiteSpace(ErrorMessage);
    }
}