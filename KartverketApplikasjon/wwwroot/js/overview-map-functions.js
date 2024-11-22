document.addEventListener('DOMContentLoaded', function () {
    // Ensure the map div is present in the DOM
    if (!document.getElementById('map')) {
        console.error('Map element (#map) not found.');
        return;
    }

    // Initialize the map and set its view to a default location (Oslo)
    var map = L.map('map').setView([59.9139, 10.7522], 13);

    // Define the standard map layer
    var standardTileLayer = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);

    // Define the sea map layer
    var sjokartTileLayer = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
        attribution: '&copy; <a href="http://www.kartverket.no/">Kartverket</a>'
    });

    // Add the custom control to toggle between the layers
    var SjoKartControl = L.Control.extend({
        options: {
            position: 'topleft' // Position in the map
        },
        onAdd: function (map) {
            // Create a control button
            var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control leaflet-control-custom');

            // Style the button
            container.style.backgroundColor = 'var(--leaflet-control-bg, white)';
            container.style.width = '40px';
            container.style.height = '40px';
            container.style.cursor = 'pointer';
            container.style.display = 'flex';
            container.style.alignItems = 'center';
            container.style.justifyContent = 'center';
            container.style.fontSize = '20px';
            container.innerHTML = '🌊'; // Default to seamap icon

            // Handle click to toggle layers
            container.onclick = function () {
                if (map.hasLayer(sjokartTileLayer)) {
                    map.removeLayer(sjokartTileLayer);
                    map.addLayer(standardTileLayer);
                    container.innerHTML = '⛰️'; // Change to standard map icon
                } else {
                    map.removeLayer(standardTileLayer);
                    map.addLayer(sjokartTileLayer);
                    container.innerHTML = '🌊'; // Change to sea map icon
                }
            };

            return container;
        }
    });

    // Add the control to the map
    map.addControl(new SjoKartControl());

    // Add geocoder control to the map
    L.Control.geocoder().addTo(map);

    // Optional: Handle geolocation to center the map
    navigator.geolocation.getCurrentPosition(function (position) {
        var lat = position.coords.latitude;
        var lon = position.coords.longitude;

        // Set the map's view to user's current location
        map.setView([lat, lon], 13);

        // Add a marker at the user's location
        L.marker([lat, lon]).addTo(map)
            .bindPopup('Du er her!').openPopup();
    }, function (error) {
        console.error('Geolocation failed: ', error);
    });
});
