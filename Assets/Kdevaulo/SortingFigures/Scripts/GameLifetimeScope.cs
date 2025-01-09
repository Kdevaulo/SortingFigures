using System.Threading;

using Kdevaulo.SortingFigures.DraggableItemsBehaviour;
using Kdevaulo.SortingFigures.PositionProviderBehaviour;
using Kdevaulo.SortingFigures.SoundsBehaviour;

using UnityEngine;

using VContainer;
using VContainer.Unity;

using Animator = Kdevaulo.SortingFigures.DraggableItemsBehaviour.Animator;

namespace Kdevaulo.SortingFigures
{
    [AddComponentMenu(nameof(GameLifetimeScope) + " in " + nameof(SortingFigures))]
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Utilities")]
        [SerializeField] private Camera _camera;

        [Header("Spawn")]
        [SerializeField] private ItemsSettings _itemsSettings;
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private DraggableItem _itemPrefab;
        [SerializeField] private PositionsProvider[] _positionsProviders;

        [Header("Animation")]
        [SerializeField] private GameplaySettings _gameplaySettings;

        [Header("Sounds")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SoundsContainer _soundsContainer;

        private CancellationTokenSource _cts;

        protected override void Configure(IContainerBuilder builder)
        {
            _cts = new CancellationTokenSource();

            builder.RegisterInstance(_cts);

            builder.RegisterComponent(_camera);
            builder.RegisterComponent(_itemPrefab);
            builder.RegisterComponent(_audioSource);
            builder.RegisterComponent(_itemsSettings);
            builder.RegisterComponent(_itemsContainer);
            builder.RegisterComponent(_soundsContainer);
            builder.RegisterComponent(_gameplaySettings);
            builder.RegisterComponent(_positionsProviders);

            builder.Register<Animator>(Lifetime.Scoped);
            builder.Register<AudioService>(Lifetime.Scoped);
            builder.Register<ItemsFactory>(Lifetime.Scoped);
            builder.Register<CameraUtilities>(Lifetime.Scoped);
            builder.Register<UserActionsHandler>(Lifetime.Scoped);

            builder.RegisterEntryPoint<ItemsBehaviourHandler>();
        }
    }
}