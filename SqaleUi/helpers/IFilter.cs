namespace SqaleUi.helpers
{
    public interface IFilter
    {
        bool FilterFunction(object parameter);

        bool IsEnabled();
    }
}