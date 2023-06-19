using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DJ.Helper.ListViewLayoutManager
{
    public abstract class ImageGridViewColumn : GridViewColumn, IValueConverter
    {
        // ##############################################################################################################################
        // Constructor
        // ##############################################################################################################################

        #region Constructor

        protected ImageGridViewColumn(Stretch imageStretch)
        {
            FrameworkElementFactory imageElement = new FrameworkElementFactory(typeof(Image));

            Binding imageSourceBinding = new Binding {Converter = this, Mode = BindingMode.OneWay};
            imageElement.SetBinding(Image.SourceProperty, imageSourceBinding);

            Binding imageStretchBinding = new Binding {Source = imageStretch};
            imageElement.SetBinding(Image.StretchProperty, imageStretchBinding);

            DataTemplate template = new DataTemplate {VisualTree = imageElement};
            CellTemplate = template;
        }

        protected ImageGridViewColumn() : this(Stretch.None)
        {
        }

        #endregion
        
        // ##############################################################################################################################
        // IValueConverter
        // ##############################################################################################################################

        #region IValueConverter

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetImageSource(value);
        }
        
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected abstract ImageSource GetImageSource(object value);
        
        #endregion
    }
}