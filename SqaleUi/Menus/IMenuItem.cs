// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMenuItem.cs" company="">
//   
// </copyright>
// <summary>
//   The MenuItem interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SqaleUi.Menus
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using ExtensionTypes;

    using GalaSoft.MvvmLight.Command;

    using SonarRestService;

    using SqaleUi.View;
    using SqaleUi.ViewModel;

    /// <summary>
    ///     The MenuItem interface.
    /// </summary>
    public interface IMenuItem
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the associated command.
        /// </summary>
        ICommand AssociatedCommand { get; set; }

        /// <summary>
        ///     Gets or sets the command text.
        /// </summary>
        string CommandText { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        ///     Gets or sets the sub items.
        /// </summary>
        ObservableCollection<IMenuItem> SubItems { get; set; }

        #endregion
    }

    /// <summary>
    /// The create tag menu item.
    /// </summary>
    internal class CreateTagMenuItem : IMenuItem
    {
        #region Fields

        /// <summary>
        /// The model.
        /// </summary>
        private readonly ISqaleGridVm model;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTagMenuItem"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public CreateTagMenuItem(ISqaleGridVm model)
        {
            this.model = model;
            this.CommandText = "Change Tags";
            this.IsEnabled = true;
            this.AssociatedCommand = new RelayCommand(this.OnAssociateCommand);

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

        /// <summary>
        /// Gets or sets the tag model.
        /// </summary>
        public TagEditorViewModel TagModel { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The make menu.
        /// </summary>
        /// <param name="sqaleGridVm">
        /// The sqale grid vm.
        /// </param>
        /// <returns>
        /// The <see cref="IMenuItem"/>.
        /// </returns>
        public static IMenuItem MakeMenu(ISqaleGridVm sqaleGridVm)
        {
            return new CreateTagMenuItem(sqaleGridVm);
        }

        /// <summary>
        /// The refresh menu items status.
        /// </summary>
        /// <param name="contextMenuItems">
        /// The context menu items.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        public static void RefreshMenuItemsStatus(ObservableCollection<IMenuItem> contextMenuItems, bool b)
        {
            foreach (IMenuItem contextMenuItem in contextMenuItems)
            {
                if (contextMenuItem is CreateTagMenuItem)
                {
                    contextMenuItem.IsEnabled = b;

                    if (((CreateTagMenuItem)contextMenuItem).TagModel != null)
                    {
                        ((CreateTagMenuItem)contextMenuItem).TagModel.RefreshTagsInRule();
                    }
                }
            }
        }

        /// </param>
        public static void RefreshMenuItemsStatus(ObservableCollection<IMenuItem> contextMenuItems, bool b, ISonarConfiguration conf, ISonarRestService rest, ISqaleGridVm model)
        {
            foreach (IMenuItem contextMenuItem in contextMenuItems)
            {
                if (contextMenuItem is CreateTagMenuItem)
                {
                    contextMenuItem.IsEnabled = b;

                    if (((CreateTagMenuItem)contextMenuItem).TagModel != null)
                    {
                        ((CreateTagMenuItem)contextMenuItem).TagModel.RefreshTagsInRule();
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The on associate command.
        /// </summary>
        public void OnAssociateCommand()
        {
            if (this.TagModel == null)
            {
                this.TagModel = new TagEditorViewModel(this.model.Configuration, this.model.RestService, this.model);
            }

            var window = new TagEditorView(this.TagModel);
            window.Show();
        }

        #endregion
    }
}