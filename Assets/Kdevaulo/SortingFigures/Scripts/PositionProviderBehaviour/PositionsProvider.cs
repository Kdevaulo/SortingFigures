using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

namespace Kdevaulo.SortingFigures.PositionProviderBehaviour
{
    [AddComponentMenu(nameof(PositionsProvider) + " in " + nameof(SortingFigures))]
    public class PositionsProvider : MonoBehaviour
    {
        [SerializeField] private Figure _figureType;

        private PositionHandler[] _positionsHandlers;

        private void Awake()
        {
            _positionsHandlers = GetComponentsInChildren<PositionHandler>();
            Assert.IsFalse(_positionsHandlers.Length == 0);
        }

        public PositionsByType GetPositions()
        {
            return new PositionsByType()
            {
                FigureType = _figureType,
                Positions = _positionsHandlers.Select(x => x.GetPosition()).ToList()
            };
        }
    }
}