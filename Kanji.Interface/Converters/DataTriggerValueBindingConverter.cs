// Credits to Alex's C# and .NET blog
// http://alexcdotnet.blogspot.fr/2011/11/datatrigger-binding-to-value.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Kanji.Interface.Converters
{
    /// <summary>
    /// This class supports databinding to the "Value" property on a DataTrigger, which is not currently supported by WPF in .NET4.
    /// A MultiBinding is required to bind value1 to the comparer, and the second to the target value
    /// The DataTrigger is simply asked to expect a value of "True"
    /// Reference objects and enums will need to have their type passed in as a parameter, as a System.Object compare seems to not always be consistent
    /// </summary>
    public class DataTriggerValueBindingConverter : IMultiValueConverter
    {
        /// <summary>
        /// We are expecting two values, which will then return true or false
        /// </summary>
        /// <param name="values">This will expect two binding values, one for the comparer, and one for the target value</param>
        /// <param name="targetType">redundant</param>
        /// <param name="parameter">This optional parameter expects a </param>
        /// <param name="culture">redundant</param>
        /// <returns>returns true if the comparer matches the value</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() != 2)
                throw new InvalidOperationException("This converter expects only two bound values, the first is your comparer object and the second is the bound value. "
                + "Please check your Multibinding object.");

            if (parameter is Type)
            {
                if (values[0] != null && values[1] != null &&
                    values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
                {
                    //we are dynamically casting our System.Object to its apparent type passed in as the parameter
                    dynamic val1 = System.Convert.ChangeType(values[0], (Type)parameter);
                    dynamic val2 = System.Convert.ChangeType(values[1], (Type)parameter);

                    if (val1 == val2)
                        return true;
                }
            }
            else if (parameter != null)
                throw new System.InvalidOperationException("Only a parameter of System.Type can be recieved, " + parameter.GetType().ToString() + " is not supported");

            //If our parameter is null, we can assume that no type is required, and we are doing a primitive compare
            if (values[0] == values[1])
                return true;

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
