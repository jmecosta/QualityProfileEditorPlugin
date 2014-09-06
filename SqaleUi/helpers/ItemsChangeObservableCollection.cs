// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsChangeObservableCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The items change observable collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi.helpers
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using SqaleUi.ViewModel;

    /// <summary>
    /// The items change observable collection.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class ItemsChangeObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The model.
        /// </summary>
        private readonly IDataModel model;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsChangeObservableCollection{T}"/> class.
        /// </summary>
        /// <param name="sqaleGridVm">
        /// The sqale grid vm.
        /// </param>
        public ItemsChangeObservableCollection(IDataModel sqaleGridVm)
        {
            this.model = sqaleGridVm;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The clear items.
        /// </summary>
        protected override void ClearItems()
        {
            this.UnRegisterPropertyChanged(this);
            base.ClearItems();
        }

        /// <summary>
        /// The on collection changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                this.RegisterPropertyChanged(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.UnRegisterPropertyChanged(e.OldItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                this.UnRegisterPropertyChanged(e.OldItems);
                this.RegisterPropertyChanged(e.NewItems);
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// The register property changed.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        private void RegisterPropertyChanged(IList items)
        {
            foreach (INotifyPropertyChanged item in items)
            {
                if (item != null)
                {
                    item.PropertyChanged += this.item_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// The un register property changed.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        private void UnRegisterPropertyChanged(IList items)
        {
            foreach (INotifyPropertyChanged item in items)
            {
                if (item != null)
                {
                    item.PropertyChanged -= this.item_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// The item_ property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.model.ProcessChanges(sender, e);
        }

        #endregion
    }
}