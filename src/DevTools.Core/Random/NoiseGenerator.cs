using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace DevTools.Core.Random
{
    public sealed class NoiseGenerator
    {
        private int _halfLength;

        private int[] _p;
        private int[] _permutation;

        public NoiseGenerator(int octaves = 16, int seed = -1)
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

            for (int i = 0; i < _permutation.Length; i++)
            {
                _p[_permutation.Length + i] = _p[i] = _permutation[i];
            }
        }

        public float Noise(Vector3 position) => Noise(position.X, position.Y, position.Z);

        public float Noise(float x, float y, float z)
        {
            int X = (int)Math.Floor(x) % _halfLength;
            int Y = (int)Math.Floor(y) % _halfLength;
            int Z = (int)Math.Floor(z) % _halfLength;

            if (X < 0)
                X += _halfLength;

            if (Y < 0)
                Y += _halfLength;

            if (Z < 0)
                Z += _halfLength;

            x -= (int)Math.Floor(x);
            y -= (int)Math.Floor(y);
            z -= (int)Math.Floor(z);

            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);

            int A = _p[X] + Y, AA = _p[A] + Z, AB = _p[A + 1] + Z,
                B = _p[X + 1] + Y, BA = _p[B] + Z, BB = _p[B + 1] + Z;

            return Lerp(
                    Lerp(
                         Lerp(Grad(_p[AA], x, y, z),
                              Grad(_p[BA], x - 1, y, z), u),
                        Lerp(Grad(_p[AB], x, y - 1, z),
                            Grad(_p[BB], x - 1, y - 1, z), u), v),
                    Lerp(
                        Lerp(Grad(_p[AA + 1], x, y, z - 1),
                            Grad(_p[BA + 1], x - 1, y, z - 1), u),
                        Lerp(Grad(_p[AB + 1], x, y - 1, z - 1),
                            Grad(_p[BB + 1], x - 1, y - 1, z - 1), u), v), w);
        }

        private static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * by + secondFloat * (1 - by);
        }

        private static float Fade(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }

        private static float Grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y,
                   v = h < 4 ? y : h == 12 || h == 14 ? x : z;

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}
