// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateRuleViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The create rule view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    using ExtensionTypes;

    using GalaSoft.MvvmLight.Command;

    using PropertyChanged;

    /// <summary>
    /// The create rule view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class CreateRuleViewModel
    {
        #region Fields

        /// <summary>
        /// The selected rule.
        /// </summary>
        private Rule selectedRule;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRuleViewModel"/> class.
        /// </summary>
        public CreateRuleViewModel()
        {
            this.TemplateRules = new ObservableCollection<Rule>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRuleViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="profile">
        /// The profile.
        /// </param>
        public CreateRuleViewModel(SqaleGridVm model)
        {
            this.Model = model;
            this.Profile = new Profile();
            this.TemplateRules = new ObservableCollection<Rule>();
            this.ExecuteRefreshCustomRuleCommand();

            this.CanExecuteCreateCustomRuleCommand = false;
            this.CreateCustomRuleCommand = new RelayCommand(this.ExecuteCreateCustomRuleCommand, () => this.CanExecuteCreateCustomRuleCommand);
            this.RefreshCustomRuleCommand = new RelayCommand(this.ExecuteRefreshCustomRuleCommand);
        }

        private void ExecuteRefreshCustomRuleCommand()
        {
            this.TemplateRules.Clear();
            this.Profile.Key = this.Model.QualityViewerModel.SelectedProfile.Key;
            this.Profile.Language = this.Model.QualityViewerModel.SelectedProfile.Language;
            this.Model.RestService.GetTemplateRules(this.Model.Configuration, this.Profile);
            
            foreach (Rule rule in this.Profile.Rules)
            {
                this.TemplateRules.Add(rule);
            }
        }

        public RelayCommand RefreshCustomRuleCommand { get; set; }

        public Profile Profile { get; set; }

        public SqaleGridVm Model { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether can execute create custom rule command.
        /// </summary>
        public bool CanExecuteCreateCustomRuleCommand { get; set; }

        /// <summary>
        /// Gets or sets the create custom rule command.
        /// </summary>
        public RelayCommand CreateCustomRuleCommand { get; set; }

        /// <summary>
        /// Gets or sets the selected rule.
        /// </summary>
        public Rule SelectedRule
        {
            get
            {
                return this.selectedRule;
            }

            set
            {
                this.selectedRule = value;
                if (value != null)
                {
                    this.CanExecuteCreateCustomRuleCommand = true;
                    this.SelectedSeverity = value.Severity;
                }
                else
                {
                    this.CanExecuteCreateCustomRuleCommand = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the template rules.
        /// </summary>
        public ObservableCollection<Rule> TemplateRules { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public Severity SelectedSeverity { get; set; }


        #endregion

        #region Methods

        /// <summary>
        /// The execute create custom rule command.
        /// </summary>
        private void ExecuteCreateCustomRuleCommand()
        {
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Key) || string.IsNullOrEmpty(this.Description))
            {
                MessageBox.Show("Cannot add rule, some elements have not been set");
                return;
            }

            Rule rule = this.SelectedRule.Clone() as Rule;
            if (rule != null)
            {
                rule.Name = this.Name;
                rule.Key = this.Key;
                rule.Severity = this.SelectedSeverity;
                rule.Description = this.Description;

                var errors = this.Model.RestService.CreateRule(this.Model.Configuration, rule, this.SelectedRule);

                if (errors != null && errors.Count != 0)
                {
                    MessageBox.Show("Cannot Update Status Of Data in Server: " + errors.Aggregate(this.Model.AggregateErrorStrings));
                }
                else
                {
                    this.Model.ProfileRules.Add(rule);
                    MessageBox.Show("Rule Added");
                }
            }
        }

        #endregion
    }
}