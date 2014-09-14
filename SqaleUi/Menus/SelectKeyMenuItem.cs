// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectKeyMenuItem.cs" company="Copyright © 2014 Tekla Corporation. Tekla is a Trimble Company">
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