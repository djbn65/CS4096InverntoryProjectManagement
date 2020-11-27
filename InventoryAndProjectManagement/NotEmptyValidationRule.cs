using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace(GetBoundValue(value)?.ToString())
                ? new ValidationResult(false, "Field is required.")
                : ValidationResult.ValidResult;
        }

        private object GetBoundValue(object value)
        {
            // ValidationStep was UpdatedValue or CommittedValue (Validate after setting)
            // Need to pull the value out of the BindingExpression.
            if (value is BindingExpression binding)
            {
                // Get the bound object and name of the property
                object dataItem = binding.DataItem;

                if (dataItem == null) return "";

                string propertyName = binding.ParentBinding.Path.Path;

                // Extract the value of the property.
                object propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);

                // This is what we want.
                return propertyValue;
            }
            else
            {
                // ValidationStep was RawProposedValue or ConvertedProposedValue
                // The argument is already what we want!
                return value;
            }
        }
    }
}