using System;
using System.Reflection;
using System.Windows.Input;
using System.Collections.Generic;

namespace BindingHelper
{
    class WeakCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private WeakReference m_canExecute;
        private WeakReference m_execute;

        private Action m_actOnDeleteExecute;

        public WeakCommand(Action<object> exectue, Action actOnDeleteExecute) : this(exectue, null, actOnDeleteExecute) { }

        public WeakCommand(Action<object> execute, Func<bool> canExecute, Action actOnDeleteExecute)
        {
            this.m_execute = new WeakReference(execute);
            this.m_canExecute = new WeakReference(canExecute);
            this.m_actOnDeleteExecute = actOnDeleteExecute;
        }

        /// <summary>
        /// implement of icommand can execute method
        /// </summary>
        /// <param name="o">parameter by default of icomand interface</param>
        /// <returns>can execute or not</returns>
        public bool CanExecute(object o)
        {
            WeakReference refAction = m_canExecute;
            if (refAction == null)
                return true;

            if (refAction.Target == null)
                return true;

            Func<bool> canExecute = (Func<bool>)refAction.Target;
            if (canExecute == null)
                return true;

            return canExecute.Invoke();
        }

        /// <summary>
        /// implement of icommand interface execute method
        /// </summary>
        /// <param name="o">parameter by default of icomand interface</param>
        public void Execute(object o)
        {
            WeakReference refAction = m_execute;
            if ((refAction == null)
                || (refAction.Target == null))
            {
                m_actOnDeleteExecute?.Invoke();
                return;
            }

            Action<object> execute = (Action<object>)refAction.Target;
            if (execute == null)
                return;

            execute(o);
        }

        /// <summary>
        /// raise ca excute changed when property changed
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }

    internal class Command : IDisposable
    {
        private object m_instance;
        private Dictionary<string, ICommand> m_commands;

        public Command(object instance)
        {
            m_instance = instance;
            m_commands = new Dictionary<string, ICommand>();
        }

        public void Dispose()
        {
            if (m_commands == null)
                return;

            m_commands.Clear();
            m_commands = null;
        }

        public ICommand GetCommand(Action<object> action, string memberName)
        {
            ICommand command = null;
            string commandName = GetCommandName(memberName);
            if (m_commands.TryGetValue(commandName, out command) == false)
            {
                command = new WeakCommand(action, ()=>ReAllocateDelegate(memberName));
                m_commands.Add(commandName, command);
            }

            return command;
        }

        private string GetCommandName(string memberName)
        {
            return $"{m_instance.GetType().ToString()}.{memberName}";
        }

        private void ReAllocateDelegate(string memberName)
        {
            string commandName = GetCommandName(memberName);
            if (m_commands.ContainsKey(commandName) == true)
                m_commands.Remove(commandName);

            Type type = m_instance.GetType();
            PropertyInfo property = type.GetProperty(memberName);
            property.GetValue(m_instance);
        }
    }
}
