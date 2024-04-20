using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UniVRM10;

namespace VrmExpressionExtension
{
    public class ExpressionClip : PlayableAsset, ITimelineClipAsset
    {
        public ExpressionBehaviour _template;
        public ClipCaps clipCaps => ClipCaps.Blending | ClipCaps.SpeedMultiplier;
        public VRM10ObjectExpression VrmObjectExpression { get; set; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<ExpressionBehaviour> playable = ScriptPlayable<ExpressionBehaviour>.Create(graph, _template);
            ExpressionBehaviour behaviour = playable.GetBehaviour();
            return playable;
        }
        
        public string GetDisplayName()
        {
            return $"{_template.Preset.ToString()} ( {_template.Weight} )";
        }
    }
}
