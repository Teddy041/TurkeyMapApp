/**
 * Şehir Detay Sayfası - Leaflet harita ve lazy-load yer listesi
 */
(function () {
    'use strict';

    const mapEl = document.getElementById('leaflet-map');
    const placesGrid = document.getElementById('places-grid');
    const loadingEl = document.getElementById('places-loading');

    if (!mapEl) return;

    const plateCode = mapEl.dataset.plateCode;
    let allPlaces = [];
    let leafletMap = null;
    let markersLayer = null;

    /**
     * Noktaları haritaya ekler.
     * @param {Array} places
     */
    function renderMapMarkers(places) {
        if (!leafletMap) {
            leafletMap = L.map('leaflet-map').setView([39.0, 35.0], 6);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
                maxZoom: 18
            }).addTo(leafletMap);
            markersLayer = L.layerGroup().addTo(leafletMap);
        }

        markersLayer.clearLayers();

        if (places.length === 0) return;

        const bounds = [];

        places.forEach(function (place) {
            const marker = L.marker([place.latitude, place.longitude]);
            marker.bindPopup(
                '<strong>' + escapeHtml(place.name) + '</strong>' +
                '<p>' + escapeHtml(place.description) + '</p>' +
                '<p><em>' + escapeHtml(place.address) + '</em></p>'
            );
            markersLayer.addLayer(marker);
            bounds.push([place.latitude, place.longitude]);
        });

        if (bounds.length === 1) {
            leafletMap.setView(bounds[0], 13);
        } else if (bounds.length > 1) {
            leafletMap.fitBounds(bounds, { padding: [40, 40] });
        }
    }

    /**
     * API yanıt alanlarını okur (camelCase / PascalCase).
     */
    function getProp(obj, camelKey, pascalKey) {
        return obj[camelKey] !== undefined ? obj[camelKey] : obj[pascalKey];
    }

    /**
     * Kırık görseller için yerel SVG yedeğine geçer.
     */
    function bindImageFallbacks(container) {
        if (!container) return;

        container.querySelectorAll('img.place-card__image').forEach(function (img) {
            img.addEventListener('error', function handleError() {
                var fallback = img.dataset.fallback;
                if (fallback && img.src.indexOf(fallback) === -1) {
                    img.src = fallback;
                }
            }, { once: true });
        });
    }

    /**
     * Yer kartlarını render eder.
     * @param {Array} places
     */
    function renderPlaceCards(places) {
        if (!placesGrid) return;

        placesGrid.innerHTML = places.map(function (place, index) {
            var imageUrl = getProp(place, 'imageUrl', 'ImageUrl');
            var fallbackUrl = getProp(place, 'fallbackImageUrl', 'FallbackImageUrl') ||
                '/images/placeholders/default.svg';

            return '<article class="place-card" style="animation-delay:' + (index * 40) + 'ms">' +
                '<img class="place-card__image" src="' + escapeHtml(imageUrl) + '"' +
                ' data-fallback="' + escapeHtml(fallbackUrl) + '"' +
                ' alt="' + escapeHtml(getProp(place, 'name', 'Name')) + '" loading="lazy" />' +
                '<div class="place-card__body">' +
                '<span class="place-card__category">' + escapeHtml(categoryLabel(getProp(place, 'category', 'Category'))) + '</span>' +
                '<h3 class="place-card__name">' + escapeHtml(getProp(place, 'name', 'Name')) + '</h3>' +
                '<p class="place-card__desc">' + escapeHtml(getProp(place, 'description', 'Description')) + '</p>' +
                '<p class="place-card__address">📍 ' + escapeHtml(getProp(place, 'address', 'Address')) + '</p>' +
                '<p class="place-card__coords">' + place.latitude.toFixed(5) + ', ' + place.longitude.toFixed(5) + '</p>' +
                '</div></article>';
        }).join('');

        bindImageFallbacks(placesGrid);
    }

    /**
     * XSS koruması için HTML escape.
     */
    function categoryLabel(name) {
        return window.TurkeyI18n ? TurkeyI18n.category(name) : name;
    }

    function activeCategory() {
        var active = document.querySelector('.category-chip--active');
        return active ? active.dataset.category : 'all';
    }

    function placesForCategory(category) {
        return category === 'all'
            ? allPlaces
            : allPlaces.filter(function (p) { return p.category === category; });
    }

    function escapeHtml(text) {
        var div = document.createElement('div');
        div.textContent = text || '';
        return div.innerHTML;
    }

    /**
     * Kategori filtreleme.
     */
    function bindCategoryFilters() {
        var chips = document.querySelectorAll('.category-chip');
        chips.forEach(function (chip) {
            chip.addEventListener('click', function () {
                chips.forEach(function (c) { c.classList.remove('category-chip--active'); });
                chip.classList.add('category-chip--active');

                var filtered = placesForCategory(chip.dataset.category);
                renderPlaceCards(filtered);
                renderMapMarkers(filtered);
            });
        });
    }

    /**
     * AJAX ile yerleri lazy-load eder.
     */
    async function loadPlaces() {
        try {
            var response = await fetch('/api/cities/' + plateCode + '/places');
            if (!response.ok) throw new Error('Veri yüklenemedi');

            var data = await response.json();
            allPlaces = data.places || [];

            if (loadingEl) loadingEl.style.display = 'none';
            renderPlaceCards(allPlaces);
            renderMapMarkers(allPlaces);
            bindCategoryFilters();
        } catch (error) {
            console.error('Yerler yüklenemedi:', error);
            if (loadingEl) {
                loadingEl.setAttribute('data-i18n', 'placesLoadError');
                loadingEl.textContent = window.TurkeyI18n
                    ? TurkeyI18n.t('placesLoadError')
                    : 'Yerler yüklenirken bir hata oluştu.';
            }
        }
    }

    document.addEventListener('languagechange', function () {
        if (!allPlaces.length) return;
        var filtered = placesForCategory(activeCategory());
        renderPlaceCards(filtered);
    });

    loadPlaces();
})();
