using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BindingHelper
{
    public class Helper : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Property m_propertyHelper;
        private Command m_commandHelper;

        public Helper()
        {
            m_propertyHelper = new Property(this);
            m_commandHelper = new Command(this);
        }

        public void Dispose()
        {
            if (m_propertyHelper != null)
                m_propertyHelper.Dispose();

            if (m_commandHelper != null)
                m_commandHelper.Dispose();

            m_propertyHelper = null;
            m_commandHelper = null;
        }

        protected void OnPropertyChanged(string memberName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        public T GetProperty<T>([CallerMemberName] string memberName = "")
        {
            return m_propertyHelper.GetProperty<T>(memberName);
        }

        public void SetProperty<T>(T value,
                                   [CallerMemberName] string memberName = "")
        {
            m_propertyHelper.SetProperty(value, memberName);
            OnPropertyChanged(memberName);
        }

        public ICommand GetCommand(Action<object> action,
                                   [CallerMemberName] string memberName = "")
        {
            return m_commandHelper.GetCommand(action, memberName);
        }
    }
}