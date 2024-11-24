let map, drawnItems, drawControl;
let currentMarker = null;
let currentDrawing = null;

// Wait for DOM to be fully loaded before initializing
document.addEventListener('DOMContentLoaded', function () {
    initMap();
    setupFormListener();
});

function initMap() {
    map = L.map('map').setView([59.9139, 10.7522], 13);
    L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
        attribution: 'Map data © Kartverket'
    }).addTo(map);

    drawnItems = new L.FeatureGroup().addTo(map);

    // Initialize draw control (will be configured based on selection)
    drawControl = new L.Control.Draw({
        edit: {
            featureGroup: drawnItems
        },
        draw: {
            polygon: false,
            polyline: false,
            rectangle: false,
            circle: false,
            circlemarker: false,
            marker: false
        }
    });

    map.addControl(drawControl);

    map.on(L.Draw.Event.CREATED, handleDrawCreated);
    map.on('click', function (e) {
        fetchKommuneInfo(e.latlng.lat, e.latlng.lng);
    });

    loadExistingChanges();
    locateUser();
}

// Seamap functionality
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

function clearCurrentDrawing() {
    if (currentDrawing) {
        drawnItems.removeLayer(currentDrawing);
        currentDrawing = null;
    }
}

function handleDrawCreated(e) {
    clearCurrentDrawing();
    drawnItems.clearLayers();

    const layer = e.layer;
    drawnItems.addLayer(layer);
    currentDrawing = layer;

    if (e.layerType === 'marker') {
        const latLng = layer.getLatLng();
        document.getElementById('latitudeInput').value = latLng.lat;
        document.getElementById('longitudeInput').value = latLng.lng;
        fetchKommuneInfo(latLng.lat, latLng.lng);
    } else {
        if (layer.getBounds) {
            const bounds = layer.getBounds();
            const center = bounds.getCenter();
            document.getElementById('geoJsonInput').value = JSON.stringify(layer.toGeoJSON());
            fetchKommuneInfo(center.lat, center.lng);
        }
    }
}

function selectChangeType(type) {
    // Update UI
    document.querySelectorAll('.type-button').forEach(btn => {
        btn.classList.remove('active');
    });
   
    document.querySelector(`[data-type="${type}"]`).classList.add('active');

    // Update hidden input
    document.getElementById('changeType').value = type;

    // Clear existing drawings
    clearCurrentDrawing();
    drawnItems.clearLayers();

    // Reset form inputs
    document.getElementById('geoJsonInput').value = '';
    document.getElementById('latitudeInput').value = '';
    document.getElementById('longitudeInput').value = '';

    // Informs user of their decision
    if (type === 'point') {
        alert("Du har nå valgt Punktkorreksjon!");
    }
    else if (type === 'area') {
        alert("Du har nå valgt Områdeendring!");
    }

    // Configure draw control based on type
    if (type === 'point') {
        map.removeControl(drawControl);
        drawControl = new L.Control.Draw({
            edit: {
                featureGroup: drawnItems
            },
            draw: {
                polygon: false,
                polyline: false,
                rectangle: false,
                circle: false,
                marker: true,
                circlemarker: false,
            }
        });
        map.addControl(drawControl);

        showInstructions('Klikk på kartet der du vil plassere markøren for punktkorreksjonen.');
    } else if (type === 'area') {
        map.removeControl(drawControl);
        drawControl = new L.Control.Draw({
            edit: {
                featureGroup: drawnItems
            },
            draw: {
                polygon: true,
                polyline: true,
                rectangle: true,
                circle: false,
                marker: false,
                circlemarker: false,
            }
        });
        map.addControl(drawControl);

        showInstructions('Bruk tegneverktøyene i menyen for å markere området som skal endres.');
    }
}

function showInstructions(text) {
    const alert = document.getElementById('instructionsAlert');
    document.getElementById('instructionText').textContent = text;
    alert.classList.remove('d-none');
}

function dismissInstructions() {
    document.getElementById('instructionsAlert').classList.add('d-none');
}

