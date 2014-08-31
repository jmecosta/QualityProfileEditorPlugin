namespace SqaleUi.View
{
    using System.Windows;

    /// <summary>
	/// Interaction logic for PromptUserData.xaml
	/// </summary>
    public partial class PromptUserData : Window
    {
        public enum InputType
        {
            Text,
            Password
        }

        private InputType _inputType = InputType.Text;

        public PromptUserData(string question, string title, string defaultValue = "", InputType inputType = InputType.Text)
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PromptDialog_Loaded);
            txtQuestion.Text = question;
            Title = title;
            txtResponse.Text = defaultValue;
            _inputType = inputType;
            if (_inputType == InputType.Password)
                txtResponse.Visibility = Visibility.Collapsed;

        }

        void PromptDialog_Loaded(object sender, RoutedEventArgs e)
        {
                txtResponse.Focus();
        }

        public static string Prompt(string question, string title, string defaultValue = "", InputType inputType = InputType.Text)
        {
            PromptUserData inst = new PromptUserData(question, title, defaultValue, inputType);
            inst.ShowDialog();
            if (inst.DialogResult == true)
                return inst.ResponseText;
            return null;
        }

        public string ResponseText
        {
            get
            {
                    return txtResponse.Text;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}