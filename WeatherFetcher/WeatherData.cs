public class WeatherData
{
    public List<WeatherEntry> list { get; set; }
}

public class WeatherEntry
{
    public string dt_txt { get; set; }
    public long dt { get; set; }
    public Main main { get; set; }
}

public class Main
{
    public float temp { get; set; }
}

public class WeatherRecord
{
    public string dt_txt { get; set; }
    public long dt { get; set; }
    public float temp { get; set; }
}

public class WeatherRecordList
{
    public List<WeatherRecord> list { get; set; }
}
