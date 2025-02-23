using System;
using System.Collections.Generic;

namespace HereticalSolutions.StanleyScript
{
	//I HAVE A STACK
	//I HAVE A LIST
	//UGH
	//STANLEYLIST
	//(((applause)))
	public class StanleyList
		: IClonable
	{
		private readonly List<IStanleyVariable> list;

		public List<IStanleyVariable> List => list;

		public StanleyList(
			List<IStanleyVariable> list)
		{
			this.list = list;
		}

		public int Count => list.Count;

		public IStanleyVariable FirstValue
		{
			get
			{
				if (list.Count == 0)
					return null;
	
				return list[0];
			}
		}

		public IStanleyVariable LastValue
		{
			get
			{
				if (list.Count == 0)
					return null;
	
				return list[list.Count - 1];
			}
		}

		public IStanleyVariable this [int i]
		{
			get => GetValueAtIndex(i);
			set => SetValueAtIndex(
				i,
				value);
		}

		public IStanleyVariable GetValueAtIndex(
			int index)
		{
			if (index < 0 || index >= list.Count)
				throw new Exception(
					"INDEX OUT OF RANGE");

			return list[index];
		}

		public void SetValueAtIndex(
			int index,
			IStanleyVariable value)
		{
			if (index < 0 || index >= list.Count)
				throw new Exception(
					"INDEX OUT OF RANGE");

			list[index] = value;
		}

		public void Push(
			IStanleyVariable value)
		{
			list.Add(value);
		}

		public IStanleyVariable Peek()
		{
			if (list.Count == 0)
				return null;

			return list[list.Count - 1];
		}

		public IStanleyVariable Pop()
		{
			if (list.Count == 0)
				return null;

			IStanleyVariable value = list[list.Count - 1];

			list.RemoveAt(list.Count - 1);

			return value;
		}

		public void InsertAt(
			int index,
			IStanleyVariable value)
		{
			list.Insert(
				index,
				value);
		}

		public void Remove(
			IStanleyVariable value)
		{
			list.Remove(
				value);
		}

		public void RemoveAt(
			int index)
		{
			list.RemoveAt(
				index);
		}

		public static StanleyList Concat(
			StanleyList list1,
			StanleyList list2)
		{
			var result = StanleyFactory.BuildStanleyList();

			result.List.AddRange(
				list1.List);

			result.List.AddRange(
				list2.List);

			return result;
		}

		//This is done with delegates passed from the host environment
		/*
		public void Enumerate()
		{
		}
		*/

		public void Clear()
		{
			list.Clear();
		}

		#region IClonable

		public object Clone()
		{
			var result = StanleyFactory.BuildStanleyList();

			foreach (var value in list)
			{
				var clonableValue = value as IClonable;

				result.Push(
					(IStanleyVariable)clonableValue.Clone());
			}

			return result;
		}

		#endregion
	}
}