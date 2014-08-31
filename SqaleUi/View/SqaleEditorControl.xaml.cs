namespace SqaleUi.View
{
    using System.Windows.Controls;

    using SqaleUi.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SqaleEditorControl : UserControl
    {

        public SqaleEditorControl()
        {
            InitializeComponent();

            this.DataContext = new SqaleEditorControlViewModel();
        }
    }
}
