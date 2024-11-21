document.addEventListener('DOMContentLoaded', function () {
    // Sjekker om kartet er initialisert
    if (typeof map === 'undefined') {
        console.error('Kartet (map) er ikke initialisert! Sørg for at "map-functions.js" kjører før denne filen.');
        return;
    }

    // Sjekker om kartlagene allerede er definert
    if (typeof standardTileLayer === 'undefined') {
        var standardTileLayer = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);
    }

    if (typeof sjokartTileLayer === 'undefined') {
        var sjokartTileLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
            attribution: '&copy; <a href="http://www.kartverket.no/">Kartverket</a>'
        });
    }

    // Definerer Leaflet-kontrollen
    var SjoKartControl = L.Control.extend({
        options: {
            position: 'topleft' // Plassering i kartet
        },
        onAdd: function (map) {
            var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control leaflet-control-custom');

            // Styling for kontrollen
            container.style.backgroundColor = 'var(--leaflet-control-bg, white)';
            container.style.width = 'var(--leaflet-control-size, 2.5rem)';
            container.style.height = 'var(--leaflet-control-size, 2.5rem)';
            container.style.cursor = 'pointer';
            container.style.display = 'flex';
            container.style.alignItems = 'center';
            container.style.justifyContent = 'center';
            container.style.fontSize = 'var(--leaflet-control-font-size, 1rem)';
            container.innerHTML = '🌊';

            // Klikkhendelse for å bytte kartlag
            container.onclick = function () {
                if (map.hasLayer(sjokartTileLayer)) {
                    map.removeLayer(sjokartTileLayer);
                    map.addLayer(standardTileLayer);
                    container.innerHTML = '⛰️';
                } else {
                    map.removeLayer(standardTileLayer);
                    map.addLayer(sjokartTileLayer);
                    container.innerHTML = '🌊';
                }
            };
            return container;
        }
    });

    // Legger til kontrollen i kartet
    map.addControl(new SjoKartControl());
});
