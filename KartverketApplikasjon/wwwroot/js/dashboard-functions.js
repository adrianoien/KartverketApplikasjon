// Fetch municipality data based on latitude and longitude
async function fetchMunicipality(lat, lng, elementId) {
    const params = new URLSearchParams({
        nord: lat.toString(),
        ost: lng.toString(),
        koordsys: '4326',
    });

    try {
        const response = await fetch(`https://api.kartverket.no/kommuneinfo/v1/punkt?${params.toString()}`);
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        document.getElementById(elementId).textContent =
            data && data.kommunenavn ? `${data.kommunenummer} ${data.kommunenavn}` : 'Ikke funnet';
    } catch (error) {
        console.error('Error fetching municipality:', error);
        document.getElementById(elementId).textContent = 'Kunne ikke bestemme kommune';
    }
}

// Fetch municipalities for a correction (latitude/longitude-based)
function fetchMunicipalityForCorrection(lat, lng, elementId) {
    fetchMunicipality(lat, lng, elementId);
}

// Fetch municipalities for an area change (GeoJSON-based)
function fetchMunicipalityForArea(geoJson, elementId) {
    try {
        // Parse GeoJSON if it's a string
        const geoJsonData = typeof geoJson === 'string' ? JSON.parse(geoJson) : geoJson;

        if (
            geoJsonData.geometry &&
            geoJsonData.geometry.coordinates &&
            geoJsonData.geometry.coordinates[0]
        ) {
            const coords = geoJsonData.geometry.coordinates[0];
            const centerLat = coords.reduce((sum, coord) => sum + coord[1], 0) / coords.length;
            const centerLon = coords.reduce((sum, coord) => sum + coord[0], 0) / coords.length;

            // Use fetchMunicipality for center coordinates
            fetchMunicipality(centerLat, centerLon, elementId);
        }
    } catch (error) {
        console.error('Error parsing GeoJSON:', error);
        document.getElementById(elementId).textContent = 'Error';
    }
}
