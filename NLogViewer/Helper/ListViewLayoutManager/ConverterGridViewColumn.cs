using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace DJ.Helper.ListViewLayoutManager
{
    public abstract class ConverterGridViewColumn : GridViewColumn, IValueConverter
    {
        public Type BindingType => _BindingType;
        private readonly Type _BindingType;

        // ##############################################################################################################################
        // Constructor
        // ##############################################################################################################################

        #region Constructor

        protected ConverterGridViewColumn(Type bindingType)
        {
            if (bindingType == null)
            {
                throw new ArgumentNullException(nameof(bindingType));
            }

            this._BindingType = bindingType;

            Binding binding = new Binding {Mode = BindingMode.OneWay, Converter = this};
            DisplayMemberBinding = binding;
        }

        #endregion

        // ##############################################################################################################################
        // IValueConverter
        // ##############################################################################################################################

        #region IValueConverter

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!_BindingType.IsInstanceOfType(value))
            {
                throw new InvalidOperationException();
            }

            return ConvertValue(value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected abstract object ConvertValue(object value);
        
        #endregion
    }
}