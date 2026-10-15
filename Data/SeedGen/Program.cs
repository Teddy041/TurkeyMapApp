using System.Text.Json;
using System.Text.RegularExpressions;

var dataDir = FindDataDir();

static string FindDataDir()
{
    var dir = AppContext.BaseDirectory;
    for (var i = 0; i < 8; i++)
    {
        if (File.Exists(Path.Combine(dir, "places_data.py"))) return dir;
        var parent = Directory.GetParent(dir);
        if (parent == null) break;
        dir = parent.FullName;
    }
    return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
}

var cities = GetCities();
var places = ParsePlacesFromPythonFiles(dataDir);

var output = new { cities, places };
var outPath = Path.Combine(dataDir, "seed-data.json");
var json = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
File.WriteAllText(outPath, json, System.Text.Encoding.UTF8);

var size = new FileInfo(outPath).Length;
var counts = places.GroupBy(p => p.plateCode).ToDictionary(g => g.Key, g => g.Count());
Console.WriteLine($"Cities: {cities.Count}");
Console.WriteLine($"Places: {places.Count}");
Console.WriteLine($"File size: {size:N0} bytes ({size / 1024.0 / 1024.0:F2} MB)");
Console.WriteLine($"Istanbul places: {counts.GetValueOrDefault(34)}, Ankara places: {counts.GetValueOrDefault(6)}");
var low = cities.Where(c => counts.GetValueOrDefault(c.plateCode, 0) < 12).Select(c => c.name).ToList();
if (low.Count > 0) Console.WriteLine($"WARNING - cities with <12 places: {string.Join(", ", low)}");

static string ResolveImg(string expr)
{
    expr = expr.Trim().TrimEnd(',');
    var uMatch = Regex.Match(expr, @"u\(""([^""]+)""\)");
    if (uMatch.Success) return $"https://images.unsplash.com/{uMatch.Groups[1].Value}?w=800&q=80";
    var wMatch = Regex.Match(expr, @"w\(""([^""]+)""\)");
    if (wMatch.Success)
    {
        var path = wMatch.Groups[1].Value;
        var fn = path.Split('/')[^1];
        return $"https://upload.wikimedia.org/wikipedia/commons/thumb/{path}/800px-{fn}";
    }
    if (expr.StartsWith("_")) return "https://images.unsplash.com/photo-1569336410465-997c472725a9?w=800&q=80";
    return expr.Trim('"');
}

static PlaceItem? ParseTuple(string line, int plateCode)
{
    var m = Regex.Match(line, @"\(\""((?:[^""\\]|\\.)*)\""\s*,\s*\""((?:[^""\\]|\\.)*)\""\s*,\s*([\d.]+)\s*,\s*([\d.]+)\s*,\s*\""((?:[^""\\]|\\.)*)\""\s*,\s*\""((?:[^""\\]|\\.)*)\""\s*,\s*(.+)\)\s*,?\s*$");
    if (!m.Success) return null;
    return new PlaceItem(plateCode, m.Groups[1].Value, m.Groups[2].Value,
        double.Parse(m.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture),
        double.Parse(m.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture),
        m.Groups[5].Value, m.Groups[6].Value, ResolveImg(m.Groups[7].Value));
}

static List<PlaceItem> ParsePlacesFromPythonFiles(string dataDir)
{
    var places = new List<PlaceItem>();
    foreach (var file in Directory.GetFiles(dataDir, "places_data*.py"))
    {
        var lines = File.ReadAllLines(file);
        int? currentPlate = null;
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            var plateMatch = Regex.Match(line, @"^(\d+):\s*\[");
            if (plateMatch.Success) { currentPlate = int.Parse(plateMatch.Groups[1].Value); continue; }
            var addMatch = Regex.Match(line, @"^add\((\d+),\s*""((?:[^""\\]|\\.)*)""\s*,\s*""((?:[^""\\]|\\.)*)""\s*,\s*([\d.]+)\s*,\s*([\d.]+)\s*,\s*""((?:[^""\\]|\\.)*)""\s*,\s*""((?:[^""\\]|\\.)*)""\s*,\s*(.+)\)\s*$");
            if (addMatch.Success)
            {
                places.Add(new PlaceItem(int.Parse(addMatch.Groups[1].Value), addMatch.Groups[2].Value, addMatch.Groups[3].Value,
                    double.Parse(addMatch.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(addMatch.Groups[5].Value, System.Globalization.CultureInfo.InvariantCulture),
                    addMatch.Groups[6].Value, addMatch.Groups[7].Value, ResolveImg(addMatch.Groups[8].Value)));
                continue;
            }
            if (currentPlate.HasValue && line.StartsWith("(\""))
            {
                var item = ParseTuple(line, currentPlate.Value);
                if (item != null) places.Add(item);
            }
            if (line == "]," || line == "]") currentPlate = null;
        }
    }
    return places;
}

