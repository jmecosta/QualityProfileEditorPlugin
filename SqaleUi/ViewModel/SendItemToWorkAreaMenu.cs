namespace SqaleUi.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Documents;
    using System.Windows.Input;

    using GalaSoft.MvvmLight.Command;

    public class SendItemToWorkAreaMenu : IMenuItem
    {
        private readonly SqaleGridVm sqaleGridVm;

        private SendItemToWorkAreaMenu(SqaleGridVm sqaleGridVm)
        {
            this.sqaleGridVm = sqaleGridVm;

            this.AssociatedCommand = new RelayCommand<object>(this.OnAssociatedCommand);
            this.SubItems = new ObservableCollection<IMenuItem>();
        }

        public static SendItemToWorkAreaMenu MakeMenu(SqaleGridVm sqaleGridVm, SqaleEditorControlViewModel mainModel)
        {
            var menu = new SendItemToWorkAreaMenu(sqaleGridVm) { CommandText = "Send selected items to work area", IsEnabled = false };
            menu.SubItems.Add(new SendItemToWorkAreaMenu(sqaleGridVm) { CommandText = "New Work Area", IsEnabled = false });

            foreach (var tab in mainModel.Tabs)
            {
                if (!tab.Header.Equals("Project"))
                {
                    menu.SubItems.Add(new SendItemToWorkAreaMenu(sqaleGridVm) { CommandText = tab.Header, IsEnabled = false });
                }
            }

            return menu;
        }

        public static void RefreshMenuItems(ObservableCollection<IMenuItem> menus, SqaleEditorControlViewModel model, SqaleGridVm gridModel, bool isenabled)
        {
            var listOfFilesToRemove = new List<IMenuItem>();
            foreach (var item in menus)
            {
                if (item is SendItemToWorkAreaMenu)
                {


                    foreach (var menuItem in item.SubItems)
                    {
                        if (menuItem.CommandText.Equals("New Work Area"))
                        {
                            continue;
                        }

                        bool found = false;
                        foreach (var tab in model.Tabs)
                        {
                            if (tab.Header.Equals(menuItem.CommandText))
                            {
                                found = true;
                            }   
                        }

                        if (!found)
                        {
                            listOfFilesToRemove.Add(menuItem);
                        }
                    }

                    for (int i =0; i < listOfFilesToRemove.Count; i++)
                    {
                        item.SubItems.Remove(listOfFilesToRemove[i]);
                    }

                }
            }

            foreach (var tab in model.Tabs)
            {
                if (tab.Header.Equals("Project"))
                {
                    continue;
                }
                        
                foreach (var item in menus)
                {
                    if (item is SendItemToWorkAreaMenu)
                    {
                        bool found = false;

                        foreach (var menuItem in item.SubItems)
                        {
                            if (tab.Header.Equals(menuItem.CommandText))
                            {
                                found = true;
                            }                                    
                        }

                        if (!found)
                        {
                            item.SubItems.Add(new SendItemToWorkAreaMenu(gridModel) { CommandText = tab.Header, IsEnabled = isenabled });
                        }
                    }
                }                       
            }                   
        }

        public static void RefreshMenuItemsStatus(ObservableCollection<IMenuItem> menus, bool enableCommands)
        {
            foreach (var item in menus)
            {
                if (item is SendItemToWorkAreaMenu)
                {
                    foreach (var menu in item.SubItems)
                    {
                        menu.IsEnabled = enableCommands;
                    }
                }
            }

        }

        private void OnAssociatedCommand(object obj)
        {
            if (IsEnabled)
            {
                this.sqaleGridVm.SendSelectedItemsToWorkArea(this.CommandText);
            }
            
        }

        public ObservableCollection<IMenuItem> SubItems { get; set; }

        public string CommandText { get; set; }

        public ICommand AssociatedCommand { get; set; }

        public bool IsEnabled { get; set; }
    }
}