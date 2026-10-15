/**
 * Türkiye Haritası - İnteraktif SVG Harita Modülü
 * AJAX ile gezilecek yer sayıları, tooltip ve tıklama yönlendirmesi.
 */
(function () {
    'use strict';

    const SVG_URL = '/svg/turkey-map.svg';
    const PLACE_COUNTS_URL = '/api/cities/place-counts';
    const mapContainer = document.getElementById('map-container');
    const tooltip = document.getElementById('map-tooltip');
    const selectedPanel = document.getElementById('selected-city-panel');

    if (!mapContainer) return;

    /** @type {Record<string, {Code:string, Name:string, PlateCode:number, PlaceCount?:number}>} */
    let cityLookup = {};

    /** @type {Record<string, number>} */
    let placeCounts = {};

    /** @type {{Code:string, Name:string, PlateCode:number, PlaceCount?:number}|null} */
    let selectedCity = null;
    /** @type {{Code:string, Name:string, PlateCode:number, PlaceCount?:number}|null} */
    let tooltipCity = null;
    let mapFailed = false;

    function t(key, vars) {
        return window.TurkeyI18n ? TurkeyI18n.t(key, vars) : key;
    }

    /**
     * API yanıtındaki alan adını okur (camelCase / PascalCase uyumlu).
     */
    function readProp(obj, camelKey, pascalKey) {
        return obj[camelKey] !== undefined ? obj[camelKey] : obj[pascalKey];
    }

    /**
     * Sunucudan gelen il verilerini code bazlı sözlüğe dönüştürür.
     */
    function buildCityLookup() {
        const dataEl = document.getElementById('city-data');
        if (!dataEl) return;

        try {
            const cities = JSON.parse(dataEl.textContent);
            cityLookup = cities.reduce(function (acc, city) {
                const code = String(readProp(city, 'code', 'Code') || '').toUpperCase();
                acc[code] = {
                    Code: code,
                    Name: readProp(city, 'name', 'Name'),
                    PlateCode: readProp(city, 'plateCode', 'PlateCode'),
                    PlaceCount: readProp(city, 'placeCount', 'PlaceCount') || 0
                };
                return acc;
            }, {});

            // Sayfa yüklenirken gelen veriyi hemen kullan
            Object.keys(cityLookup).forEach(function (code) {
                placeCounts[code] = cityLookup[code].PlaceCount;
            });
        } catch (e) {
            console.error('İl verileri parse edilemedi:', e);
        }
    }

    /**
     * AJAX ile gezilecek yer sayılarını yükler (API camelCase döndürür).
     */
    async function loadPlaceCounts() {
        try {
            const response = await fetch(PLACE_COUNTS_URL);
            if (!response.ok) return;

            const counts = await response.json();
            counts.forEach(function (item) {
                const code = String(readProp(item, 'code', 'Code') || '').toUpperCase();
                const count = readProp(item, 'placeCount', 'PlaceCount') || 0;
                placeCounts[code] = count;
            });
        } catch (e) {
            console.error('Yer sayıları yüklenemedi:', e);
        }
    }

    function formatPlate(plateCode) {
        return String(plateCode).padStart(2, '0');
    }

    function getPlaceCount(city) {
        return placeCounts[city.Code.toUpperCase()] ?? city.PlaceCount ?? 0;
    }

    function showTooltip(event, city) {
        if (!tooltip) return;

        const count = getPlaceCount(city);

        tooltipCity = city;
        tooltip.innerHTML =
            '<p class="map-tooltip__name">' + city.Name + '</p>' +
            '<div class="map-tooltip__count">' + t('placeCount', { count: count }) + '</div>';

        moveTooltip(event);
        tooltip.classList.add('is-visible');
    }

    function moveTooltip(event) {
        if (!tooltip) return;

        const offsetX = 16;
        const offsetY = 16;
        const tooltipRect = tooltip.getBoundingClientRect();

        let x = event.clientX + offsetX;
        let y = event.clientY + offsetY;

        if (x + tooltipRect.width > window.innerWidth) {
            x = event.clientX - tooltipRect.width - offsetX;
        }
        if (y + tooltipRect.height > window.innerHeight) {
            y = event.clientY - tooltipRect.height - offsetY;
        }

        tooltip.style.left = x + 'px';
        tooltip.style.top = y + 'px';
    }

    function hideTooltip() {
        if (tooltip) tooltip.classList.remove('is-visible');
    }

    function updateSelectedPanel(city) {
        if (!selectedPanel) return;

        selectedCity = city;
        const count = getPlaceCount(city);

        selectedPanel.classList.add('has-selection');
        selectedPanel.innerHTML =
            '<div>' +
            '<p class="selected-city-panel__name">' + city.Name + '</p>' +
            '<div class="selected-city-panel__details">' +
            '<span>' + t('plate') + ': <strong>' + formatPlate(city.PlateCode) + '</strong></span>' +
            '<span>' + t('placesToVisit') + ': <strong>' + count + '</strong></span>' +
            '</div></div>';
    }

    function showMapError() {
        mapFailed = true;
        mapContainer.innerHTML =
            '<p class="map-load-error">' + t('mapLoadError') + '</p>';
    }

    function bindProvinceEvents(path, group) {
        const code = (path.dataset.code || '').toUpperCase();
        const city = cityLookup[code];
        if (!city) return;

        path.addEventListener('mouseenter', function (event) {
            group.classList.add('is-hovering');
            path.classList.add('is-hovered');
            showTooltip(event, city);
        });

        path.addEventListener('mousemove', function (event) {
            moveTooltip(event);
        });

        path.addEventListener('mouseleave', function () {
            group.classList.remove('is-hovering');
            path.classList.remove('is-hovered');
            hideTooltip();
        });

        path.addEventListener('click', function () {
            window.location.href = '/City/Details/' + city.PlateCode;
        });
    }

    async function loadMap() {
        mapContainer.classList.add('is-loading');

        try {
            const [svgResponse] = await Promise.all([
                fetch(SVG_URL),
                loadPlaceCounts()
            ]);

            if (!svgResponse.ok) throw new Error('SVG yüklenemedi');

            const svgText = await svgResponse.text();
            mapContainer.innerHTML = svgText;

            const group = mapContainer.querySelector('.provinces-group');
            if (!group) throw new Error('SVG yapısı geçersiz');

            group.querySelectorAll('.province').forEach(function (path) {
                bindProvinceEvents(path, group);
            });

            mapContainer.classList.remove('is-loading');
            mapContainer.classList.add('is-loaded');
        } catch (error) {
            console.error('Harita yükleme hatası:', error);
            mapContainer.classList.remove('is-loading');
            showMapError();
        }
    }

    document.addEventListener('languagechange', function () {
        if (mapFailed) {
            showMapError();
            return;
        }
        if (selectedCity) updateSelectedPanel(selectedCity);
        if (tooltip && tooltip.classList.contains('is-visible') && tooltipCity) {
            const count = getPlaceCount(tooltipCity);
            tooltip.innerHTML =
                '<p class="map-tooltip__name">' + tooltipCity.Name + '</p>' +
                '<div class="map-tooltip__count">' + t('placeCount', { count: count }) + '</div>';
        }
    });

    buildCityLookup();
    loadMap();
})();
