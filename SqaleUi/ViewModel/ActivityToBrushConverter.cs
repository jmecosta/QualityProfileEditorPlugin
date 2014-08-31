// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityToBrushConverter.cs" company="">
//   
// </copyright>
// <summary>
//   The activity to brush converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SqaleUi.ViewModel
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// The activity to brush converter.
    /// </summary>
    public class ActivityToBrushConverter : IValueConverter
    {
        #region Fields

        /// <summary>
        /// The _active brush.
        /// </summary>
        private readonly Brush activeBrush = new SolidColorBrush(Color.FromArgb(255, 255, 191, 191));

        /// <summary>
        /// The _inactive brush.
        /// </summary>
        private readonly Brush inactiveBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? this.activeBrush : this.inactiveBrush;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}