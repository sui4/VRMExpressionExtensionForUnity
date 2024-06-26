using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UniVRM10;

namespace VrmExpressionExtension
{
    [TrackBindingType(typeof(Vrm10Instance))]
    [TrackClipType(typeof(ExpressionClip))]
    [TrackColor(0.2f, 1.0f, 0.8f)]
    [DisplayName("VRM Expression Extension/Expression Track")]
    public class ExpressionTrack : TrackAsset, ILayerable
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<ExpressionMixerBehaviour> mixer =
                ScriptPlayable<ExpressionMixerBehaviour>.Create(graph, inputCount);

            ExpressionTrack baseTrack = parent.GetType() == typeof(ExpressionTrack) ? parent as ExpressionTrack : this;

            var bindingVrmInstance = GetBindingComponent<Vrm10Instance>(baseTrack, go);

            foreach (TimelineClip clip in GetClips())
            {
                var asset = clip.asset as ExpressionClip;
                if (asset == null) continue;

                asset.VrmObjectExpression = bindingVrmInstance == null ? null : bindingVrmInstance.Vrm.Expression;
            }

            return mixer;
        }

        public Playable CreateLayerMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<ExpressionLayerMixerBehaviour> mixer =
                ScriptPlayable<ExpressionLayerMixerBehaviour>.Create(graph, inputCount);
            mixer.GetBehaviour().VrmInstance = GetBindingComponent<Vrm10Instance>(this, go);
            return mixer;
        }

        private static T GetBindingComponent<T>(TrackAsset asset, GameObject gameObject) where T : class
        {
            if (gameObject == null) return default;

            var director = gameObject.GetComponent<PlayableDirector>();
            if (director == null) return default;

            var binding = director.GetGenericBinding(asset) as T;

            return binding switch
            {
                { } component => component,
                _ => default
            };
        }
    }
}