using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.MVVM.ViewModel
{
    public abstract class AViewModel
        : ALifetimeable,
          IViewModel
    {
        protected readonly IRepository<string, CommandDelegate> commands;

        protected readonly IRepository<string, CommandWithArgsDelegate> commandsWithArguments;

        protected readonly IRepository<string, object> observables;

        public AViewModel(
            IRepository<string, CommandDelegate> commands,
            IRepository<string, CommandWithArgsDelegate> commandsWithArguments,
            IRepository<string, object> observables,
            ILogger logger = null)
            : base (logger)
        {
            this.commands = commands;

            this.commandsWithArguments = commandsWithArguments;

            this.observables = observables;
        }

        //VMs should not be implementing a default IInitializable because they should be called Initialize(..) with their specific arguments
        //by other VMs or bootstrappers that DO know which VMs they're initializing and what arguments they need
        //
        //Views on the other side are not required to have specific arguments to be initialized, so they can implement a default IInitializable
        /*
        #region IIinitializable

        public virtual void Initialize(object[] args)
        {
            if (!IsSetUp)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        "VIEWMODEL SHOULD BE SET UP BEFORE BEING INITIALIZED"));
            }

            if (IsInitialized)
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"INITIALIZING VIEWMODEL THAT IS ALREADY INITIALIZED"));
            }

            InitializeInternal(args);

            IsInitialized = true;

            OnInitialized?.Invoke();
        }

        #endregion
        */

        #region IViewModel

        /// <summary>
        /// Tries to get an observable by its identifier
        /// </summary>
        /// <typeparam name="T">Generic type of the observable</typeparam>
        /// <param name="key">Identifier of the observable</param>
        /// <param name="observable">Observable object</param>
        /// <returns>Whether the observable is present in the exposed observables list</returns>
        public bool GetObservable<T>(
            string key,
            out IObservableProperty<T> observable)
        {
            bool result = observables.TryGet(
                key,
                out object output);

            observable = result
                ? (IObservableProperty<T>)output
                : null;

            return result;
        }
        
        /// <summary>
        /// Tries to get a command by its identifier
        /// </summary>
        /// <param name="key">Identifier of the command</param>
        /// <returns>Command delegate</returns>
        public CommandDelegate GetCommand(string key)
        {
            if (!commands.TryGet(
                key,
                out CommandDelegate result))
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EXPOSED COMMANDS LIST DOES NOT HAVE A KEY \"{key}\""));
            }

            return result;
        }
        
        /// <summary>
        /// Tries to get a command with arguments by its identifier
        /// </summary>
        /// <param name="key">Identifier of the command</param>
        /// <returns>Command delegate</returns>
        public CommandWithArgsDelegate GetCommandWithArguments(string key)
        {
            if (!commandsWithArguments.TryGet(
                key,
                out CommandWithArgsDelegate result))
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EXPOSED COMMANDS WITH ARGUMENTS LIST DOES NOT HAVE A KEY \"{key}\""));
            }

            return result;
        }

        #endregion

        protected override void CleanupInternal()
        {
            UnpublishObservables();

            UnpublishCommands();

            base.CleanupInternal();
        }

        /// <summary>
        /// Tries to poll the value of an observable property using the active poll delegate
        /// </summary>
        /// <param name="observableProperty">Observable property</param>
        /// <typeparam name="T">Value type of the observable property</typeparam>
        protected void TryPollObservable<T>(IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
                ((IValuePollable)observableProperty).PollValue();
        }

        /// <summary>
        /// Tears down the observable and cleans it up
        /// </summary>
        /// <param name="observableProperty">Observable property</param>
        /// <typeparam name="T">Value type of the observable property</typeparam>
        protected void TearDownObservable<T>(ref IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
            {
                observableProperty.Cleanup();

                observableProperty = null;
            }
        }

        /// <summary>
        /// Adds an observable to the list of exposed observables
        /// </summary>
        /// <param name="key">Identifier for the observable</param>
        /// <param name="observable">Observable object</param>
        protected void PublishObservable(
            string key,
            object observable)
        {
            if (observables.Has(key))
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EXPOSED OBSERVABLES LIST ALREADY HAS A KEY \"{key}\""));
            }

            observables.TryAdd(
                key,
                observable);
        }

        /// <summary>
        /// Cleans up the exposed observables list
        /// </summary>
        protected void UnpublishObservables()
        {
            observables.Clear();
        }

        /// <summary>
        /// Adds a delegate to the list of exposed commands
        /// </summary>
        /// <param name="key">Identifier for the command</param>
        /// <param name="delegate">Delegate object</param>
        protected void PublishCommand(string key, CommandDelegate @delegate)
        {
            if (commands.Has(key))
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EXPOSED COMMANDS LIST ALREADY HAS A KEY \"{key}\""));
            }

            commands.TryAdd(
                key,
                @delegate);
        }

        /// <summary>
        /// Adds a delegate to the list of exposed commands with arguments
        /// </summary>
        /// <param name="key">Identifier for the command</param>
        /// <param name="delegate">Delegate object</param>
        protected void PublishCommandWithArguments(
            string key,
            CommandWithArgsDelegate @delegate)
        {
            if (commandsWithArguments.Has(key))
            {
                throw new Exception(
                    logger.TryFormat(
                        GetType(),
                        $"EXPOSED COMMANDS WITH ARGUMENTS LIST ALREADY HAS A KEY \"{key}\""));
            }

            commandsWithArguments.TryAdd(
                key,
                @delegate);
        }

        /// <summary>
        /// Cleans up the exposed commands list
        /// </summary>
        protected void UnpublishCommands()
        {
            commands.Clear();

            commandsWithArguments.Clear();
        }
    }
}