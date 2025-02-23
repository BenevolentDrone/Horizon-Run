using System;
using System.Reflection;

namespace HereticalSolutions.StanleyScript
{
	public interface IVariableImporter
	{
		public bool ImportVariable(
			string objectName,
			IStanleyVariable variable);

		public void ImportOrUpdateVariable(
			string objectName,
			IStanleyVariable variable);

		public bool ImportValueVariable<TValue>(
			string objectName,
			string variableName,
			TValue value,
			out IStanleyVariable variable);

		public bool ImportValueVariable(
			string objectName,
			string variableName,
			Type variableType,
			object value,
			out IStanleyVariable variable);

		public bool ImportPollableVariable<TValue>(
			string objectName,
			string variableName,
			Func<object> getter,
			Action<object> setter,
			out IStanleyVariable variable);

		public bool ImportPollableVariable(
			string objectName,
			string variableName,
			Type variableType,
			Func<object> getter,
			Action<object> setter,
			out IStanleyVariable variable);

		public bool ImportObject(
			string objectName,
			out IStanleyVariable variable);
		
		public bool ImportDelegate(
			string objectName,
			string variableName,
			object target,
			MethodInfo methodInfo,
			bool awaitCompletion,
			out IStanleyVariable variable);

		public bool ImportEvent(
			string objectName,
			string variableName,
			Func<bool> poller,
			bool jumpToLabel,
			string label,
			out IStanleyVariable variable);

		public bool TryExportVariable(
			string objectName,
			string name,
			out IStanleyVariable variable);

		public bool TryRemoveVariable(
			string objectName,
			string variableName);
	}
}