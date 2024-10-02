using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerPositionRotation3DViewSystem : AEntitySetSystem<float>
    {
        public ServerPositionRotation3DViewSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<ServerPositionRotation3DViewComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            ref var serverPositionRotation3DViewComponent = ref entity.Get<ServerPositionRotation3DViewComponent>();

            if (!serverPositionRotation3DViewComponent.Visible)
            {
                serverPositionRotation3DViewComponent.PositionRotationTransform.gameObject.SetActive(false);
                
                return;
            }
            
            serverPositionRotation3DViewComponent.PositionRotationTransform.gameObject.SetActive(true);
            
            var currentServerPosition = serverPositionRotation3DViewComponent.ServerPosition;
            
            var lineRenderer = serverPositionRotation3DViewComponent.LineRenderer;

            if (lineRenderer.positionCount != 2)
            {
                // set the color of the line
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;

                // set width of the renderer
                lineRenderer.startWidth = serverPositionRotation3DViewComponent.LineWidth;
                lineRenderer.endWidth = serverPositionRotation3DViewComponent.LineWidth;
                
                lineRenderer.positionCount = 2;
            }

            Vector3 paddingVector = new Vector3(0f, serverPositionRotation3DViewComponent.Padding, 0f);

            Vector3 currentPosition = serverPositionRotation3DViewComponent.Position;

            lineRenderer.SetPosition(0, currentPosition + paddingVector);
            lineRenderer.SetPosition(1, currentServerPosition + paddingVector);
            
            if (!serverPositionRotation3DViewComponent.Dirty)
            {
                return;
            }

            serverPositionRotation3DViewComponent.PositionRotationTransform.position =
                currentServerPosition + paddingVector;
            
            serverPositionRotation3DViewComponent.PositionRotationTransform.rotation =
                serverPositionRotation3DViewComponent.ServerRotation;

            serverPositionRotation3DViewComponent.Dirty = false;
        }
    }
}