// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataModel.cs" company="">
//   
// </copyright>
// <summary>
//   The DataModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi.helpers
{
    using System.ComponentModel;

    /// <summary>
    /// The DataModel interface.
    /// </summary>
    public interface IDataModel
    {
        #region Public Methods and Operators

        /// <summary>
        /// The process changes.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="propertyChangedEventArgs">
        /// The property changed event args.
        /// </param>
        void ProcessChanges(object sender, PropertyChangedEventArgs propertyChangedEventArgs);

        #endregion
    }
}