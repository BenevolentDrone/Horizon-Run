using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyDelegate
		: IClonable
	{
		private readonly object target;

		private readonly bool isInstanceMethod;

		private readonly Type instanceType;

		private readonly bool isAsync;

		private readonly Type returnType;

		private readonly int argumentsCount;

		private readonly Type[] argumentTypes;

		private readonly MethodInfo methodInfo;

		private readonly bool awaitCompletion;


		public object Target => target;
		
		public bool IsInstanceMethod => isInstanceMethod;

		public Type InstanceType => instanceType;

		public bool IsAsync => isAsync;

		public Type ReturnType => returnType;

		public int ArgumentsCount => argumentsCount;

		public Type[] ArgumentTypes => argumentTypes;

		public MethodInfo MethodInfo => methodInfo;

		public bool AwaitCompletion => awaitCompletion;

		public StanleyDelegate(
			object target,
			MethodInfo methodInfo,
			bool awaitCompletion)
		{
			this.target = target;

			this.methodInfo = methodInfo;

			this.awaitCompletion = awaitCompletion;

			if (methodInfo == null)
			{
				#region Null initialization

				isInstanceMethod = false;

				instanceType = null;

				isAsync = false;

				returnType = null;

				argumentsCount = 0;

				argumentTypes = Array.Empty<Type>();

				#endregion

				return;
			}

			#region MethodInfo initialization

			// For lambdas, we need special handling
			bool isCompilerGenerated =
				methodInfo
					.DeclaringType
					.GetCustomAttribute<CompilerGeneratedAttribute>()
				!= null;

			if (isCompilerGenerated)
			{
				isInstanceMethod = true;
				
				bool isTaskFactory =
					(methodInfo.ReturnType.IsGenericType
					 && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
					|| methodInfo.ReturnType == typeof(Task);

				// For compiler generated methods (like lambdas), we need to check if it's a Task-returning delegate
				if (isTaskFactory)
				{
					isAsync = true;
				}
				else
				{
					isAsync = IsAsyncMethod(methodInfo);
				}
			}
			else
			{
				// For regular methods, just check if it's static
				isInstanceMethod = !methodInfo.IsStatic;

				isAsync = IsAsyncMethod(methodInfo);
			}

			instanceType = methodInfo.DeclaringType;

			if (isAsync)
			{
				if (methodInfo.ReturnType.GenericTypeArguments.Length > 0) //Task<T>
					returnType = methodInfo.ReturnType.GenericTypeArguments[0]; //T
				else
					returnType = typeof(void);
			}
			else
			{
				returnType = methodInfo.ReturnType;
			}

			// Get parameters from methodInfo for all other cases
			argumentsCount = methodInfo.GetParameters().Length;
			
			argumentTypes = new Type[argumentsCount];
			
			for (int i = 0; i < argumentsCount; i++)
			{
				argumentTypes[i] = methodInfo.GetParameters()[i].ParameterType;
			}

			#endregion
		}

		//Courtesy of https://stackoverflow.com/questions/20350397/how-can-i-tell-if-a-c-sharp-method-is-async-await-via-reflection
		private static bool IsAsyncMethod(
			MethodInfo method)
		{
			Type attType = typeof(AsyncStateMachineAttribute);

			// Obtain the custom attribute for the method. 
			// The value returned contains the StateMachineType property. 
			// Null is returned if the attribute isn't present for the method. 
			var attrib = (AsyncStateMachineAttribute)method.GetCustomAttribute(attType);

			return (attrib != null);
		}

		#region IClonable

		public object Clone()
		{
			return StanleyFactory.BuildStanleyDelegate(
				target,
				methodInfo,
				awaitCompletion);
		}

		#endregion
	}
}