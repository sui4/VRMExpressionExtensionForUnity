using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UniVRM10;

namespace VrmExpressionExtension
{
    public class ExpressionLayerMixerBehaviour : PlayableBehaviour
    {
        private readonly Dictionary<ExpressionKey, float> _weightSum = new();
        public Vrm10Instance VrmInstance { get; set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var vrmInstance = playerData as Vrm10Instance;
            if (vrmInstance == null) return;
            VrmInstance = vrmInstance;

            _weightSum.Clear();

            int inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<ExpressionMixerBehaviour>)playable.GetInput(i);
                ExpressionMixerBehaviour mixer = inputPlayable.GetBehaviour();
                foreach ((ExpressionKey key, float weight) in mixer.Weight)
                {
                    if (_weightSum.TryGetValue(key, out float curWeight))
                    {
                        _weightSum[key] = curWeight + weight * inputWeight;
                    }
                    else
                    {
                        _weightSum.Add(key, weight * inputWeight);
                    }
                }
            }

            var presets = (ExpressionPreset[])System.Enum.GetValues(typeof(ExpressionPreset));

            // 未使用のキーのWeightを0にするために全てのキーを処理
            foreach (ExpressionPreset preset in presets)
            {
                if (preset == ExpressionPreset.custom) continue;

                var key = new ExpressionKey(preset);
                float weight = _weightSum.TryGetValue(key, out float w) ? w : 0f;
                vrmInstance.Runtime.Expression.SetWeight(key, weight);
            }

            vrmInstance.Vrm.Expression.CustomClips.ForEach(clip =>
            {
                var key = new ExpressionKey(ExpressionPreset.custom, clip.name);
                float weight = _weightSum.TryGetValue(key, out float w) ? w : 0f;
                vrmInstance.Runtime.Expression.SetWeight(key, weight);
            });

            if (!Application.isPlaying)
            {
                vrmInstance.Runtime.Process();
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (VrmInstance == null) return;

            var presets = (ExpressionPreset[])System.Enum.GetValues(typeof(ExpressionPreset));

            foreach (ExpressionPreset preset in presets)
            {
                if (preset == ExpressionPreset.custom) continue;

                var key = new ExpressionKey(preset);
                VrmInstance.Runtime.Expression.SetWeight(key, 0f);
            }

            VrmInstance.Vrm.Expression.CustomClips.ForEach(clip =>
            {
                var key = new ExpressionKey(ExpressionPreset.custom, clip.name);
                VrmInstance.Runtime.Expression.SetWeight(key, 0f);
            });

            if (!Application.isPlaying)
            {
                VrmInstance.Runtime.Process();
            }

            base.OnPlayableDestroy(playable);
        }
    }
}