// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateRuleViewModel.cs" company="Copyright © 2014 Tekla Corporation. Tekla is a Trimble Company">
//     Copyright (C) 2013 [Jorge Costa, Jorge.Costa@tekla.com]
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
// You should have received a copy of the GNU Lesser General Public License along with this program; if not, write to the Free
// Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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