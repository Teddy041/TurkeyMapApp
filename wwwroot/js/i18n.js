/**
 * Arayüz dili - TR / EN / DE
 * Tercih localStorage'da saklanır.
 */
(function () {
    'use strict';

    const STORAGE_KEY = 'turkey-map-lang';
    const LANGS = ['tr', 'en', 'de'];

    const STRINGS = {
        tr: {
            language: 'Dil',
            toggleTheme: 'Tema değiştir',
            darkMode: 'Koyu Mod',
            lightMode: 'Açık Mod',
            appTitle: 'Türkiye İnteraktif Harita',
            appSubtitle: '81 il · SVG tabanlı interaktif dashboard',
            statsLabel: 'Genel istatistikler',
            totalProvinces: 'Toplam İl',
            totalPopulation: 'Toplam Nüfus',
            mapType: 'Harita Tipi',
            mapTitle: 'Türkiye Cumhuriyeti Haritası',
            mapHint: 'İllerin üzerine gelin veya tıklayarak detayları görün',
            mapAria: 'Türkiye haritası',
            selectPlaceholder: 'Bir ile tıklayarak gezilecek yerleri keşfedin.',
            footer: 'Türkiye Haritası · ASP.NET Core MVC',
            homeTitle: 'Türkiye Haritası',
            appName: 'Türkiye Haritası',
            placeCount: '{count} Gezilecek Yer',
            plate: 'Plaka',
            placesToVisit: 'Gezilecek Yer',
            mapLoadError: 'Harita yüklenirken bir hata oluştu.',
            backToMap: '← Haritaya Dön',
            category: 'Kategori',
            categories: 'Kategoriler',
            all: 'Tümü',
            locationMap: 'Konum Haritası',
            placesTitle: 'Gezilecek Yerler',
            placesLoading: 'Yerler yükleniyor...',
            placesLoadError: 'Yerler yüklenirken bir hata oluştu.'
        },
        en: {
            language: 'Language',
            toggleTheme: 'Toggle theme',
            darkMode: 'Dark Mode',
            lightMode: 'Light Mode',
            appTitle: 'Interactive Map of Turkey',
            appSubtitle: '81 provinces · SVG interactive dashboard',
            statsLabel: 'Overview',
            totalProvinces: 'Provinces',
            totalPopulation: 'Population',
            mapType: 'Map Type',
            mapTitle: 'Map of the Republic of Turkey',
            mapHint: 'Hover or click a province to see details',
            mapAria: 'Map of Turkey',
            selectPlaceholder: 'Click a province to explore places to visit.',
            footer: 'Turkey Map · ASP.NET Core MVC',
            homeTitle: 'Turkey Map',
            appName: 'Turkey Map',
            placeCount: '{count} places to visit',
            plate: 'Plate',
            placesToVisit: 'Places to visit',
            mapLoadError: 'The map could not be loaded.',
            backToMap: '← Back to map',
            category: 'Category',
            categories: 'Categories',
            all: 'All',
            locationMap: 'Location map',
            placesTitle: 'Places to visit',
            placesLoading: 'Loading places...',
            placesLoadError: 'Places could not be loaded.'
        },
        de: {
            language: 'Sprache',
            toggleTheme: 'Design wechseln',
            darkMode: 'Dunkler Modus',
            lightMode: 'Heller Modus',
            appTitle: 'Interaktive Karte der Türkei',
            appSubtitle: '81 Provinzen · Interaktives SVG-Dashboard',
            statsLabel: 'Überblick',
            totalProvinces: 'Provinzen',
            totalPopulation: 'Bevölkerung',
            mapType: 'Kartentyp',
            mapTitle: 'Karte der Republik Türkei',
            mapHint: 'Zeigen Sie auf eine Provinz oder klicken Sie, um Details zu sehen',
            mapAria: 'Karte der Türkei',
            selectPlaceholder: 'Klicken Sie auf eine Provinz, um Sehenswürdigkeiten zu entdecken.',
            footer: 'Türkei-Karte · ASP.NET Core MVC',
            homeTitle: 'Türkei-Karte',
            appName: 'Türkei-Karte',
            placeCount: '{count} Sehenswürdigkeiten',
            plate: 'Kennzeichen',
            placesToVisit: 'Sehenswürdigkeiten',
            mapLoadError: 'Die Karte konnte nicht geladen werden.',
            backToMap: '← Zurück zur Karte',
            category: 'Kategorie',
            categories: 'Kategorien',
            all: 'Alle',
            locationMap: 'Standortkarte',
            placesTitle: 'Sehenswürdigkeiten',
            placesLoading: 'Orte werden geladen...',
            placesLoadError: 'Orte konnten nicht geladen werden.'
        }
    };

    const REGIONS = {
        'Akdeniz': { en: 'Mediterranean', de: 'Mittelmeer' },
        'Ege': { en: 'Aegean', de: 'Ägäis' },
        'Marmara': { en: 'Marmara', de: 'Marmara' },
        'Karadeniz': { en: 'Black Sea', de: 'Schwarzes Meer' },
        'İç Anadolu': { en: 'Central Anatolia', de: 'Zentralanatolien' },
        'Doğu Anadolu': { en: 'Eastern Anatolia', de: 'Ostanatolien' },
        'Güneydoğu Anadolu': { en: 'Southeastern Anatolia', de: 'Südostanatolien' }
    };

    const CATEGORIES = {
        'Tarihi Yer': { en: 'Historic Site', de: 'Historischer Ort' },
        'Camii': { en: 'Mosque', de: 'Moschee' },
        'Müze': { en: 'Museum', de: 'Museum' },
        'Antik Kent': { en: 'Ancient City', de: 'Antike Stadt' },
        'Doğal Güzellik': { en: 'Natural Beauty', de: 'Natursehenswürdigkeit' },
        'Milli Park': { en: 'National Park', de: 'Nationalpark' },
        'Plaj': { en: 'Beach', de: 'Strand' }
    };

    let currentLang = 'tr';

    function normalize(lang) {
        return LANGS.indexOf(lang) === -1 ? 'tr' : lang;
    }

    function translate(key, vars) {
        const table = STRINGS[currentLang] || STRINGS.tr;
        let text = table[key] || STRINGS.tr[key] || key;
        if (vars) {
            Object.keys(vars).forEach(function (name) {
                text = text.replace('{' + name + '}', vars[name]);
            });
        }
        return text;
    }

    function named(dictionary, turkishName) {
        if (!turkishName || currentLang === 'tr') return turkishName || '';
        const entry = dictionary[turkishName];
        return (entry && entry[currentLang]) || turkishName;
    }

    function applyDocumentTitle() {
        if (!document.body) return;
        const key = document.body.getAttribute('data-document-title-key');
        const literal = document.body.getAttribute('data-document-title');
        if (key) {
            document.title = translate(key);
        } else if (literal) {
            document.title = literal + ' - ' + translate('appName');
        }
    }

    function apply() {
        document.documentElement.lang = currentLang;

        document.querySelectorAll('[data-i18n]').forEach(function (el) {
            el.textContent = translate(el.getAttribute('data-i18n'));
        });

        document.querySelectorAll('[data-i18n-aria]').forEach(function (el) {
            el.setAttribute('aria-label', translate(el.getAttribute('data-i18n-aria')));
        });

        document.querySelectorAll('[data-region]').forEach(function (el) {
            el.textContent = named(REGIONS, el.getAttribute('data-region'));
        });

        document.querySelectorAll('[data-i18n-category]').forEach(function (el) {
            el.textContent = named(CATEGORIES, el.getAttribute('data-category'));
        });

        document.querySelectorAll('[data-i18n-plate]').forEach(function (el) {
            el.textContent = translate('plate') + ': ' + (el.getAttribute('data-plate') || '');
        });

        const select = document.getElementById('lang-select');
        if (select && select.value !== currentLang) {
            select.value = currentLang;
        }

        applyDocumentTitle();
        document.dispatchEvent(new CustomEvent('languagechange', { detail: { lang: currentLang } }));
    }

    function setLang(lang) {
        currentLang = normalize(lang);
        localStorage.setItem(STORAGE_KEY, currentLang);
        apply();
    }

    currentLang = normalize(localStorage.getItem(STORAGE_KEY));
    document.documentElement.lang = currentLang;

    function bind() {
        const select = document.getElementById('lang-select');
        if (select) {
            select.addEventListener('change', function () {
                setLang(select.value);
            });
        }
        apply();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', bind);
    } else {
        bind();
    }

    window.TurkeyI18n = {
        getLang: function () { return currentLang; },
        setLang: setLang,
        t: translate,
        region: function (name) { return named(REGIONS, name); },
        category: function (name) { return named(CATEGORIES, name); }
    };
})();