function fetchKommuneInfo(lat, lon) {
    const kommuneInfoDiv = document.getElementById('kommuneInfo');
    kommuneInfoDiv.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';

    
    fetch(`https://api.kartverket.no/kommuneinfo/v1/punkt?nord=${lat}&ost=${lon}&koordsys=4326`)
        .then(response => response.json())
        .then(data => {
            if (data.kommunenavn) {
                
                kommuneInfoDiv.innerHTML = `
                    <div class="alert alert-info mb-0">
                        <strong>Kommune:</strong> ${data.kommunenavn}<br>
                        <strong>Kommune nr:</strong> ${data.kommunenummer}<br>
                        <strong>Koordinater:</strong> ${lat.toFixed(4)}, ${lon.toFixed(4)}
                    </div>`;
            } else {
                kommuneInfoDiv.innerHTML = '<div class="alert alert-warning mb-0">Kommune information not found</div>';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            kommuneInfoDiv.innerHTML = '<div class="alert alert-danger mb-0">Error fetching kommune information</div>';
        });
}

function loadExistingChanges() {
    fetch('/AreaChange/GetAllChanges')
        .then(response => response.json())
        .then(data => {
            if (data.areas) {
                data.areas.forEach(area => {
                    try {
                        const geoJson = JSON.parse(area.geoJson);
                        L.geoJSON(geoJson, {
                            style: {
                                color: '#FF4444',
                                weight: 2,
                                opacity: 0.7
                            }
                        }).addTo(map);
                    } catch (e) {
                        console.error('Error parsing GeoJSON:', e);
                    }
                });
            }

            if (data.points) {
                data.points.forEach(point => {
                    L.marker([point.latitude, point.longitude])
                        .bindPopup(point.description)
                        .addTo(map);
                });
            }

            const recentList = document.getElementById('recentChangesList');
            if (recentList) {
                recentList.innerHTML = '';
                [...(data.areas || []), ...(data.points || [])].sort((a, b) =>
                    new Date(b.submittedDate) - new Date(a.submittedDate)
                ).slice(0, 5).forEach(change => {
                    const item = document.createElement('a');
                    item.className = 'list-group-item list-group-item-action';
                    item.textContent = change.description;
                    recentList.appendChild(item);
                });
            }
        })
        .catch(error => {
            console.error('Error loading existing changes:', error);
        });
}

function locateUser() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            const lat = position.coords.latitude;
            const lon = position.coords.longitude;
            map.setView([lat, lon], 13);

            L.marker([lat, lon])
                .addTo(map)
                .bindPopup('Din Posisjon!')
                .openPopup();

            fetchKommuneInfo(lat, lon);
        }, function (error) {
            console.error('Geolocation error:', error);
        });
    }
}

function setupFormListener() {
    const form = document.getElementById('changeForm');
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            if (!document.getElementById('changeType').value) {
                alert('Vennligst velg en endringstype først.');
                return;
            }

            if (document.getElementById('changeType').value === 'point' &&
                (!document.getElementById('latitudeInput').value || !document.getElementById('longitudeInput').value)) {
                alert('Vennligst marker et punkt på kartet først.');
                return;
            }

            if (document.getElementById('changeType').value === 'area' &&
                !document.getElementById('geoJsonInput').value) {
                alert('Vennligst tegn et område på kartet først.');
                return;
            }

            const data = {
                type: document.getElementById('changeType').value,
                description: document.getElementById('description').value,
                geoJson: document.getElementById('geoJsonInput').value,
                latitude: document.getElementById('latitudeInput').value,
                longitude: document.getElementById('longitudeInput').value
            };

            fetch('/AreaChange/SaveChange', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            })
                .then(response => response.json())
                .then(result => {
                    if (result.success) {
                        alert('Change saved successfully!');
                        clearCurrentDrawing();
                        drawnItems.clearLayers();
                        form.reset();
                        document.getElementById('geoJsonInput').value = '';
                        document.getElementById('latitudeInput').value = '';
                        document.getElementById('longitudeInput').value = '';
                        loadExistingChanges();
                    } else {
                        alert('Error saving change: ' + result.message);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert('Error saving change');
                });
        });
    }
}
// I map-functions.js eller tilsvarende fil hvor du håndterer kartet
function removeMarker(id) {
    // Finn markøren basert på ID
    const marker = markers.find(m => m.id === id);
    if (marker) {
        // Fjern markøren fra kartet
        map.removeLayer(marker);
        // Fjern markøren fra markers-array
        markers = markers.filter(m => m.id !== id);
    }
}