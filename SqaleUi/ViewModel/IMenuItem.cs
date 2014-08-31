namespace SqaleUi.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Windows.Input;

    /// <summary>
    /// The MenuItem interface.
    /// </summary>
    public interface IMenuItem
    {
        string CommandText { get; set; }

        ICommand AssociatedCommand { get; set; }

        bool IsEnabled { get; set; }

        ObservableCollection<IMenuItem> SubItems { get; set; }
    }
}