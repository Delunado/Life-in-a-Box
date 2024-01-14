using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sand.Sandbox
{
    public class SeedParticle : Particle
    {
        public override ParticleType Type => ParticleType.Seed;

        private Color _color;
        public override Color Color => _color * _brightnessMultiplier;


        private int _growthRate = 200;
        private int _growthCounter = 0;
        private bool _isLeaf = false;

        public SeedParticle(ParticleGrid grid, bool isLeaf) : base(grid)
        {
            _isLeaf = isLeaf;

            //If it's leaf, there is a possibility to be red. But not always!
            _color = _random.Next(12) == 0 ? Color.red : Color.green;
            _color *= _random.NextFloat(0.6f, 0.8f);

            speed = 100.0f;
        }

        private void SetLeaf(bool isLeaf)
        {
            _isLeaf = isLeaf;
        }

        public override void UpdateState(int x, int y, float deltaTime)
        {
        }

        public override void UpdateBehaviour(int x, int y, float deltaTime)
        {
            // First, it try to move down. when it's on a surface, can grow
            if (!_isLeaf && (IsCellEmpty(x, y - 1) || IsCellOfType(x, y - 1, ParticleType.Water) ||
                             IsCellOfType(x, y - 1, ParticleType.Gas) || IsCellOfType(x, y - 1, ParticleType.Cloud)))
            {
                movementDirection = Vector2.down;
                UpdateMovement(x, y, deltaTime, IsCellSwappable);
            }
            else
            {
                _growthCounter++;

                if (_growthCounter >= _growthRate)
                {
                    _growthCounter = 0;

                    // Random direction to grow. Can be up, left or right
                    int direction = _random.Next(100);

                    switch (direction)
                    {
                        case < 33:
                            TryGrow(x - 1, y, true);
                            break;
                        case < 66:
                            TryGrow(x + 1, y, true);
                            break;
                        case < 100:
                            TryGrow(x, y + 1, false);
                            break;
                    }
                }
            }
        }

        private void TryGrow(int x, int y, bool isLeaf)
        {
            // If the cell is out of bounds, it's not empty
            if (!IsCellBounded(x, y)) return;

            if (IsCellEmpty(x, y) || IsCellOfType(x, y, ParticleType.Gas) ||
                IsCellOfType(x, y, ParticleType.Water) || IsCellOfType(x, y, ParticleType.Cloud))
            {
                int randomPossibility = _random.Next(20);

                if (randomPossibility == 0)
                {
                    _growthRate *= _random.Next(100, 200);
                    _growthRate = Mathf.Clamp(_growthRate, 0, 100000000);
                }
                else
                {
                    _growthRate = Int32.MaxValue;
                }

                SeedParticle particle = CreateCellAt(x, y, ParticleType.Seed) as SeedParticle;
                particle?.SetLeaf(isLeaf);
            }
        }

        public void Watered()
        {
            _growthCounter += 20;
        }

        protected override bool IsCellSwappable(int x, int y)
        {
            return IsCellOfType(x, y, ParticleType.Water) || IsCellEmpty(x, y);
        }
    }
}