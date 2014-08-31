// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqaleEditorControlViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   This class contains properties that the main View can data bind to.
//   Use the mvvminpc snippet to add bindable properties to this ViewModel.
//   You can also use Blend to data bind with the tool's support.
//   See http://www.galasoft.ch/mvvm
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SqaleUi.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Input;

    using GalaSoft.MvvmLight.Command;

    using PropertyChanged;

    using SqaleManager;

    using SqaleUi.View;

    /// <summary>
    /// The sqale editor control view model.
    /// </summary>
    [ImplementPropertyChanged]
    public class SqaleEditorControlViewModel
    {
        #region Fields

        /// <summary>
        ///     The selected tab.
        /// </summary>
        private SqaleGridVm selectedTab;

        /// <summary>
        /// The tab changed.
        /// </summary>
        private PropertyChangingEventHandler tabChanged;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the SqaleEditorControlViewModel class.
        /// </summary>
        public SqaleEditorControlViewModel()
        {
            this.Tabs = new ObservableCollection<SqaleGridVm>();

            this.CanExecuteNewProjectCommand = true;
            this.CanExecuteOpenProjectCommand = true;
            this.CanExecuteSaveProjectCommand = false;
            this.CanExecuteSaveAsProjectCommand = false;
            this.CanExecuteCloseProjectCommand = false;

            this.NewProjectCommand = new RelayCommand(this.ExecuteNewProjectCommand, () => this.CanExecuteNewProjectCommand);
            this.OpenProjectCommand = new RelayCommand(this.ExecuteOpenProjectCommand, () => this.CanExecuteOpenProjectCommand);
            this.SaveProjectCommand = new RelayCommand(this.ExecuteSaveProjectCommand, () => this.CanExecuteSaveProjectCommand);
            this.SaveAsProjectCommand = new RelayCommand(this.ExecuteSaveAsProjectCommand, () => this.CanExecuteSaveAsProjectCommand);
            this.CloseProjectCommand = new RelayCommand(this.ExecuteCloseProjectCommand, () => this.CanExecuteCloseProjectCommand);

            this.CreateWorkAreaCommand = new RelayCommand<object>(item => this.CreateNewWorkArea(false));
            this.DeleteWorkAreaCommand = new RelayCommand(this.RemoveCurrentSelectedTab);
        }



        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether can execute new project command.
        /// </summary>
        public bool CanExecuteCloseProjectCommand { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether can execute new project command.
        /// </summary>
        public bool CanExecuteNewProjectCommand { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether can execute new project command.
        /// </summary>
        public bool CanExecuteOpenProjectCommand { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether can execute save project command.
        /// </summary>
        public bool CanExecuteSaveProjectCommand { get; set; }

        /// <summary>
        ///     Gets the close project command.
        /// </summary>
        public RelayCommand CloseProjectCommand { get; private set; }

        /// <summary>
        /// Gets or sets the create work area command.
        /// </summary>
        public ICommand CreateWorkAreaCommand { get; set; }

        /// <summary>
        /// Gets or sets the delete work area command.
        /// </summary>
        public object DeleteWorkAreaCommand { get; set; }

        /// <summary>
        /// Gets a value indicating whether is add tab enabled.
        /// </summary>
        public bool IsAddTabEnabled
        {
            get
            {
                return this.Tabs.Count > 0;
            }
        }

        /// <summary>
        /// Gets the is remove tab enabled.
        /// </summary>
        public object IsRemoveTabEnabled
        {
            get
            {
                if (this.SelectedTab == null)
                {
                    return false;
                }

                return !this.SelectedTab.Header.Equals("Project");
            }
        }

        /// <summary>
        ///     Gets the new project command.
        /// </summary>
        public RelayCommand NewProjectCommand { get; private set; }

        /// <summary>
        ///     Gets the open project command.
        /// </summary>
        public RelayCommand OpenProjectCommand { get; private set; }

        /// <summary>
        /// Gets or sets the save project command.
        /// </summary>
        public ICommand SaveProjectCommand { get; set; }

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        public SqaleGridVm SelectedTab
        {
            get
            {
                return this.selectedTab;
            }

            set
            {
                if (value != null)
                {
                    value.RefreshView();
                }

                this.selectedTab = value;
            }
        }

        /// <summary>
        /// Gets or sets the tabs.
        /// </summary>
        public ObservableCollection<SqaleGridVm> Tabs { get; set; }

        public ICommand SaveAsProjectCommand { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create new work area.
        /// </summary>
        /// <param name="showContextMenu">
        /// The show context menu.
        /// </param>
        /// <returns>
        /// The <see cref="SqaleGridVm"/>.
        /// </returns>
        public SqaleGridVm CreateNewWorkArea(bool showContextMenu)
        {
            var newWorkArea = new SqaleGridVm(this, new SqaleManager())
                                  {
                                      Header = "Non Saved Data: " + this.Tabs.Count, 
                                      ShowContextMenu = showContextMenu, 
                                      CanSendToProject = true, 
                                      CanSendToWorkAreaCommand = true, 
                                      IsDirty = false
                                  };
            newWorkArea.CanSendToWorkAreaCommand = false;
            this.Tabs.Add(newWorkArea);
            this.SelectedTab = newWorkArea;
            return newWorkArea;
        }

        /// <summary>
        /// The create work area with data set.
        /// </summary>
        /// <param name="profileRules">
        /// The profile rules.
        /// </param>
        public void CreateWorkAreaWithDataSet(ObservableCollection<Rule> profileRules)
        {
            var newWorkAread = new SqaleGridVm(this, new SqaleManager()) { Header = "Non Saved Data: " + this.Tabs.Count };
            newWorkAread.CanSendToWorkAreaCommand = false;
            foreach (Rule profileRule in profileRules)
            {
                newWorkAread.ProfileRules.Add(new Rule());
            }

            this.Tabs.Add(newWorkAread);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The execute close project command.
        /// </summary>
        private void ExecuteCloseProjectCommand()
        {

            this.ExecuteSaveAsProjectCommand();
            this.Tabs.Remove(this.Tabs[0]);

            this.CanExecuteNewProjectCommand = true;
            this.CanExecuteOpenProjectCommand = true;

            this.CanExecuteSaveProjectCommand = false;
            this.CanExecuteSaveAsProjectCommand = false;
            this.CanExecuteCloseProjectCommand = false;
        }

        /// <summary>
        ///     The execute new project command.
        /// </summary>
        private void ExecuteNewProjectCommand()
        {
            this.CreateNewProject(string.Empty);
            this.CanExecuteNewProjectCommand = false;
            this.CanExecuteOpenProjectCommand = false;

            this.CanExecuteSaveProjectCommand = true;
            this.CanExecuteSaveAsProjectCommand = true;
            this.CanExecuteCloseProjectCommand = true;
        }

        public bool CanExecuteSaveAsProjectCommand { get; set; }

        /// <summary>
        ///     The execute open project command.
        /// </summary>
        private void ExecuteOpenProjectCommand()
        {
            // Do something 
            var filedialog = new OpenFileDialog { Filter = @"Project Model|*.xml" };

            DialogResult result = filedialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            try
            {
                this.CreateNewProject(filedialog.FileName);

                this.CanExecuteNewProjectCommand = false;
                this.CanExecuteOpenProjectCommand = false;
                this.CanExecuteSaveAsProjectCommand = true;
                this.CanExecuteSaveProjectCommand = true;
                this.CanExecuteCloseProjectCommand = true;

                this.Tabs[0].ProjectFile = filedialog.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot Open Project: " + ex.Message);
            }

        }

        private void CreateNewProject(string fileName)
        {
            var project = new SqaleGridVm(this, new SqaleManager())
                              {
                                  Header = "Project",
                                  ShowContextMenu = true,
                                  CanSendToProject = false,
                                  CanSendToWorkAreaCommand = true
                              };

            this.Tabs.Add(project);
            this.SelectedTab = this.Tabs[0];

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            if (!File.Exists(fileName))
            {
                return;
            }

            var model = project.SqaleManager.ImportSqaleProjectFromFile(fileName);
            foreach (var rule in model.GetProfile().Rules)
            {
                project.ProfileRules.Add(rule);
            }

            var importViewerModel = new ImportLogViewModel();
            importViewerModel.ImportLog = project.SqaleManager.GetImportLog();
            if (importViewerModel.ImportLog.Count > 0)
            {
                var importLogWindow = new ImportLogView(importViewerModel);
                importLogWindow.Show();
            }
        }

        /// <summary>
        /// The execute save project command.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void ExecuteSaveProjectCommand()
        {
            if (string.IsNullOrEmpty(this.Tabs[0].ProjectFile))
            {
                this.ExecuteSaveAsProjectCommand();
                return;
            }

            var modelToExport = this.Tabs[0].SqaleManager.CreateModelFromRules(this.Tabs[0].ProfileRules);
            this.Tabs[0].SqaleManager.SaveSqaleModelAsXmlProject(modelToExport, this.Tabs[0].ProjectFile);
        }

        private void ExecuteSaveAsProjectCommand()
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "xml files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var modelToExport = this.Tabs[0].SqaleManager.CreateModelFromRules(this.Tabs[0].ProfileRules);
                    this.Tabs[0].SqaleManager.SaveSqaleModelAsXmlProject(modelToExport, saveFileDialog.FileName);
                    this.Tabs[0].ProjectFile = saveFileDialog.FileName;
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Cannot Save: " + ex.Message);
                }


            }            
        }

        /// <summary>
        /// The remove current selected tab.
        /// </summary>
        private void RemoveCurrentSelectedTab()
        {
            if (!this.SelectedTab.IsDirty)
            {
                this.Tabs.Remove(this.SelectedTab);
                this.SelectedTab = this.Tabs[0];
                this.Tabs[0].RefreshMenus();
                return;
            }

            DialogResult result = MessageBox.Show("Changes will be lost, proceed?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Tabs.Remove(this.SelectedTab);
                this.SelectedTab = this.Tabs[0];
            }
        }

        #endregion
    }
}