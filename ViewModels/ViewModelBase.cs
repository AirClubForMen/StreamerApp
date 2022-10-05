using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamerApp.ViewModels
{
    /// <summary>
    /// Handles the Onproperty Changed Events
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// If called with no parameters it uses the caller name from the setter otherwise it uses the value passed
        ///   Can be overridden if need be.
        /// </summary>
        /// <param name="propertyName">Property name to refresh</param>
        protected virtual void PropChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
