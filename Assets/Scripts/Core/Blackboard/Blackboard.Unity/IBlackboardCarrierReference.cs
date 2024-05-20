using System;

namespace HereticalSolutions.Blackboard
{
	public interface IBlackboardCarrierReference
		: IBlackboardCarrier
	{
		IBlackboardCarrier ReferencedCarrier { get; }

		Action<IBlackboardCarrierReference> OnReferenceModified { get; set; }
	}
}