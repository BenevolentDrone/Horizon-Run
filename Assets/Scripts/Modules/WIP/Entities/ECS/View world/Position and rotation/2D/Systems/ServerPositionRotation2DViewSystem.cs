using DefaultEcs;
using DefaultEcs.System;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerPositionRotation2DViewSystem : AEntitySetSystem<float>
    {
        public ServerPositionRotation2DViewSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<ServerPositionRotation2DViewComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            ref var serverPositionRotation2DViewComponent = ref entity.Get<ServerPositionRotation2DViewComponent>();

            if (!serverPositionRotation2DViewComponent.Visible)
            {
                serverPositionRotation2DViewComponent.PositionRotationTransform.gameObject.SetActive(false);
                
                return;
            }
            
            serverPositionRotation2DViewComponent.PositionRotationTransform.gameObject.SetActive(true);
            
            var currentServerPositionWithHeightAdjusted = new Vector3(
                serverPositionRotation2DViewComponent.ServerPosition.x,
                serverPositionRotation2DViewComponent.Position.y,
                serverPositionRotation2DViewComponent.ServerPosition.y);
            
            var lineRenderer = serverPositionRotation2DViewComponent.LineRenderer;

            if (lineRenderer.positionCount != 2)
            {
                // set the color of the line
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;

                // set width of the renderer
                lineRenderer.startWidth = serverPositionRotation2DViewComponent.LineWidth;
                lineRenderer.endWidth = serverPositionRotation2DViewComponent.LineWidth;
                
                lineRenderer.positionCount = 2;
            }

            Vector3 paddingVector = new Vector3(0f, serverPositionRotation2DViewComponent.Padding, 0f);
            
            Vector3 currentPosition3D = new Vector3(
                serverPositionRotation2DViewComponent.Position.x,
                0f,
                serverPositionRotation2DViewComponent.Position.y);

            lineRenderer.SetPosition(0, currentPosition3D + paddingVector);
            lineRenderer.SetPosition(1, currentServerPositionWithHeightAdjusted + paddingVector);
            
            if (!serverPositionRotation2DViewComponent.Dirty)
            {
                return;
            }

            serverPositionRotation2DViewComponent.PositionRotationTransform.position =
                currentServerPositionWithHeightAdjusted + paddingVector;
            
            serverPositionRotation2DViewComponent.PositionRotationTransform.localEulerAngles = new Vector3(
                0f,
                0f,
                serverPositionRotation2DViewComponent.ServerRotation);

            serverPositionRotation2DViewComponent.Dirty = false;
        }
    }
}