using System.ComponentModel;
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
            ScriptPlayable<ExpressionMixerBehaviour> mixer = ScriptPlayable<ExpressionMixerBehaviour>.Create(graph, inputCount);
            
            var bindingVrmInstance = GetBindingComponent<Vrm10Instance>(this, go);
            // rename clip to preset name
            foreach (TimelineClip clip in GetClips())
            {
                RenameClip(clip);
                var asset = clip.asset as ExpressionClip;
                if (asset == null || bindingVrmInstance == null) continue;
                asset.VrmObjectExpression = bindingVrmInstance.Vrm.Expression;
            }
            return mixer;
        }

        public Playable CreateLayerMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<ExpressionLayerMixerBehaviour> mixer = ScriptPlayable<ExpressionLayerMixerBehaviour>.Create(graph, inputCount);
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

        private static void RenameClip(TimelineClip clip)
        {
            var asset = clip.asset as ExpressionClip;
            if (asset == null) return;
            clip.displayName = asset._template.Preset + " ( " + asset._template.Weight + " )";
        }
    }
}