static List<CityItem> GetCities() =>
[
    new(1,"Adana","ADA","Akdeniz",2270000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4e/Tas_Kopru_Adana.jpg/800px-Tas_Kopru_Adana.jpg"),
    new(2,"Adıyaman","ADI","Güneydoğu Anadolu",635000,"https://upload.wikimedia.org/wikipedia/commons/thumb/a/a0/Nemrut_Dagi_Heidelberg_20080329.jpg/800px-Nemrut_Dagi_Heidelberg_20080329.jpg"),
    new(3,"Afyonkarahisar","AFY","Ege",747000,"https://images.unsplash.com/photo-1590076216867-059c96750484?w=800&q=80"),
    new(4,"Ağrı","AGR","Doğu Anadolu",510000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5c/Mount_Ararat_from_Akhuryan.jpg/800px-Mount_Ararat_from_Akhuryan.jpg"),
    new(5,"Amasya","AMA","Karadeniz",339000,"https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Amasya_yesilirmak.jpg/800px-Amasya_yesilirmak.jpg"),
    new(6,"Ankara","ANK","İç Anadolu",5800000,"https://upload.wikimedia.org/wikipedia/commons/thumb/1/1f/Anitkabir_from_the_air.jpg/800px-Anitkabir_from_the_air.jpg"),
    new(7,"Antalya","ANT","Akdeniz",2700000,"https://upload.wikimedia.org/wikipedia/commons/thumb/b/b6/Kaleici_Antalya.jpg/800px-Kaleici_Antalya.jpg"),
    new(8,"Artvin","ART","Karadeniz",169000,"https://upload.wikimedia.org/wikipedia/commons/thumb/d/d4/Kackar_Mountains.jpg/800px-Kackar_Mountains.jpg"),
    new(9,"Aydın","AYD","Ege",1150000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4a/Aphrodisias_Turkey_Theater.jpg/800px-Aphrodisias_Turkey_Theater.jpg"),
    new(10,"Balıkesir","BAL","Marmara",1260000,"https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Ayvalik_Cunda_Island.jpg/800px-Ayvalik_Cunda_Island.jpg"),
    new(11,"Bilecik","BIL","Marmara",228000,"https://images.unsplash.com/photo-1569336410465-997c472725a9?w=800&q=80"),
    new(12,"Bingöl","BIN","Doğu Anadolu",283000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(13,"Bitlis","BIT","Doğu Anadolu",353000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/6e/Nemrut_Crater_Lake.jpg/800px-Nemrut_Crater_Lake.jpg"),
    new(14,"Bolu","BOL","Karadeniz",324000,"https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Yedigoller_National_Park.jpg/800px-Yedigoller_National_Park.jpg"),
    new(15,"Burdur","BRD","Akdeniz",273000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Sagalassos_Agora.jpg/800px-Sagalassos_Agora.jpg"),
    new(16,"Bursa","BUR","Marmara",3200000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4f/Bursa_Ulu_Cami_2014.jpg/800px-Bursa_Ulu_Cami_2014.jpg"),
    new(17,"Çanakkale","CNK","Marmara",560000,"https://upload.wikimedia.org/wikipedia/commons/thumb/8/8e/Troya_antik_kenti_giris.jpg/800px-Troya_antik_kenti_giris.jpg"),
    new(18,"Çankırı","CAN","İç Anadolu",195000,"https://images.unsplash.com/photo-1464822759844-d150baec0133?w=800&q=80"),
    new(19,"Çorum","COR","Karadeniz",528000,"https://upload.wikimedia.org/wikipedia/commons/thumb/0/0a/Hattusa_Lion_Gate.jpg/800px-Hattusa_Lion_Gate.jpg"),
    new(20,"Denizli","DEN","Ege",1050000,"https://upload.wikimedia.org/wikipedia/commons/thumb/e/e0/Pamukkale_30.jpg/800px-Pamukkale_30.jpg"),
    new(21,"Diyarbakır","DIY","Güneydoğu Anadolu",1800000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4d/Diyarbakir_city_walls.jpg/800px-Diyarbakir_city_walls.jpg"),
    new(22,"Edirne","EDI","Marmara",413000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Selimiye_Mosque_Edirne_Turkey.jpg/800px-Selimiye_Mosque_Edirne_Turkey.jpg"),
    new(23,"Elazığ","ELA","Doğu Anadolu",591000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Harput_Castle_Elazig.jpg/800px-Harput_Castle_Elazig.jpg"),
    new(24,"Erzincan","ERC","Doğu Anadolu",239000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(25,"Erzurum","ERZ","Doğu Anadolu",758000,"https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Erzurum_Cifte_Minareli_Medrese.jpg/800px-Erzurum_Cifte_Minareli_Medrese.jpg"),
    new(26,"Eskişehir","ESK","İç Anadolu",898000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Odunpazari_Eskisehir.jpg/800px-Odunpazari_Eskisehir.jpg"),
    new(27,"Gaziantep","GAZ","Güneydoğu Anadolu",2150000,"https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Gaziantep_Castle.jpg/800px-Gaziantep_Castle.jpg"),
    new(28,"Giresun","GIR","Karadeniz",453000,"https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Giresun_Castle.jpg/800px-Giresun_Castle.jpg"),
    new(29,"Gümüşhane","GUM","Karadeniz",151000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(30,"Hakkari","HAK","Doğu Anadolu",287000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(31,"Hatay","HAT","Akdeniz",1700000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4a/Antakya_Habib-i_Neccar_Cami.jpg/800px-Antakya_Habib-i_Neccar_Cami.jpg"),
    new(32,"Isparta","ISP","Akdeniz",445000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Sagalassos_Agora.jpg/800px-Sagalassos_Agora.jpg"),
    new(33,"Mersin","MER","Akdeniz",1900000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Kizkalesi_Mersin.jpg/800px-Kizkalesi_Mersin.jpg"),
    new(34,"İstanbul","IST","Marmara",15900000,"https://upload.wikimedia.org/wikipedia/commons/thumb/9/93/Hagia_Sophia_Mars_2013.jpg/800px-Hagia_Sophia_Mars_2013.jpg"),
    new(35,"İzmir","IZM","Ege",4500000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4f/Clock_Tower_Izmir.jpg/800px-Clock_Tower_Izmir.jpg"),
    new(36,"Kars","KAR","Doğu Anadolu",278000,"https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Ani_Ruins_Kars.jpg/800px-Ani_Ruins_Kars.jpg"),
    new(37,"Kastamonu","KAS","Karadeniz",376000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Kastamonu_Castle.jpg/800px-Kastamonu_Castle.jpg"),
    new(38,"Kayseri","KAY","İç Anadolu",1440000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Mount_Erciyes.jpg/800px-Mount_Erciyes.jpg"),
    new(39,"Kırklareli","KIR","Marmara",368000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Selimiye_Mosque_Edirne_Turkey.jpg/800px-Selimiye_Mosque_Edirne_Turkey.jpg"),
    new(40,"Kırşehir","KRS","İç Anadolu",243000,"https://images.unsplash.com/photo-1569336410465-997c472725a9?w=800&q=80"),
    new(41,"Kocaeli","KOC","Marmara",2050000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Masukiye_Waterfall.jpg/800px-Masukiye_Waterfall.jpg"),
    new(42,"Konya","KON","İç Anadolu",2300000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Mevlana_Museum_Konya.jpg/800px-Mevlana_Museum_Konya.jpg"),
    new(43,"Kütahya","KUT","Ege",571000,"https://images.unsplash.com/photo-1590076216867-059c96750484?w=800&q=80"),
    new(44,"Malatya","MAL","Doğu Anadolu",812000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Harput_Castle_Elazig.jpg/800px-Harput_Castle_Elazig.jpg"),
    new(45,"Manisa","MAN","Ege",1470000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4a/Aphrodisias_Turkey_Theater.jpg/800px-Aphrodisias_Turkey_Theater.jpg"),
    new(46,"Kahramanmaraş","KAH","Akdeniz",1180000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(47,"Mardin","MAR","Güneydoğu Anadolu",870000,"https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Mardin_old_city.jpg/800px-Mardin_old_city.jpg"),
    new(48,"Muğla","MUG","Ege",1050000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4a/Oludeniz_Beach.jpg/800px-Oludeniz_Beach.jpg"),
    new(49,"Muş","MUS","Doğu Anadolu",408000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(50,"Nevşehir","NEV","İç Anadolu",310000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Goreme_Open_Air_Museum.jpg/800px-Goreme_Open_Air_Museum.jpg"),
    new(51,"Niğde","NIG","İç Anadolu",364000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Goreme_Open_Air_Museum.jpg/800px-Goreme_Open_Air_Museum.jpg"),
    new(52,"Ordu","ORD","Karadeniz",760000,"https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Giresun_Castle.jpg/800px-Giresun_Castle.jpg"),
    new(53,"Rize","RIZ","Karadeniz",344000,"https://upload.wikimedia.org/wikipedia/commons/thumb/d/d4/Kackar_Mountains.jpg/800px-Kackar_Mountains.jpg"),
    new(54,"Sakarya","SAK","Marmara",1070000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Masukiye_Waterfall.jpg/800px-Masukiye_Waterfall.jpg"),
    new(55,"Samsun","SAM","Karadeniz",1360000,"https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Ataturk_Monument_Samsun.jpg/800px-Ataturk_Monument_Samsun.jpg"),
    new(56,"Siirt","SII","Güneydoğu Anadolu",331000,"https://images.unsplash.com/photo-1569336410465-997c472725a9?w=800&q=80"),
    new(57,"Sinop","SIN","Karadeniz",218000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Sinop_Fortress.jpg/800px-Sinop_Fortress.jpg"),
    new(58,"Sivas","SIV","İç Anadolu",635000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Sivas_Gok_Madrasa.jpg/800px-Sivas_Gok_Madrasa.jpg"),
    new(59,"Tekirdağ","TEK","Marmara",1150000,"https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Ayvalik_Cunda_Island.jpg/800px-Ayvalik_Cunda_Island.jpg"),
    new(60,"Tokat","TOK","Karadeniz",612000,"https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Amasya_yesilirmak.jpg/800px-Amasya_yesilirmak.jpg"),
    new(61,"Trabzon","TRA","Karadeniz",816000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Sumela_Monastery.jpg/800px-Sumela_Monastery.jpg"),
    new(62,"Tunceli","TUN","Doğu Anadolu",84000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(63,"Şanlıurfa","SAN","Güneydoğu Anadolu",2200000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4a/Gobekli_Tepe_site.jpg/800px-Gobekli_Tepe_site.jpg"),
    new(64,"Uşak","USA","Ege",373000,"https://images.unsplash.com/photo-1590076216867-059c96750484?w=800&q=80"),
    new(65,"Van","VAN","Doğu Anadolu",1130000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Van_Lake_and_Akdamar_Island.jpg/800px-Van_Lake_and_Akdamar_Island.jpg"),
    new(66,"Yozgat","YOZ","İç Anadolu",421000,"https://images.unsplash.com/photo-1569336410465-997c472725a9?w=800&q=80"),
    new(67,"Zonguldak","ZON","Karadeniz",589000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(68,"Aksaray","AKS","İç Anadolu",429000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5a/Goreme_Open_Air_Museum.jpg/800px-Goreme_Open_Air_Museum.jpg"),
    new(69,"Bayburt","BAY","Karadeniz",82000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(70,"Karaman","KRM","İç Anadolu",260000,"https://images.unsplash.com/photo-1569336410465-997c472725a9?w=800&q=80"),
    new(71,"Kırıkkale","KRK","İç Anadolu",286000,"https://upload.wikimedia.org/wikipedia/commons/thumb/1/1f/Anitkabir_from_the_air.jpg/800px-Anitkabir_from_the_air.jpg"),
    new(72,"Batman","BAT","Güneydoğu Anadolu",634000,"https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Mardin_old_city.jpg/800px-Mardin_old_city.jpg"),
    new(73,"Şırnak","SIR","Güneydoğu Anadolu",546000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(74,"Bartın","BAR","Karadeniz",198000,"https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Yedigoller_National_Park.jpg/800px-Yedigoller_National_Park.jpg"),
    new(75,"Ardahan","ARD","Doğu Anadolu",96000,"https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80"),
    new(76,"Iğdır","IGD","Doğu Anadolu",203000,"https://upload.wikimedia.org/wikipedia/commons/thumb/5/5c/Mount_Ararat_from_Akhuryan.jpg/800px-Mount_Ararat_from_Akhuryan.jpg"),
    new(77,"Yalova","YAL","Marmara",296000,"https://images.unsplash.com/photo-1569336410465-997c472725a9?w=800&q=80"),
    new(78,"Karabük","KRB","Karadeniz",248000,"https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Safranbolu_Old_Houses.jpg/800px-Safranbolu_Old_Houses.jpg"),
    new(79,"Kilis","KIL","Güneydoğu Anadolu",145000,"https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Gaziantep_Castle.jpg/800px-Gaziantep_Castle.jpg"),
    new(80,"Osmaniye","OSM","Akdeniz",557000,"https://upload.wikimedia.org/wikipedia/commons/thumb/4/4e/Tas_Kopru_Adana.jpg/800px-Tas_Kopru_Adana.jpg"),
    new(81,"Düzce","DUZ","Karadeniz",405000,"https://upload.wikimedia.org/wikipedia/commons/thumb/9/9a/Yedigoller_National_Park.jpg/800px-Yedigoller_National_Park.jpg"),
];

record CityItem(int plateCode, string name, string code, string region, long population, string imageUrl);
record PlaceItem(int plateCode, string name, string description, double latitude, double longitude, string address, string category, string imageUrl);
