// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateRuleWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for CreateRuleWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SqaleUi
{
    using SqaleUi.ViewModel;

    /// <summary>
    ///     Interaction logic for CreateRuleWindow.xaml
    /// </summary>
    public partial class CreateRuleWindow
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRuleWindow"/> class.
        /// </summary>
        /// <param name="createRulesModel">
        /// The create rules model.
        /// </param>
        public CreateRuleWindow(CreateRuleViewModel createRulesModel)
        {
            this.InitializeComponent();

            this.DataContext = createRulesModel;
        }

        #endregion
    }
}