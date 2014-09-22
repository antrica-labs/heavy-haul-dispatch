using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SingerDispatch.Controls.Validators
{
    public class SelectionExistsValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Item not found");

            return new ValidationResult(true, null);
        }
    }
}
