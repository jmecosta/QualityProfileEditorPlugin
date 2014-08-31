namespace SqaleUi.ViewModel
{
    public interface IFilter
    {
        bool FilterFunction(object parameter);

        bool IsEnabled();
    }
}