// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqaleGridVm.cs" company="">
//   
// </copyright>
// <summary>
//   The SqaleGridVm interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace SqaleUi.Menus
{
    using System.Collections.Generic;

    using ExtensionTypes;

    using SonarRestService;

    using SqaleUi.helpers;
    using SqaleUi.ViewModel;

    /// <summary>
    ///     The SqaleGridVm interface.
    /// </summary>
    public interface ISqaleGridVm
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        ISonarConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the profile rules.
        /// </summary>
        ItemsChangeObservableCollection<Rule> ProfileRules { get; set; }

        /// <summary>
        /// Gets or sets the rest service.
        /// </summary>
        ISonarRestService RestService { get; set; }

        /// <summary>
        /// Gets or sets the selected profile.
        /// </summary>
        Profile SelectedProfile { get; set; }

        /// <summary>
        /// Gets or sets the selected rule.
        /// </summary>
        Rule SelectedRule { get; set; }

        /// <summary>
        /// Gets or sets the quality viewer model.
        /// </summary>
        QualityViewerViewModel QualityViewerModel { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The aggregate error strings.
        /// </summary>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string AggregateErrorStrings(string arg1, string arg2);

        /// <summary>
        /// The create new key.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string CreateNewKey();

        /// <summary>
        /// The merge rules into project.
        /// </summary>
        /// <param name="rules">
        /// The rules.
        /// </param>
        void MergeRulesIntoProject(List<Rule> rules);

        /// <summary>
        /// The refresh view.
        /// </summary>
        void RefreshView();

        /// <summary>
        /// The send selected items to work area.
        /// </summary>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        void SendSelectedItemsToWorkArea(string commandText);

        /// <summary>
        /// The set connected to server.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        void SetConnectedToServer(bool b);

        #endregion
    }
}