using System.Linq;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Assertions;

using VContainer;

namespace Kdevaulo.SortingFigures.SoundsBehaviour
{
    public class AudioService
    {
        [Inject] private AudioSource _audioSource;
        [Inject] private SoundsContainer _soundsContainer;

        [Inject] private CancellationTokenSource _cts;

        public void PlayOneShot(Sound sound)
        {
            Assert.IsTrue(_soundsContainer.Clips.Any(x => x.SoundType == sound));

            var clipByType = _soundsContainer.Clips.First(x => x.SoundType == sound);

            PlayOneShotAsync(clipByType.Clip, _cts.Token).Forget();
        }

        private async UniTask PlayOneShotAsync(AudioClip clip, CancellationToken token)
        {
            _audioSource.PlayOneShot(clip);
            await UniTask.WaitForSeconds(clip.length, cancellationToken: token);
        }
    }
}