using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodingConnected.Composition.Example.WPF.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public void RaisePropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
