using System;

using HereticalSolutions.Delegates;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Synchronization
{
	public class SynchronizationContextGeneric<TDelta>
		: ISynchronizableGenericArg<TDelta>,
		  ISynchronizationProvider,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IStronglyTypedMetadata metadata;

		private readonly IPublisherSingleArgGeneric<TDelta> broadcasterAsPublisher;

		private readonly INonAllocSubscribable broadcasterAsSubscribable;


		private SynchronizationDescriptor descriptor;

		public SynchronizationContextGeneric(
			SynchronizationDescriptor descriptor,
			IStronglyTypedMetadata metadata,
			IPublisherSingleArgGeneric<TDelta> broadcasterAsPublisher,
			INonAllocSubscribable broadcasterAsSubscribable)
		{
			this.descriptor = descriptor;

			this.metadata = metadata;

			this.broadcasterAsPublisher = broadcasterAsPublisher;

			this.broadcasterAsSubscribable = broadcasterAsSubscribable;


			if (metadata.Has<IHasFixedDelta<TDelta>>())
			{
				var fixedDeltaMetadata = metadata.Get<IHasFixedDelta<TDelta>>();

				((IPublisherDependencyRecipient<TDelta>)fixedDeltaMetadata).BroadcasterAsPublisher = broadcasterAsPublisher;
			}
		}

		#region ISynchronizableGenericArg

		#region ISynchronizable

		public SynchronizationDescriptor Descriptor { get => descriptor; }

		public IStronglyTypedMetadata Metadata { get => metadata; }

		#endregion

		public void Synchronize(TDelta delta)
		{
			if (metadata.Has<ITogglable>())
			{
				var togglable = metadata.Get<ITogglable>();

				if (!togglable.Active)
				{
					return;
				}
			}

			TDelta deltaActual = delta;

			if (metadata.Has<IScalable<TDelta>>())
			{
				var scalable = metadata.Get<IScalable<TDelta>>();

				deltaActual = scalable.Scale(deltaActual);
			}

			if (metadata.Has<IHasFixedDelta<TDelta>>())
			{
				var fixedDeltaMetadata = metadata.Get<IHasFixedDelta<TDelta>>();

				fixedDeltaMetadata.Tick(deltaActual);

				return;
			}

			broadcasterAsPublisher.Publish(deltaActual);
		}

		#endregion

		#region ISynchronizationProvider

		public void Subscribe(INonAllocSubscription subscription)
		{
			broadcasterAsSubscribable.Subscribe(
				subscription);
		}

		public void Unsubscribe(INonAllocSubscription subscription)
		{
			broadcasterAsSubscribable.Unsubscribe(
				subscription);
		}

		public void UnsubscribeAll()
		{
			broadcasterAsSubscribable.UnsubscribeAll();
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (metadata is ICleanuppable)
				(metadata as ICleanuppable).Cleanup();

			if (broadcasterAsPublisher is ICleanuppable)
				(broadcasterAsPublisher as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (metadata is IDisposable)
				(metadata as IDisposable).Dispose();

			if (broadcasterAsPublisher is IDisposable)
				(broadcasterAsPublisher as IDisposable).Dispose();
		}

		#endregion
	}
}