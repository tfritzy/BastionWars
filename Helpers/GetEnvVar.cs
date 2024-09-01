using DotNetEnv;

namespace Helpers;

public static class EnvHelpers
{
    public static string Get(string name)
    {
        string variable = Environment.GetEnvironmentVariable(name)
            ?? throw new Exception($"Missing {name} in env file");

        if (string.IsNullOrEmpty(variable))
        {
            throw new Exception($"Missing {name} in env file");
        }

        return variable;
    }

    public static void Init()
    {
        string environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development";
        string envFile = environment == "Production" ? ".env.production" : ".env";
        Env.Load(envFile);
    }
}