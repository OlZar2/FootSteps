namespace FS.API.SecretsLoader;

public static class FileSecretsLoader
{
    private static void ApplyFileEnv(string key)
    {
        var file = Environment.GetEnvironmentVariable(key + "_FILE");
        if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
        {
            var value = File.ReadAllText(file).Trim();
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    public static void LoadSecretFiles()
    {
        ApplyFileEnv("ConnectionStrings__DefaultConnection");
        ApplyFileEnv("JwtOptions__SecretKey");
        ApplyFileEnv("S3StorageConfiguration__AccessKey");
        ApplyFileEnv("S3StorageConfiguration__SecretKey");
        ApplyFileEnv("YandexApiOptions__ApiKey");
        ApplyFileEnv("RabbitMqOptions__Uri");
    }
}