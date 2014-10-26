// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectKeyMenuItem.cs" company="">
//   
// </copyright>
// <summary>
//   The select key menu item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SqaleUi.Menus
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using GalaSoft.MvvmLight.Command;

    using SqaleUi.ViewModel;

    /// <summary>
    ///     The select key menu item.
    /// </summary>
    internal class SelectKeyMenuItem : IMenuItem
    {
        #region Fields

        /// <summary>
        /// The sqale grid vm.
        /// </summary>
        private readonly ISqaleGridVm sqaleGridVm;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectKeyMenuItem"/> class.
        /// </summary>
        /// <param name="sqaleGridVm">
        /// The sqale grid vm.
        /// </param>
        private SelectKeyMenuItem(ISqaleGridVm sqaleGridVm)
        {
            this.sqaleGridVm = sqaleGridVm;

            this.AssociatedCommand = new RelayCommand<object>(this.OnAssociatedCommand);
            this.SubItems = new ObservableCollection<IMenuItem>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the associated command.
        /// </summary>
        public ICommand AssociatedCommand { get; set; }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the sub items.
        /// </summary>
        public ObservableCollection<IMenuItem> SubItems { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The make menu.
        /// </summary>
        /// <param name="sqaleGridVm">
        /// The sqale grid vm.
        /// </param>
        /// <returns>
        /// The <see cref="SelectKeyMenuItem"/>.
        /// </returns>
        public static SelectKeyMenuItem MakeMenu(ISqaleGridVm sqaleGridVm)
        {
            var menu = new SelectKeyMenuItem(sqaleGridVm) { CommandText = "Change Key", IsEnabled = true };

            return menu;
        }

        /// <summary>
        /// The refresh menu items status.
        /// </summary>
        /// <param name="contextMenuItems">
        /// The context menu items.
        /// </param>
        /// <param name="isEnabled">
        /// The is enabled.
        /// </param>
        public static void RefreshMenuItemsStatus(ObservableCollection<IMenuItem> contextMenuItems, bool isEnabled)
        {
            foreach (IMenuItem item in contextMenuItems)
            {
                if (item is SelectKeyMenuItem)
                {
                    item.IsEnabled = isEnabled;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on associated command.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        private void OnAssociatedCommand(object obj)
        {
            if (this.sqaleGridVm.SelectedRule == null)
            {
                return;
            }

            string key = this.sqaleGridVm.CreateNewKey();
            if (!string.IsNullOrEmpty(key))
            {
                this.sqaleGridVm.SelectedRule.Key = key;
            }

            this.sqaleGridVm.RefreshView();
        }

        #endregion
    }
}