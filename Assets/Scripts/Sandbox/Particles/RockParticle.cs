using UnityEngine;

namespace Sand.Sandbox
{
    public class RockParticle : Particle
    {
        public override ParticleType Type => ParticleType.Rock;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;

        public RockParticle(ParticleGrid parentGrid) : base(parentGrid)
        {
            _color = Color.gray * _random.NextFloat(0.75f, 1.0f);
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