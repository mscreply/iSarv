using System.Text;

namespace iSarv.Data;

public interface IAppSettingsManager
{
    IConfiguration Configuration { get; set; }
    public string ReadAllAppSettingsJson();
    public void WriteAllAppSettingsJson(string config);
}

public class AppSettingsManager: IAppSettingsManager
{
    public IConfiguration Configuration { get; set; }

    public AppSettingsManager(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public string ReadAllAppSettingsJson()
    {
        return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
    }

    public void WriteAllAppSettingsJson(string config)
    {
        Configuration  = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(config))).Build();
        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "appsettings.json"),config);
    }
}