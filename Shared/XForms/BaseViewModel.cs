using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ColorControl.Shared.XForms;

public abstract class BaseViewModel : IDataErrorInfo, INotifyPropertyChanged
{
    public virtual string Error => null;
    public bool IsValid { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

    public virtual string this[string columnName]
    {
        get
        {
            var validationResults = new List<ValidationResult>();

            var fieldValid = Validator.TryValidateProperty(GetType().GetProperty(columnName).GetValue(this)
                    , new ValidationContext(this)
                    {
                        MemberName = columnName
                    }
                    , validationResults);

            bool isValid;
            if (fieldValid)
            {
                var allValidationResults = new List<ValidationResult>();
                isValid = PreValidate() && Validator.TryValidateObject(this, new ValidationContext(this), allValidationResults, true);
            }
            else
            {
                isValid = false;
            }
            if (isValid != IsValid)
            {
                IsValid = isValid;
                OnPropertyChanged("IsValid");
            }

            return fieldValid ? null : validationResults.First().ErrorMessage;
        }
    }

    public virtual bool PreValidate()
    {
        return true;
    }
}
