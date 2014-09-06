namespace SqaleUi.Menus
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using GalaSoft.MvvmLight.Command;

    using SqaleUi.ViewModel;

    /// <summary>
    /// The select key menu item.
    /// </summary>
    class SelectKeyMenuItem : IMenuItem
    {
        private SqaleGridVm sqaleGridVm;

        private SelectKeyMenuItem(SqaleGridVm sqaleGridVm)
        {
            this.sqaleGridVm = sqaleGridVm;

            this.AssociatedCommand = new RelayCommand<object>(this.OnAssociatedCommand);
            this.SubItems = new ObservableCollection<IMenuItem>();

        }
        public static SelectKeyMenuItem MakeMenu(SqaleGridVm sqaleGridVm, SqaleEditorControlViewModel mainModel)
        {
            var menu = new SelectKeyMenuItem(sqaleGridVm) { CommandText = "Change Key", IsEnabled = true };
            
            return menu;
        }

        private void OnAssociatedCommand(object obj)
        {
            if (this.sqaleGridVm.SelectedRule == null)
            {
                return;
            }

            var key = this.sqaleGridVm.CreateNewKey();
            if (!string.IsNullOrEmpty(key))
            {
                this.sqaleGridVm.SelectedRule.Key = key;
            }
            
            this.sqaleGridVm.RefreshView();
        }

        public string CommandText { get; set; }

        public ICommand AssociatedCommand { get; set; }

        public bool IsEnabled { get; set; }

        public ObservableCollection<IMenuItem> SubItems { get; set; }

        public static void RefreshMenuItemsStatus(ObservableCollection<IMenuItem> contextMenuItems, bool isEnabled)
        {
            foreach (var item in contextMenuItems)
            {
                if (item is SelectKeyMenuItem)
                {
                    item.IsEnabled = isEnabled;
                }
            }
        }
    }
}