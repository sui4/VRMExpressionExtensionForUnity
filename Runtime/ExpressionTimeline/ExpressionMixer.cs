using System.Collections.Generic;
using UnityEngine.Playables;
using UniVRM10;

namespace VrmExpressionExtension
{
    public class ExpressionMixerBehaviour : PlayableBehaviour
    {
        public Dictionary<ExpressionKey, float> Weight { get; } = new();

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var vrmInstance = playerData as Vrm10Instance;
            if (vrmInstance == null) return;

            Weight.Clear();

            int inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<ExpressionBehaviour>)playable.GetInput(i);
                ExpressionBehaviour behaviour = inputPlayable.GetBehaviour();
                if (inputWeight > 0f && behaviour.Weight > 0f)
                {
                    bool isCustom = behaviour.Preset == ExpressionPreset.custom;
                    if (isCustom && behaviour.CustomExpression == null) continue;

                    string label = isCustom ? behaviour.CustomExpression.name : null;
                    var key = new ExpressionKey(behaviour.Preset, label);
                    if (Weight.TryGetValue(key, out float curWeight))
                    {
                        Weight[key] = curWeight + behaviour.Weight * inputWeight;
                    }
                    else
                    {
                        Weight.Add(key, behaviour.Weight * inputWeight);
                    }
                }
            }
        }
    }
}