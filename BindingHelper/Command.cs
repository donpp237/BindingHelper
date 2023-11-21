using System;
using System.Windows.Input;
using System.Collections.Generic;

namespace BindingHelper
{
    class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Func<bool> m_canExecute;
        private Action<object> m_execute;

        public DelegateCommand(Action<object> exectue) : this(exectue, null) { }

        public DelegateCommand(Action<object> execute, Func<bool> canExecute)
        {
            this.m_execute = execute;
            this.m_canExecute = canExecute;
        }

        /// <summary>
        /// implement of icommand can execute method
        /// </summary>
        /// <param name="o">parameter by default of icomand interface</param>
        /// <returns>can execute or not</returns>
        public bool CanExecute(object o)
        {
            if (this.m_canExecute == null)
                return true;

            return this.m_canExecute();
        }

        /// <summary>
        /// implement of icommand interface execute method
        /// </summary>
        /// <param name="o">parameter by default of icomand interface</param>
        public void Execute(object o)
        {
            this.m_execute(o);
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
        private Dictionary<string, ICommand> m_commands;

        private string m_classType;

        public Command(object instance)
        {
            m_classType = instance.GetType().ToString();
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
                command = new DelegateCommand(action);
                m_commands.Add(commandName, command);
            }

            return command;
        }

        private string GetCommandName(string memberName)
        {
            return $"{m_classType}.{memberName}";
        }
    }
}
