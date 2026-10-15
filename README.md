# TurkeyMapApp

Türkiye’nin 81 ilini gösteren bir harita sitesi.

Ana sayfada illerin sınırları çizili bir Türkiye haritası var. Farenle bir ilin üstüne gelince ilin adı, plaka numarası ve o ilde kaç gezilecek yer olduğu çıkıyor. İle tıklayınca o ilin kendi sayfası açılıyor.

İl sayfasında şunlar var:

- Gezilecek yerler kartlar halinde listeleniyor
- Müze, plaj, tarihi yer gibi kategorilere göre süzülebiliyor
- Yerler bir konum haritasında iğneyle işaretleniyor; haritayı yakınlaştırıp uzaklaştırabiliyorsun

Bilgiler uygulamayla birlikte geliyor. İlk açılışta veritabanı kendiliğinden hazırlanıyor.

Sağ üstten aydınlık ya da karanlık görünüm seçiliyor. Dil de Türkçe, İngilizce veya Almanca olabiliyor. Bu seçimler tarayıcıda hatırlanıyor.

## Nasıl açılır

Bilgisayarda [.NET 10](https://dotnet.microsoft.com/download) yüklü olmalı.

Proje klasöründe:

```bash
dotnet run
```

Sonra tarayıcıda şu adresi aç: `http://localhost:5008`

## Ne ile yapıldı

- Web sitesi: ASP.NET Core
- Veritabanı: SQLite (bilgisayarda tutulan küçük bir veritabanı)
- Ana harita: illerin çizildiği bir vektör resim (SVG). Bu, Google Street View değil; tıklanabilen bir Türkiye çizimi
- İl sayfasındaki konum haritası: Leaflet. Yerler normal bir harita üzerinde işaretleniyor
