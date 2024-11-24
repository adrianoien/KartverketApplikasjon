document.addEventListener('DOMContentLoaded', function () {
    console.log('Model Type:', mapModelData.type);
    console.log('Current Working URL:', window.location.href);

    // Checks if the map allready exist
    if (typeof map === 'undefined') {
        // If not, initializes a new map
        var map = L.map('map').setView([62.0, 10.0], 4);

        // Uses kartverkest tile layer
        L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
            attribution: 'Map data © Kartverket',
            maxZoom: 18,
            minZoom: 3
        }).addTo(map);
    } else {
        console.log("Kartet er allerede initialisert.");
    }

    // Seamap
    // Checks if the maplayers allready is defined 
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

    // Defines the Leaflet-controller
    var SjoKartControl = L.Control.extend({
        options: {
            position: 'topleft' // Placement in the map
        },
        onAdd: function (map) {
            var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control leaflet-control-custom');

            // Styling of the controler
            container.style.backgroundColor = 'var(--leaflet-control-bg, white)';
            container.style.width = 'var(--leaflet-control-size, 2.5rem)';
            container.style.height = 'var(--leaflet-control-size, 2.5rem)';
            container.style.cursor = 'pointer';
            container.style.display = 'flex';
            container.style.alignItems = 'center';
            container.style.justifyContent = 'center';
            container.style.fontSize = 'var(--leaflet-control-font-size, 1rem)';
            container.innerHTML = '🌊';

            // Clicking action to change maplayer
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

    // Adds the controller in the map
    map.addControl(new SjoKartControl());

    // Function to style GeoJSON
    function styleGeoJson() {
        return {
            color: '#FF4444',
            weight: 3,
            opacity: 0.7,
            fillOpacity: 0.3
        };
    }

    if (mapModelData.type === "map") {
        (function () {
            // Standardcoordinates for Norway when no other position is given
            var defaultLat = 62.0;
            var defaultLng = 10.0;

            // Gets coordinates from the model
            var lat = mapModelData.latitude;
            var lng = mapModelData.longitude;

            console.log('Råkoordinater:', { lat, lng });

            // Parse coordinates, uses standard if invalid
            lat = !isNaN(parseFloat(lat)) ? parseFloat(lat) : defaultLat;
            lng = !isNaN(parseFloat(lng)) ? parseFloat(lng) : defaultLng;

            console.log('Parsed koordinater:', { lat, lng });

            map.whenReady(function () {
                console.log('Setter kartvisning til:', { lat, lng });
                map.setView([lat, lng], 15); // Set zoom level to 15 for better view

                if (lat !== defaultLat || lng !== defaultLng) {
                    L.marker([lat, lng])
                        .addTo(map)
                        .bindPopup(mapModelData.description)
                        .openPopup();
                }

                // Gets communicationinformation 
                if (lat !== defaultLat && lng !== defaultLng) {
                    const params = new URLSearchParams({
                        nord: lat.toString(),
                        ost: lng.toString(),
                        koordsys: '4326'
                    });

                    const apiUrl = `https://api.kartverket.no/kommuneinfo/v1/punkt?${params.toString()}`;
                    console.log('Henter kommuneinformasjon fra:', apiUrl);

                    fetch(apiUrl)
                        .then(response => {
                            console.log('Kommune API-respons:', response.status);
                            if (!response.ok) {
                                throw new Error(`HTTP-feil! status: ${response.status}`);
                            }
                            return response.json();
                        })
                        .then(data => {
                            console.log('Kommuneinformasjon:', data);
                            document.getElementById('municipality').textContent =
                                data && data.kommunenavn ? data.kommunenavn : 'Ikke funnet';
                        })
                        .catch(function (error) {
                            console.error('Feil ved henting av kommune:', error);
                            document.getElementById('municipality').textContent =
                                'Kunne ikke bestemme kommune';
                        });
                } else {
                    document.getElementById('municipality').textContent = 'Ingen lokasjon spesifisert';
                }
            });
        })();
    } else if (mapModelData.type === "area") {
        try {
            // Use GeoJSON directly if it's an object; parse it if it's a string
            var geoJsonData = typeof mapModelData.geoJson === "string"
                ? JSON.parse(mapModelData.geoJson)
                : mapModelData.geoJson;

            console.log("Parsed GeoJSON data:", geoJsonData);

            // Add GeoJSON to map
            var geoJsonLayer = L.geoJSON(geoJsonData, {
                style: styleGeoJson,
                onEachFeature: function (feature, layer) {
                    layer.bindPopup(mapModelData.description);
                }
            }).addTo(map);

            // Fit the map to the GeoJSON bounds
            var bounds = geoJsonLayer.getBounds();
            map.fitBounds(bounds, { padding: [50, 50] });

            // Fetch municipality info for the center of the bounds
            var center = bounds.getCenter();
            const params = new URLSearchParams({
                nord: center.lat.toString(),
                ost: center.lng.toString(),
                koordsys: "4326"
            });

            fetch(`https://api.kartverket.no/kommuneinfo/v1/punkt?${params.toString()}`)
                .then(response => response.json())
                .then(data => {
                    document.getElementById("municipality").textContent =
                        data && data.kommunenavn ? data.kommunenavn : "Not found";
                })
                .catch(error => {
                    console.error("Error fetching municipality info:", error);
                    document.getElementById("municipality").textContent =
                        "Unable to determine municipality";
                });
        } catch (error) {
            console.error("Error parsing GeoJSON or displaying area:", error);
            map.setView([59.9139, 10.7522], 4); // Default to Oslo
            document.getElementById("municipality").textContent = "Error displaying area";
        }
    }
});
