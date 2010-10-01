using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SingerDispatch.Controls.Validators
{
    class SelectionExistValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var text = combo.Text;
            var selection = combo.SelectedItem;

            if (selection == null && text.Length > 0)
                return new ValidationResult(false, "Item not found");

            return new ValidationResult(true, null);
        }
    }
}
