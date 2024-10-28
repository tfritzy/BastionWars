using DotnetNoise;
using System;
using System.Text;

namespace KeepLordWarriors
{
    public static class MapGenerator
    {
        public class TerrainConfig
        {
            public float Scale { get; set; } = 8f;
            public float WaterThreshold { get; set; } = -.8f;
            public float TreeThreshold { get; set; } = 0.7f;
            public int Seed { get; set; } = 0;
            public int KeepSpacing { get; set; } = 5;
            public int FieldSpacing { get; set; } = 2;
        }

        public static string Generate(int width, int height, TerrainConfig config = null)
        {
            // Use default config if none provided
            config ??= new TerrainConfig();

            Random r = new();

            // Initialize noise generator
            var noise = new FastNoiseLite(config.Seed);

            char[,] map = new char[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[y, x] = '.';
                }
            }

            // generate fields
            for (int y = 1; y < height / config.FieldSpacing; y++)
            {
                for (int x = 1; x < width / config.FieldSpacing; x++)
                {
                    int modX = x * config.FieldSpacing + r.Next(-1, 2);
                    int modY = y * config.FieldSpacing + r.Next(-1, 2);
                    map[modY, modX] = 'F';
                }
            }

            // generate keeps
            for (int y = 1; y < height / config.KeepSpacing; y++)
            {
                for (int x = 1; x < width / config.KeepSpacing; x++)
                {
                    int modX = x * config.KeepSpacing + r.Next(-2, 3);
                    int modY = y * config.KeepSpacing + r.Next(-2, 3);
                    map[modY, modX] = 'W';
                }
            }

            // Generate terrain
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Generate noise value
                    float nx = x * config.Scale;
                    float ny = y * config.Scale;
                    float noiseValue = noise.GetNoise(nx, ny);

                    // Already normalized to 0-1 range in DotnetNoise
                    if (noiseValue < config.WaterThreshold)
                    {
                        map[y, x] = 'X'; // Water
                    }
                    else if (noiseValue > config.TreeThreshold)
                    {
                        map[y, x] = 'T'; // Trees
                    }
                }
            }

            // Convert the map to a string for easy viewing/debugging
            string result = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result += map[y, x];
                }

                if (y != height - 1)
                    result += "\n";
            }

            return result;
        }

        // Helper method to directly get the char array if needed
        public static char[,] GenerateArray(int width, int height, TerrainConfig config = null)
        {
            config ??= new TerrainConfig();

            var noise = new FastNoiseLite(config.Seed);

            char[,] map = new char[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float nx = x * config.Scale;
                    float ny = y * config.Scale;
                    float noiseValue = noise.GetNoise(nx, ny);

                    if (noiseValue < config.WaterThreshold)
                    {
                        map[y, x] = 'X';
                    }
                    else if (noiseValue > config.TreeThreshold)
                    {
                        map[y, x] = 'T';
                    }
                    else
                    {
                        map[y, x] = '.';
                    }
                }
            }

            return map;
        }
    }
}