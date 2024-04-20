using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace VrmExpressionExtension
{
    [CustomTimelineEditor(typeof(ExpressionClip))]
    public class ExpressionClipEditor : ClipEditor
    {
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);
            var expressionClip = clip.asset as ExpressionClip;
            if (expressionClip == null) return;
            clip.displayName = expressionClip.GetDisplayName();
        }

        public override void OnClipChanged(TimelineClip clip)
        {
            base.OnClipChanged(clip);
            var expressionClip = clip.asset as ExpressionClip;
            if (expressionClip == null) return;
            clip.displayName = expressionClip.GetDisplayName();
        }
    }
}