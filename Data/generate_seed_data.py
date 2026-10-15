#!/usr/bin/env python3
"""Generate seed-data.json for TurkeyMapApp."""
import json
import os

def w(path, width=800):
    fn = path.split("/")[-1]
    return f"https://upload.wikimedia.org/wikipedia/commons/thumb/{path}/{width}px-{fn}"

def u(photo_id):
    return f"https://images.unsplash.com/{photo_id}?w=800&q=80"

CITIES = [
    {"plateCode": 1, "name": "Adana", "code": "ADA", "region": "Akdeniz", "population": 2270000,
     "imageUrl": w("4/4e/Tas_Kopru_Adana.jpg")},
    {"plateCode": 2, "name": "Adıyaman", "code": "ADI", "region": "Güneydoğu Anadolu", "population": 635000,
     "imageUrl": w("a/a0/Nemrut_Dagi_Heidelberg_20080329.jpg")},
    {"plateCode": 3, "name": "Afyonkarahisar", "code": "AFY", "region": "Ege", "population": 747000,
     "imageUrl": u("photo-1590076216867-059c96750484")},
    {"plateCode": 4, "name": "Ağrı", "code": "AGR", "region": "Doğu Anadolu", "population": 510000,
     "imageUrl": w("5/5c/Mount_Ararat_from_Akhuryan.jpg")},
    {"plateCode": 5, "name": "Amasya", "code": "AMA", "region": "Karadeniz", "population": 339000,
     "imageUrl": w("8/8a/Amasya_yesilirmak.jpg")},
    {"plateCode": 6, "name": "Ankara", "code": "ANK", "region": "İç Anadolu", "population": 5800000,
     "imageUrl": w("1/1f/Anitkabir_from_the_air.jpg")},
    {"plateCode": 7, "name": "Antalya", "code": "ANT", "region": "Akdeniz", "population": 2700000,
     "imageUrl": w("b/b6/Kaleici_Antalya.jpg")},
    {"plateCode": 8, "name": "Artvin", "code": "ART", "region": "Karadeniz", "population": 169000,
     "imageUrl": w("d/d4/Kackar_Mountains.jpg")},
    {"plateCode": 9, "name": "Aydın", "code": "AYD", "region": "Ege", "population": 1150000,
     "imageUrl": w("4/4a/Aphrodisias_Turkey_Theater.jpg")},
    {"plateCode": 10, "name": "Balıkesir", "code": "BAL", "region": "Marmara", "population": 1260000,
     "imageUrl": w("0/0c/Ayvalik_Cunda_Island.jpg")},
    {"plateCode": 11, "name": "Bilecik", "code": "BIL", "region": "Marmara", "population": 228000,
     "imageUrl": u("photo-1569336410465-997c472725a9")},
    {"plateCode": 12, "name": "Bingöl", "code": "BIN", "region": "Doğu Anadolu", "population": 283000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 13, "name": "Bitlis", "code": "BIT", "region": "Doğu Anadolu", "population": 353000,
     "imageUrl": w("6/6e/Nemrut_Crater_Lake.jpg")},
    {"plateCode": 14, "name": "Bolu", "code": "BOL", "region": "Karadeniz", "population": 324000,
     "imageUrl": w("9/9a/Yedigoller_National_Park.jpg")},
    {"plateCode": 15, "name": "Burdur", "code": "BRD", "region": "Akdeniz", "population": 273000,
     "imageUrl": w("5/5a/Sagalassos_Agora.jpg")},
    {"plateCode": 16, "name": "Bursa", "code": "BUR", "region": "Marmara", "population": 3200000,
     "imageUrl": w("4/4f/Bursa_Ulu_Cami_2014.jpg")},
    {"plateCode": 17, "name": "Çanakkale", "code": "CNK", "region": "Marmara", "population": 560000,
     "imageUrl": w("8/8e/Troya_antik_kenti_giris.jpg")},
    {"plateCode": 18, "name": "Çankırı", "code": "CAN", "region": "İç Anadolu", "population": 195000,
     "imageUrl": u("photo-1464822759844-d150baec0133")},
    {"plateCode": 19, "name": "Çorum", "code": "COR", "region": "Karadeniz", "population": 528000,
     "imageUrl": w("0/0a/Hattusa_Lion_Gate.jpg")},
    {"plateCode": 20, "name": "Denizli", "code": "DEN", "region": "Ege", "population": 1050000,
     "imageUrl": w("e/e0/Pamukkale_30.jpg")},
    {"plateCode": 21, "name": "Diyarbakır", "code": "DIY", "region": "Güneydoğu Anadolu", "population": 1800000,
     "imageUrl": w("4/4d/Diyarbakir_city_walls.jpg")},
    {"plateCode": 22, "name": "Edirne", "code": "EDI", "region": "Marmara", "population": 413000,
     "imageUrl": w("6/64/Selimiye_Mosque_Edirne_Turkey.jpg")},
    {"plateCode": 23, "name": "Elazığ", "code": "ELA", "region": "Doğu Anadolu", "population": 591000,
     "imageUrl": w("5/5e/Harput_Castle_Elazig.jpg")},
    {"plateCode": 24, "name": "Erzincan", "code": "ERC", "region": "Doğu Anadolu", "population": 239000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 25, "name": "Erzurum", "code": "ERZ", "region": "Doğu Anadolu", "population": 758000,
     "imageUrl": w("8/8a/Erzurum_Cifte_Minareli_Medrese.jpg")},
    {"plateCode": 26, "name": "Eskişehir", "code": "ESK", "region": "İç Anadolu", "population": 898000,
     "imageUrl": w("5/5a/Odunpazari_Eskisehir.jpg")},
    {"plateCode": 27, "name": "Gaziantep", "code": "GAZ", "region": "Güneydoğu Anadolu", "population": 2150000,
     "imageUrl": w("9/9a/Gaziantep_Castle.jpg")},
    {"plateCode": 28, "name": "Giresun", "code": "GIR", "region": "Karadeniz", "population": 453000,
     "imageUrl": w("3/3a/Giresun_Castle.jpg")},
    {"plateCode": 29, "name": "Gümüşhane", "code": "GUM", "region": "Karadeniz", "population": 151000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 30, "name": "Hakkari", "code": "HAK", "region": "Doğu Anadolu", "population": 287000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 31, "name": "Hatay", "code": "HAT", "region": "Akdeniz", "population": 1700000,
     "imageUrl": w("4/4a/Antakya_Habib-i_Neccar_Cami.jpg")},
    {"plateCode": 32, "name": "Isparta", "code": "ISP", "region": "Akdeniz", "population": 445000,
     "imageUrl": w("5/5a/Sagalassos_Agora.jpg")},
    {"plateCode": 33, "name": "Mersin", "code": "MER", "region": "Akdeniz", "population": 1900000,
     "imageUrl": w("5/5a/Kizkalesi_Mersin.jpg")},
    {"plateCode": 34, "name": "İstanbul", "code": "IST", "region": "Marmara", "population": 15900000,
     "imageUrl": w("9/93/Hagia_Sophia_Mars_2013.jpg")},
    {"plateCode": 35, "name": "İzmir", "code": "IZM", "region": "Ege", "population": 4500000,
     "imageUrl": w("4/4f/Clock_Tower_Izmir.jpg")},
    {"plateCode": 36, "name": "Kars", "code": "KAR", "region": "Doğu Anadolu", "population": 278000,
     "imageUrl": w("8/8a/Ani_Ruins_Kars.jpg")},
    {"plateCode": 37, "name": "Kastamonu", "code": "KAS", "region": "Karadeniz", "population": 376000,
     "imageUrl": w("6/6a/Kastamonu_Castle.jpg")},
    {"plateCode": 38, "name": "Kayseri", "code": "KAY", "region": "İç Anadolu", "population": 1440000,
     "imageUrl": w("5/5a/Mount_Erciyes.jpg")},
    {"plateCode": 39, "name": "Kırklareli", "code": "KIR", "region": "Marmara", "population": 368000,
     "imageUrl": w("6/64/Selimiye_Mosque_Edirne_Turkey.jpg")},
    {"plateCode": 40, "name": "Kırşehir", "code": "KRS", "region": "İç Anadolu", "population": 243000,
     "imageUrl": u("photo-1569336410465-997c472725a9")},
    {"plateCode": 41, "name": "Kocaeli", "code": "KOC", "region": "Marmara", "population": 2050000,
     "imageUrl": w("5/5a/Masukiye_Waterfall.jpg")},
    {"plateCode": 42, "name": "Konya", "code": "KON", "region": "İç Anadolu", "population": 2300000,
     "imageUrl": w("6/6a/Mevlana_Museum_Konya.jpg")},
    {"plateCode": 43, "name": "Kütahya", "code": "KUT", "region": "Ege", "population": 571000,
     "imageUrl": u("photo-1590076216867-059c96750484")},
    {"plateCode": 44, "name": "Malatya", "code": "MAL", "region": "Doğu Anadolu", "population": 812000,
     "imageUrl": w("5/5e/Harput_Castle_Elazig.jpg")},
    {"plateCode": 45, "name": "Manisa", "code": "MAN", "region": "Ege", "population": 1470000,
     "imageUrl": w("4/4a/Aphrodisias_Turkey_Theater.jpg")},
    {"plateCode": 46, "name": "Kahramanmaraş", "code": "KAH", "region": "Akdeniz", "population": 1180000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 47, "name": "Mardin", "code": "MAR", "region": "Güneydoğu Anadolu", "population": 870000,
     "imageUrl": w("8/8a/Mardin_old_city.jpg")},
    {"plateCode": 48, "name": "Muğla", "code": "MUG", "region": "Ege", "population": 1050000,
     "imageUrl": w("4/4a/Oludeniz_Beach.jpg")},
    {"plateCode": 49, "name": "Muş", "code": "MUS", "region": "Doğu Anadolu", "population": 408000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 50, "name": "Nevşehir", "code": "NEV", "region": "İç Anadolu", "population": 310000,
     "imageUrl": w("5/5a/Goreme_Open_Air_Museum.jpg")},
    {"plateCode": 51, "name": "Niğde", "code": "NIG", "region": "İç Anadolu", "population": 364000,
     "imageUrl": w("5/5a/Goreme_Open_Air_Museum.jpg")},
    {"plateCode": 52, "name": "Ordu", "code": "ORD", "region": "Karadeniz", "population": 760000,
     "imageUrl": w("3/3a/Giresun_Castle.jpg")},
    {"plateCode": 53, "name": "Rize", "code": "RIZ", "region": "Karadeniz", "population": 344000,
     "imageUrl": w("d/d4/Kackar_Mountains.jpg")},
    {"plateCode": 54, "name": "Sakarya", "code": "SAK", "region": "Marmara", "population": 1070000,
     "imageUrl": w("5/5a/Masukiye_Waterfall.jpg")},
    {"plateCode": 55, "name": "Samsun", "code": "SAM", "region": "Karadeniz", "population": 1360000,
     "imageUrl": w("3/3a/Ataturk_Monument_Samsun.jpg")},
    {"plateCode": 56, "name": "Siirt", "code": "SII", "region": "Güneydoğu Anadolu", "population": 331000,
     "imageUrl": u("photo-1569336410465-997c472725a9")},
    {"plateCode": 57, "name": "Sinop", "code": "SIN", "region": "Karadeniz", "population": 218000,
     "imageUrl": w("6/6a/Sinop_Fortress.jpg")},
    {"plateCode": 58, "name": "Sivas", "code": "SIV", "region": "İç Anadolu", "population": 635000,
     "imageUrl": w("6/6a/Sivas_Gok_Madrasa.jpg")},
    {"plateCode": 59, "name": "Tekirdağ", "code": "TEK", "region": "Marmara", "population": 1150000,
     "imageUrl": w("0/0c/Ayvalik_Cunda_Island.jpg")},
    {"plateCode": 60, "name": "Tokat", "code": "TOK", "region": "Karadeniz", "population": 612000,
     "imageUrl": w("8/8a/Amasya_yesilirmak.jpg")},
    {"plateCode": 61, "name": "Trabzon", "code": "TRA", "region": "Karadeniz", "population": 816000,
     "imageUrl": w("5/5a/Sumela_Monastery.jpg")},
    {"plateCode": 62, "name": "Tunceli", "code": "TUN", "region": "Doğu Anadolu", "population": 84000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 63, "name": "Şanlıurfa", "code": "SAN", "region": "Güneydoğu Anadolu", "population": 2200000,
     "imageUrl": w("4/4a/Gobekli_Tepe_site.jpg")},
    {"plateCode": 64, "name": "Uşak", "code": "USA", "region": "Ege", "population": 373000,
     "imageUrl": u("photo-1590076216867-059c96750484")},
    {"plateCode": 65, "name": "Van", "code": "VAN", "region": "Doğu Anadolu", "population": 1130000,
     "imageUrl": w("5/5a/Van_Lake_and_Akdamar_Island.jpg")},
    {"plateCode": 66, "name": "Yozgat", "code": "YOZ", "region": "İç Anadolu", "population": 421000,
     "imageUrl": u("photo-1569336410465-997c472725a9")},
    {"plateCode": 67, "name": "Zonguldak", "code": "ZON", "region": "Karadeniz", "population": 589000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 68, "name": "Aksaray", "code": "AKS", "region": "İç Anadolu", "population": 429000,
     "imageUrl": w("5/5a/Goreme_Open_Air_Museum.jpg")},
    {"plateCode": 69, "name": "Bayburt", "code": "BAY", "region": "Karadeniz", "population": 82000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 70, "name": "Karaman", "code": "KRM", "region": "İç Anadolu", "population": 260000,
     "imageUrl": u("photo-1569336410465-997c472725a9")},
    {"plateCode": 71, "name": "Kırıkkale", "code": "KRK", "region": "İç Anadolu", "population": 286000,
     "imageUrl": w("1/1f/Anitkabir_from_the_air.jpg")},
    {"plateCode": 72, "name": "Batman", "code": "BAT", "region": "Güneydoğu Anadolu", "population": 634000,
     "imageUrl": w("8/8a/Mardin_old_city.jpg")},
    {"plateCode": 73, "name": "Şırnak", "code": "SIR", "region": "Güneydoğu Anadolu", "population": 546000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 74, "name": "Bartın", "code": "BAR", "region": "Karadeniz", "population": 198000,
     "imageUrl": w("9/9a/Yedigoller_National_Park.jpg")},
    {"plateCode": 75, "name": "Ardahan", "code": "ARD", "region": "Doğu Anadolu", "population": 96000,
     "imageUrl": u("photo-1506905925346-21bda4d32df4")},
    {"plateCode": 76, "name": "Iğdır", "code": "IGD", "region": "Doğu Anadolu", "population": 203000,
     "imageUrl": w("5/5c/Mount_Ararat_from_Akhuryan.jpg")},
    {"plateCode": 77, "name": "Yalova", "code": "YAL", "region": "Marmara", "population": 296000,
     "imageUrl": u("photo-1569336410465-997c472725a9")},
    {"plateCode": 78, "name": "Karabük", "code": "KRB", "region": "Karadeniz", "population": 248000,
     "imageUrl": w("6/6a/Safranbolu_Old_Houses.jpg")},
    {"plateCode": 79, "name": "Kilis", "code": "KIL", "region": "Güneydoğu Anadolu", "population": 145000,
     "imageUrl": w("9/9a/Gaziantep_Castle.jpg")},
    {"plateCode": 80, "name": "Osmaniye", "code": "OSM", "region": "Akdeniz", "population": 557000,
     "imageUrl": w("4/4e/Tas_Kopru_Adana.jpg")},
    {"plateCode": 81, "name": "Düzce", "code": "DUZ", "region": "Karadeniz", "population": 405000,
     "imageUrl": w("9/9a/Yedigoller_National_Park.jpg")},
]

