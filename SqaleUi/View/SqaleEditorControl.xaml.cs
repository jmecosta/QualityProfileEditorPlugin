namespace SqaleUi.View
{
    using System.Windows.Controls;

    using ExtensionTypes;

    using SqaleUi.ViewModel;

    using VSSonarPlugins;

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

        public SqaleEditorControl(SqaleEditorControlViewModel model)
        {
            InitializeComponent();
            this.DataContext = model;
        }
    }
}
