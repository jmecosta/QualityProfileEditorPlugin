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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    using ExtensionTypes;

    using GalaSoft.MvvmLight.Command;

    using PropertyChanged;

    using SqaleUi.Menus;

    /// <summary>
    ///     The create rule view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class CreateRuleViewModel
    {
        #region Fields

        /// <summary>
        ///     The selected rule.
        /// </summary>
        private Rule selectedRule;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateRuleViewModel" /> class.
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
        public CreateRuleViewModel(ISqaleGridVm model)
        {
            this.Model = model;
            this.Profile = new Profile();
            this.TemplateRules = new ObservableCollection<Rule>();
            this.ExecuteRefreshCustomRuleCommand();

            this.CanExecuteCreateCustomRuleCommand = false;
            this.CreateCustomRuleCommand = new RelayCommand(this.ExecuteCreateCustomRuleCommand);
            this.RefreshCustomRuleCommand = new RelayCommand(this.ExecuteRefreshCustomRuleCommand);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRuleViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="selectedProfile">
        /// The selected profile.
        /// </param>
        public CreateRuleViewModel(ISqaleGridVm model, Profile selectedProfile)
        {
            this.Model = model;
            this.Profile = selectedProfile;
            this.TemplateRules = new ObservableCollection<Rule>();
            this.ExecuteRefreshCustomRuleCommand();

            this.CanExecuteCreateCustomRuleCommand = false;
            this.CreateCustomRuleCommand = new RelayCommand(this.ExecuteCreateCustomRuleCommand);
            this.RefreshCustomRuleCommand = new RelayCommand(this.ExecuteRefreshCustomRuleCommand);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether can execute create custom rule command.
        /// </summary>
        public bool CanExecuteCreateCustomRuleCommand { get; set; }

        /// <summary>
        ///     Gets or sets the create custom rule command.
        /// </summary>
        public RelayCommand CreateCustomRuleCommand { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        public ISqaleGridVm Model { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        public Profile Profile { get; set; }

        /// <summary>
        /// Gets or sets the refresh custom rule command.
        /// </summary>
        public RelayCommand RefreshCustomRuleCommand { get; set; }

        /// <summary>
        ///     Gets or sets the selected rule.
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
        /// Gets or sets the selected severity.
        /// </summary>
        public Severity SelectedSeverity { get; set; }

        /// <summary>
        ///     Gets or sets the template rules.
        /// </summary>
        public ObservableCollection<Rule> TemplateRules { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The execute create custom rule command.
        /// </summary>
        private void ExecuteCreateCustomRuleCommand()
        {
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Key) || string.IsNullOrEmpty(this.Description))
            {
                MessageBox.Show("Cannot add rule, some elements have not been set");
                return;
            }

            var rule = this.SelectedRule.Clone() as Rule;
            if (rule != null)
            {
                rule.Name = this.Name;
                rule.Key = this.Key;
                rule.Severity = this.SelectedSeverity;
                rule.Description = this.Description;

                List<string> errors = this.Model.RestService.CreateRule(this.Model.Configuration, rule, this.SelectedRule);

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

        /// <summary>
        /// The execute refresh custom rule command.
        /// </summary>
        private void ExecuteRefreshCustomRuleCommand()
        {
            this.TemplateRules.Clear();
            
            if (this.Profile.Key == null)
            {
                this.Profile.Key = this.Model.QualityViewerModel.SelectedProfile.Key;
                this.Profile.Language = this.Model.QualityViewerModel.SelectedProfile.Language;
                this.Model.RestService.GetTemplateRules(this.Model.Configuration, this.Profile);
                foreach (Rule rule in this.Profile.Rules)
                {
                    this.TemplateRules.Add(rule);
                }
            }
            else
            {
                var profileTag = new Profile();
                if (profileTag.Rules == null)
                {
                    profileTag.Rules = new List<Rule>();
                }

                profileTag.Key = this.Profile.Key;
                profileTag.Language = this.Profile.Language;

                this.Model.RestService.GetTemplateRules(this.Model.Configuration, profileTag);
                foreach (var rule in profileTag.Rules)
                {
                    this.TemplateRules.Add(rule);
                }
            }
        }

        #endregion
    }
}