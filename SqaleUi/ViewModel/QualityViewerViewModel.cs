// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityViewerViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The server project viewer view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SqaleUi.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    using ExtensionTypes;

    using GalaSoft.MvvmLight.Command;

    using PropertyChanged;

    using SonarRestService;

    /// <summary>
    ///     The server project viewer view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class QualityViewerViewModel
    {
        #region Fields

        /// <summary>
        /// The selected profile.
        /// </summary>
        private Profile selectedProfile;

        /// <summary>
        ///     The selected profile.
        /// </summary>
        private SonarProject selectedProject;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="QualityViewerViewModel" /> class.
        /// </summary>
        public QualityViewerViewModel()
        {
            this.Profiles = new ObservableCollection<Profile>();
            this.Service = new SonarRestService(new JsonSonarConnector());
            this.Projects = new ObservableCollection<SonarProject>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QualityViewerViewModel"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="showOnlyProfiles">
        /// The show Only Profiles.
        /// </param>
        public QualityViewerViewModel(ConnectionConfiguration config, SqaleGridVm model, bool showOnlyProfiles = false)
        {
            this.Model = model;
            this.Configuration = config;
            this.ShowOnlyProfiles = showOnlyProfiles;

            this.Service = new SonarRestService(new JsonSonarConnector());
            this.Profiles = new ObservableCollection<Profile>();
            this.Projects = new ObservableCollection<SonarProject>();
            this.StartCommand();

            this.ExecuteRefreshDataCommand();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether can execute import profile command.
        /// </summary>
        public bool CanExecuteImportProfileCommand { get; set; }

        /// <summary>
        ///     Gets or sets the configuration.
        /// </summary>
        public ConnectionConfiguration Configuration { get; set; }

        /// <summary>
        ///     Gets or sets the import profile command.
        /// </summary>
        public RelayCommand<Window> ImportProfileCommand { get; set; }

        /// <summary>
        ///     Gets or sets the model.
        /// </summary>
        public SqaleGridVm Model { get; set; }

        /// <summary>
        ///     Gets or sets the projects.
        /// </summary>
        public ObservableCollection<Profile> Profiles { get; set; }

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        public ObservableCollection<SonarProject> Projects { get; set; }

        /// <summary>
        /// Gets or sets the refresh data command.
        /// </summary>
        public RelayCommand RefreshDataCommand { get; set; }

        /// <summary>
        /// Gets or sets the selected profile.
        /// </summary>
        public Profile SelectedProfile
        {
            get
            {
                return this.selectedProfile;
            }

            set
            {
                this.selectedProfile = value;
                if (value == null)
                {
                    this.CanExecuteImportProfileCommand = false;
                }
                else
                {
                    this.CanExecuteImportProfileCommand = true;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the selected profile.
        /// </summary>
        [AlsoNotifyFor("SelectedProfile")]
        public SonarProject SelectedProject
        {
            get
            {
                return this.selectedProject;
            }

            set
            {
                this.selectedProject = value;
                this.Profiles.Clear();
                if (value != null)
                {
                    foreach (Profile profile in value.Profiles)
                    {
                        this.Profiles.Add(profile);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the service.
        /// </summary>
        public ISonarRestService Service { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show only profiles.
        /// </summary>
        public bool ShowOnlyProfiles { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The can execute import profile command func.
        /// </summary>
        /// <param name="sdas">
        /// The sdas.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanExecuteImportProfileCommandFunc(object sdas)
        {
            return this.CanExecuteImportProfileCommand;
        }

        /// <summary>
        /// The execute import profile command.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        private void ExecuteImportProfileCommand(Window window)
        {
            if (this.SelectedProfile.Rules == null || this.SelectedProfile.Rules.Count == 0 || this.Model.ProfileRules.Count == 0)
            {
                this.Service.GetRulesForProfileUsingRulesApp(this.Configuration, this.SelectedProfile, true);

                this.Service.GetRulesForProfileUsingRulesApp(this.Configuration, this.SelectedProfile, false);
            }

            this.Model.SelectedProfile = this.SelectedProfile.Key;
            this.Model.MergeRulesIntoProject(this.SelectedProfile.Rules);
            this.Model.SyncingModelWithSonarServer = true;
            if (window != null)
            {
                window.Hide();
            }
        }

        /// <summary>
        /// The execute refresh data command.
        /// </summary>
        private void ExecuteRefreshDataCommand()
        {
            this.Projects.Clear();
            this.Profiles.Clear();
            if (this.ShowOnlyProfiles)
            {
                List<Profile> profiles = this.Service.GetProfilesUsingRulesApp(this.Configuration);
                foreach (Profile profile in profiles)
                {
                    this.Profiles.Add(profile);
                }
            }
            else
            {
                List<SonarProject> projects = this.Service.GetProjects(this.Configuration);
                List<Profile> profiles = this.Service.GetProfilesUsingRulesApp(this.Configuration);
                foreach (SonarProject sonarProject in projects)
                {
                    foreach (Profile profile in this.Service.GetQualityProfilesForProject(this.Configuration, sonarProject.Key))
                    {
                        foreach (Profile profile1 in profiles)
                        {
                            if (profile1.Name.Equals(profile1.Name) && profile1.Language.Equals(profile.Language))
                            {
                                profile.Key = profile1.Key;
                            }
                        }

                        sonarProject.Profiles.Add(profile);
                    }

                    this.Projects.Add(sonarProject);
                }
            }
        }

        /// <summary>
        ///     The start command.
        /// </summary>
        private void StartCommand()
        {
            try
            {
                this.ImportProfileCommand = new RelayCommand<Window>(this.ExecuteImportProfileCommand, this.CanExecuteImportProfileCommandFunc);
                this.RefreshDataCommand = new RelayCommand(this.ExecuteRefreshDataCommand);
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}