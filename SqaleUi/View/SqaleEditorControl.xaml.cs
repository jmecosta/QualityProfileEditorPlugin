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

        public void UpdateConfiguration(ConnectionConfiguration configuration, Resource project, IVsEnvironmentHelper vshelper)
        {
            this.Configuration = configuration;
            this.Project = project;
            this.VsHelper = vshelper;
        }

        public IVsEnvironmentHelper VsHelper { get; set; }

        public Resource Project { get; set; }

        public ConnectionConfiguration Configuration { get; set; }
    }
}
