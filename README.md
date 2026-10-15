# TurkeyMapApp

Türkiye’nin 81 ilini gösteren bir harita uygulaması. İl sınırları SVG; bir ilin üstüne gelince plaka ve gezilecek yer sayısı çıkıyor, tıklayınca o ilin sayfası açılıyor.

İl sayfasında yerler kart olarak listeleniyor, kategoriye göre süzülüyor ve Leaflet haritasında işaretleniyor. Veriler SQLite’ta. İlk çalıştırmada migration ve seed kendisi yapılıyor.

Arayüz açık ve koyu temayı hatırlıyor. Dil seçimi TR, EN ve DE.

## Çalıştırmak

.NET 10 SDK kurulu olmalı.

```bash
dotnet run
```

Tarayıcıda `http://localhost:5008`.

## Yığın

- ASP.NET Core MVC
- Entity Framework Core, SQLite
- SVG harita
- Leaflet
