using UnityEngine;

namespace Sand.Sandbox
{
    public class EmptyParticle : Particle
    {
        public override ParticleType Type => ParticleType.Empty;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        public EmptyParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            _color = Color.white * _random.NextFloat(0.02f, 0.1f);
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
        }

        protected override bool IsCellSwappable(int x, int y)
        {
            return false;
        }
    }
}