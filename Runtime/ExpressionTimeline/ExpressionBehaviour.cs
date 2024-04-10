using UnityEngine;
using UnityEngine.Playables;
using UniVRM10;


namespace VrmExpressionExtension
{
    [System.Serializable]
    public class ExpressionBehaviour : PlayableBehaviour
    {
        [SerializeField] private ExpressionPreset _preset;
        [SerializeField, Range(0.0f, 2.0f)] private float _weight;
        [SerializeField] private VRM10Expression _customExpression;
        public ExpressionPreset Preset => _preset;
        public float Weight => _weight;

        public VRM10Expression CustomExpression
        {
            get => _customExpression;
            set => _customExpression = value;
        }
    }
}