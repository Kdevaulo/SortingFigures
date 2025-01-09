using UnityEngine;

using VContainer;

namespace Kdevaulo.SortingFigures
{
    public class CameraUtilities
    {
        [Inject]
        private Camera _camera;

        public Vector2 ScreenToWorldPoint(Vector2 position)
        {
            return _camera.ScreenToWorldPoint(position);
        }
    }
}