/**
 * Tema yönetimi - Light / Dark Mode
 * Kullanıcı tercihi localStorage'da saklanır.
 */
(function () {
    'use strict';

    const STORAGE_KEY = 'turkey-map-theme';

    /**
     * Temayı DOM'a uygular.
     * @param {string} theme - 'light' veya 'dark'
     */
    function applyTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(STORAGE_KEY, theme);

        const toggleIcon = document.getElementById('theme-toggle-icon');
        const toggleLabel = document.getElementById('theme-toggle-label');

        if (toggleIcon) {
            toggleIcon.textContent = theme === 'dark' ? '☀️' : '🌙';
        }
        if (toggleLabel) {
            const darkLabel = window.TurkeyI18n ? TurkeyI18n.t('darkMode') : 'Koyu Mod';
            const lightLabel = window.TurkeyI18n ? TurkeyI18n.t('lightMode') : 'Açık Mod';
            toggleLabel.textContent = theme === 'dark' ? lightLabel : darkLabel;
        }
    }

    /**
     * Kayıtlı veya sistem temasını belirler.
     */
    function getInitialTheme() {
        const saved = localStorage.getItem(STORAGE_KEY);
        if (saved) return saved;

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    /**
     * Temayı değiştirir.
     */
    function toggleTheme() {
        const current = document.documentElement.getAttribute('data-theme') || 'light';
        applyTheme(current === 'light' ? 'dark' : 'light');
    }

    // Sayfa yüklenmeden temayı uygula (FOUC önleme)
    applyTheme(getInitialTheme());

    function bindThemeToggle() {
        const button = document.getElementById('theme-toggle');
        if (button && !button.dataset.bound) {
            button.dataset.bound = 'true';
            button.addEventListener('click', toggleTheme);
        }
        applyTheme(document.documentElement.getAttribute('data-theme') || getInitialTheme());
    }

    document.addEventListener('languagechange', function () {
        applyTheme(document.documentElement.getAttribute('data-theme') || 'light');
    });

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', bindThemeToggle);
    } else {
        bindThemeToggle();
    }
})();
