using System;
using System.Collections.Generic;
using System.Reflection;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class ScopeSymbolTable
		: ISymbolTable,
		  IVariableImporter,
		  IVariableEventProcessor,
		  IClonable
	{
		private readonly StanleyScope parentScope;

		private readonly IRepository<string, IStanleyVariable> variables;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public ScopeSymbolTable(
			StanleyScope parentScope,
			IRepository<string, IStanleyVariable> variables,
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.parentScope = parentScope;

			this.variables = variables;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region ISymbolTable

		#region IReadOnlySymbolTable

		public bool TryGetVariable(
			string name,
			out IStanleyVariable variable)
		{
			if (variables.TryGet(
				name,
				out variable))
			{
				return true;
			}

			if (parentScope != null)
			{
				return parentScope.Variables.TryGetVariable(
					name,
					out variable);
			}

			return false;
		}

		public IEnumerable<IStanleyVariable> AllVariables => variables.Values;

		#endregion

		public bool TryAddVariable(
			IStanleyVariable variable)
		{
			bool result = variables.TryAdd(
				variable.Name,
				variable);

			if (result)
			{
				ProcessVariableAddedEvent(
					variable);
			}

			return result;
		}

		public void AddOrUpdateVariable(
			IStanleyVariable variable)
		{
			if (TryGetVariable(
				variable.Name,
				out var previousVariable))
			{
				ProcessVariableRemovedEvent(
					previousVariable);
			}

			variables.AddOrUpdate(
				variable.Name,
				variable);

			ProcessVariableAddedEvent(
				variable);
		}

		public bool TryRemoveVariable(
			string variableName)
		{
			if (TryGetVariable(
				variableName,
				out var variable))
			{
				ProcessVariableRemovedEvent(
					variable);
			}

			return variables.TryRemove(
				variableName);
		}

		#endregion

		#region IVariableImporter

		public bool ImportVariable(
			string objectName,
			IStanleyVariable variable)
		{
			if (string.IsNullOrEmpty(objectName))
			{
				return TryAddVariable(
					variable);
			}

			return TryAddVariableToObject(
				objectName,
				variable);
		}

		public void ImportOrUpdateVariable(
			string objectName,
			IStanleyVariable variable)
		{
			if (string.IsNullOrEmpty(objectName))
			{
				AddOrUpdateVariable(
					variable);
			}

			AddOrUpdateVariableToObject(
				objectName,
				variable);
		}

		public bool ImportValueVariable<TValue>(
			string objectName,
			string variableName,
			TValue value,
			out IStanleyVariable variable)
		{
			variable = StanleyFactory.BuildValueVariable(
				variableName,
				typeof(TValue),
				value,
				loggerResolver);

			return ImportVariable(
				objectName,
				variable);
		}

		public bool ImportValueVariable(
			string objectName,
			string variableName,
			Type variableType,
			object value,
			out IStanleyVariable variable)
		{
			variable = StanleyFactory.BuildValueVariable(
				variableName,
				variableType,
				value,
				loggerResolver);

			return ImportVariable(
				objectName,
				variable);
		}

		public bool ImportPollableVariable<TValue>(
			string objectName,
			string variableName,
			Func<object> getter,
			Action<object> setter,
			out IStanleyVariable variable)
		{
			variable = StanleyFactory.BuildPollableVariable(
				variableName,
				typeof(TValue),
				getter,
				setter,
				loggerResolver);

			return ImportVariable(
				objectName,
				variable);
		}

		public bool ImportPollableVariable(
			string objectName,
			string variableName,
			Type variableType,
			Func<object> getter,
			Action<object> setter,
			out IStanleyVariable variable)
		{
			variable = StanleyFactory.BuildPollableVariable(
				variableName,
				variableType,
				getter,
				setter,
				loggerResolver);

			return ImportVariable(
				objectName,
				variable);
		}

		public bool ImportObject(
			string objectName,
			out IStanleyVariable variable)
		{
			variable = StanleyFactory.BuildValueVariable(
				objectName,
				typeof(StanleyObject),
				StanleyFactory.BuildStanleyObject(),
				loggerResolver);

			return ImportVariable(
				string.Empty,
				variable);
		}
		
		public bool ImportDelegate(
			string objectName,
			string variableName,
			object target,
			MethodInfo methodInfo,
			bool awaitCompletion,
			out IStanleyVariable variable)
		{
			variable = StanleyFactory.BuildValueVariable(
				variableName,
				typeof(StanleyDelegate),
				StanleyFactory.BuildStanleyDelegate(
					target,
					methodInfo,
					awaitCompletion),
				loggerResolver);

			return ImportVariable(
				objectName,
				variable);
		}

		public bool ImportEvent(
			string objectName,
			string variableName,
			Func<bool> poller,
			bool jumpToLabel,
			string label,
			out IStanleyVariable variable)
		{
			variable = StanleyFactory.BuildValueVariable(
				variableName,
				typeof(StanleyEvent),
				StanleyFactory.BuildStanleyEvent(
					poller,
					jumpToLabel,
					label),
				loggerResolver);

			return ImportVariable(
				objectName,
				variable);
		}

		public bool TryExportVariable(
			string objectName,
			string variableName,
			out IStanleyVariable variable)
		{
			if (string.IsNullOrEmpty(objectName))
			{
				return TryGetVariable(
					variableName,
					out variable);
			}

			return TryGetVariableFromObject(
				objectName,
				variableName,
				out variable);
		}

		public bool TryRemoveVariable(
			string objectName,
			string variableName)
		{
			if (string.IsNullOrEmpty(objectName))
			{
				return TryRemoveVariable(
					variableName);
			}

			return TryRemoveVariableFromObject(
				objectName,
				variableName);
		}

		#endregion

		#region IVariableEventProcessor

		public void ProcessVariableAddedEvent(
			IStanleyVariable variable)
		{
			var objectValue = variable.GetValue();

			switch (objectValue)
			{
				case StanleyEvent stanleyEvent:
				{
					if (TryGetVariableFromObject(
						StanleyConsts.SCOPE_OBJECT_NAME,
						StanleyConsts.SCOPE_EVENT_LIST_VARIABLE_NAME,
						out var eventListVariable))
					{
						var eventList = eventListVariable.GetValue<StanleyList>();

						eventList.Push(
							variable);
					}

					break;
				}
			}
		}

		public void ProcessVariableRemovedEvent(
			IStanleyVariable variable)
		{
			var objectValue = variable.GetValue();

			switch (objectValue)
			{
				case StanleyEvent stanleyEvent:
				{
					if (TryGetVariableFromObject(
						StanleyConsts.SCOPE_OBJECT_NAME,
						StanleyConsts.SCOPE_EVENT_LIST_VARIABLE_NAME,
						out var eventListVariable))
					{
						var eventList = eventListVariable.GetValue<StanleyList>();

						eventList.Remove(
							variable);
					}

					break;
				}

				case StanleyObject stanleyObject:
				{
					foreach (var property in stanleyObject.Properties.AllVariables)
					{
						ProcessVariableRemovedEvent(
							property);
					}

					break;
				}
			}
		}

		#endregion

		private bool TryAddVariableToObject(
			string objectName,
			IStanleyVariable variable)
		{
			if (!TryGetVariable(
				objectName,
				out var objectVariable))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND OBJECT {objectName}");

				return false;
			}

			if (!objectVariable.VariableType.IsSameOrInheritor(typeof(StanleyObject)))
			{
				logger?.LogError(
					GetType(),
					$"VARIABLE {objectName} IS NOT AN OBJECT");

				return false;
			}

			var objectValue = objectVariable.GetValue<StanleyObject>();

			bool result = objectValue.Properties.TryAddVariable(
				variable);

			if (result)
			{
				ProcessVariableAddedEvent(
					variable);
			}

			return result;
		}

		private void AddOrUpdateVariableToObject(
			string objectName,
			IStanleyVariable variable)
		{
			if (!TryGetVariable(
				objectName,
				out var objectVariable))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND OBJECT {objectName}");

				return;
			}

			if (!objectVariable.VariableType.IsSameOrInheritor(typeof(StanleyObject)))
			{
				logger?.LogError(
					GetType(),
					$"VARIABLE {objectName} IS NOT AN OBJECT");

				return;
			}

			var objectValue = objectVariable.GetValue<StanleyObject>();

			if (objectValue.Properties.TryGetVariable(
				variable.Name,
				out var previousVariable))
			{
				ProcessVariableRemovedEvent(
					previousVariable);
			}

			objectValue.Properties.AddOrUpdateVariable(
				variable);

			ProcessVariableAddedEvent(
				variable);
		}

		private bool TryGetVariableFromObject(
			string objectName,
			string variableName,
			out IStanleyVariable variable)
		{
			if (!TryGetVariable(
				objectName,
				out var objectVariable))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND OBJECT {objectName}");

				variable = null;

				return false;
			}

			if (!objectVariable.VariableType.IsSameOrInheritor(typeof(StanleyObject)))
			{
				logger?.LogError(
					GetType(),
					$"VARIABLE {objectName} IS NOT AN OBJECT");

				variable = null;

				return false;
			}

			var objectValue = objectVariable.GetValue<StanleyObject>();

			return objectValue.Properties.TryGetVariable(
				variableName,
				out variable);
		}

		private bool TryRemoveVariableFromObject(
			string objectName,
			string variableName)
		{
			if (!TryGetVariable(
				objectName,
				out var objectVariable))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND OBJECT {objectName}");

				return false;
			}

			if (!objectVariable.VariableType.IsSameOrInheritor(typeof(StanleyObject)))
			{
				logger?.LogError(
					GetType(),
					$"VARIABLE {objectName} IS NOT AN OBJECT");

				return false;
			}

			var objectValue = objectVariable.GetValue<StanleyObject>();

			if (objectValue.Properties.TryGetVariable(
				variableName,
				out var variable))
			{
				ProcessVariableRemovedEvent(
					variable);
			}

			return objectValue.Properties.TryRemoveVariable(
				variableName);
		}

		#region IClonable

		public object Clone()
		{
			var result = StanleyFactory.BuildScopeSymbolTable(
				parentScope,
				loggerResolver);

			TryGetVariable(
				StanleyConsts.SCOPE_OBJECT_NAME,
				out var sourceScopeObjectVariable);

			foreach (var variable in AllVariables)
			{
				if (variable == sourceScopeObjectVariable)
					continue;

				var clonableVariable = variable as IClonable;

				result.TryAddVariable(
					(IStanleyVariable)clonableVariable.Clone());
			}

			return result;
		}

		#endregion
	}
}