using System;
using System.Linq;
using System.Numerics;

namespace DevTools.Core.Random
{
    public sealed class OptimizedNoiseGenerator
    {
        private int _halfLength;
        private int _bitmask;
        private int[] _p;
        private int[] _permutation;

        public OptimizedNoiseGenerator(int octaves = 16, int seed = -1)
        {
            var len = (int)Math.Pow(2, octaves);
            _permutation = new int[len];

            GeneratePermutation(seed);
        }

        public void GeneratePermutation(int seed = -1)
        {
            System.Random random = GetRandom(seed);
            var perm = Enumerable.Range(0, _permutation.Length).ToArray();
            for (var i = 0; i < perm.Length; i++)
            {
                var swapIndex = random.Next(perm.Length);

                var t = perm[i];
                perm[i] = perm[swapIndex];
                perm[swapIndex] = t;
            }

            _permutation = perm;
            Calculate();
        }

        private static System.Random GetRandom(int seed) => seed != -1 ? new System.Random(seed) : new System.Random();

        private void Calculate()
        {
            _p = new int[_permutation.Length * 2];
            _halfLength = _permutation.Length;
            _bitmask = _halfLength - 1;

            for (int i = 0; i < _permutation.Length; i++)
            {
                _p[_permutation.Length + i] = _p[i] = _permutation[i];
            }
        }

        public float Noise(Vector3 position) => Noise(position.X, position.Y, position.Z);

        public float Noise(float x, float y, float z)
        {
            int X = FastFloor(x);
            int Y = FastFloor(y);
            int Z = FastFloor(z);

            x -= X;
            y -= Y;
            z -= Z;

            X &= _bitmask;
            Y &= _bitmask;
            Z &= _bitmask;

            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);

            int A = _p[X] + Y;
            int B = _p[X + 1] + Y;

            int AA = _p[A] + Z;
            int AB = _p[A + 1] + Z;

            int BA = _p[B] + Z;
            int BB = _p[B + 1] + Z;

            return Mix(
                    Mix(
                         Mix(Grad(_p[AA], x, y, z),
                              Grad(_p[BA], x - 1, y, z), u),
                        Mix(Grad(_p[AB], x, y - 1, z),
                            Grad(_p[BB], x - 1, y - 1, z), u), v),
                    Mix(
                        Mix(Grad(_p[AA + 1], x, y, z - 1),
                            Grad(_p[BA + 1], x - 1, y, z - 1), u),
                        Mix(Grad(_p[AB + 1], x, y - 1, z - 1),
                            Grad(_p[BB + 1], x - 1, y - 1, z - 1), u), v), w);
        }

        private int FastFloor(float a)
        {
            return a > 0 ? (int)a : (int)a - 1;
        }

        private static float Mix(float a, float b, float by)
        {
            return a * by + b * (1 - by);
        }

        private static float Fade(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }

        private static float Grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            u = (h & 1) == 0 ? u : -u;
            float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            v = (h & 2) == 0 ? v : -v;

            return u + v;
        }
    }
}