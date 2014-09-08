// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomRuleSectorViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The custom rule sector view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Windows;

    using ExtensionTypes;

    using GalaSoft.MvvmLight.Command;

    using PropertyChanged;

    /// <summary>
    ///     The custom rule sector view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class CustomRuleSectorViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRuleSectorViewModel"/> class.
        /// </summary>
        public CustomRuleSectorViewModel()
        {
            this.CustomRules = new ObservableCollection<Rule>();
            this.SelectRuleCommand = new RelayCommand<Window>(this.ExecuteSelectRule, this.CanExecute);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the custom rules.
        /// </summary>
        public ObservableCollection<Rule> CustomRules { get; set; }

        /// <summary>
        /// Gets or sets the select rule command.
        /// </summary>
        public RelayCommand<Window> SelectRuleCommand { get; set; }

        /// <summary>
        /// Gets or sets the selected rule.
        /// </summary>
        public Rule SelectedRule { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="arg">
        /// The arg.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanExecute(Window arg)
        {
            return this.SelectedRule != null;
        }

        /// <summary>
        /// The execute select rule.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void ExecuteSelectRule(Window obj)
        {
            obj.Close();
        }

        #endregion
    }
}