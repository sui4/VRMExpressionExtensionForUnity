using System.Collections.Generic;
using UnityEngine.Playables;
using UniVRM10;

namespace VrmExpressionExtension
{
    public class ExpressionMixerBehaviour : PlayableBehaviour
    {
        private readonly Dictionary<ExpressionKey, float> _weightSum = new();
        public Dictionary<ExpressionKey, float> Weight => _weightSum;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var vrmInstance = playerData as Vrm10Instance;
            if (vrmInstance == null) return;
            
            _weightSum.Clear();

            int inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<ExpressionBehaviour>)playable.GetInput(i);
                ExpressionBehaviour behaviour = inputPlayable.GetBehaviour();
                if (inputWeight > 0f && behaviour.Weight > 0f)
                {
                    string label = behaviour.Preset == ExpressionPreset.custom ? behaviour.CustomExpression.name : null;
                    var key = new ExpressionKey(behaviour.Preset, label);
                    if (_weightSum.TryGetValue(key, out float curWeight))
                    {
                        _weightSum[key] = curWeight + behaviour.Weight * inputWeight;
                    }
                    else
                    {
                        _weightSum.Add(key, behaviour.Weight * inputWeight);
                    }

                }
            }
        }
    }
}