# (plateCode, name, description, lat, lon, address, category, imageUrl)
RAW_PLACES = []

def add(pc, name, desc, lat, lon, addr, cat, img):
    RAW_PLACES.append({
        "plateCode": pc, "name": name, "description": desc,
        "latitude": lat, "longitude": lon, "address": addr,
        "category": cat, "imageUrl": img
    })

# Import places from separate data module
from places_data import populate_places
populate_places(add, w, u)

def main():
    out = os.path.join(os.path.dirname(__file__), "seed-data.json")
    data = {"cities": CITIES, "places": RAW_PLACES}
    with open(out, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)
    size = os.path.getsize(out)
    counts = {}
    for p in RAW_PLACES:
        counts[p["plateCode"]] = counts.get(p["plateCode"], 0) + 1
    print(f"Cities: {len(CITIES)}")
    print(f"Places: {len(RAW_PLACES)}")
    print(f"File size: {size:,} bytes ({size/1024/1024:.2f} MB)")
    low = [c["name"] for c in CITIES if counts.get(c["plateCode"], 0) < 12]
    if low:
        print(f"WARNING - cities with <12 places: {low}")
    ist = counts.get(34, 0)
    ank = counts.get(6, 0)
    print(f"Istanbul places: {ist}, Ankara places: {ank}")

if __name__ == "__main__":
    main()
