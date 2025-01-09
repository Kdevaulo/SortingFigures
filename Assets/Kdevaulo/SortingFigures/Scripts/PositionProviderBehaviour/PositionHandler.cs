using UnityEngine;

namespace Kdevaulo.SortingFigures.PositionProviderBehaviour
{
    [AddComponentMenu(nameof(PositionHandler) + " in " + nameof(SortingFigures))]
    public class PositionHandler : MonoBehaviour
    {
        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}