// Fetch municipality data for a given latitude and longitude
export async function fetchMunicipality(lat, lng, elementId) {
    const params = new URLSearchParams({
        nord: lat.toString(),
        ost: lng.toString(),
        koordsys: '4326',
    });

    try {
        const response = await fetch(`https://api.kartverket.no/kommuneinfo/v1/punkt?${params.toString()}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        const kommuneText = data?.kommunenavn ? `${data.kommunenummer} ${data.kommunenavn}` : 'Ikke funnet';
        document.getElementById(elementId).textContent = kommuneText;

        // Call updateKommuneFilter after data is loaded
        updateKommuneFilter();
    } catch (error) {
        console.error('Error fetching municipality:', error);
        document.getElementById(elementId).textContent = 'Kunne ikke bestemme kommune';
    }
}

// Fetch municipality data for an area change using GeoJSON
export function fetchMunicipalityForArea(geoJson, elementId) {
    try {
        const parsedGeoJson = typeof geoJson === 'string' ? JSON.parse(geoJson) : geoJson;
        if (parsedGeoJson.geometry?.coordinates?.[0]) {
            const coords = parsedGeoJson.geometry.coordinates[0];
            const centerLat = coords.reduce((sum, coord) => sum + coord[1], 0) / coords.length;
            const centerLon = coords.reduce((sum, coord) => sum + coord[0], 0) / coords.length;
            fetchMunicipality(centerLat, centerLon, elementId);
        }
    } catch (error) {
        console.error('Error parsing GeoJSON:', error);
        document.getElementById(elementId).textContent = 'Error';
    }
}

// Update the municipality filter dropdown
export function updateKommuneFilter() {
    const kommuneCells = document.querySelectorAll('tbody tr td:nth-child(2)');
    const kommuneSet = new Set();

    kommuneCells.forEach((cell) => {
        const kommuneText = cell.textContent.trim();
        if (
            kommuneText &&
            kommuneText !== 'Laster...' &&
            kommuneText !== 'Error' &&
            kommuneText !== 'Ikke funnet' &&
            kommuneText !== 'Kunne ikke bestemme kommune'
        ) {
            const [nummer, ...navnDeler] = kommuneText.split(' ');
            const navn = navnDeler.join(' ');
            kommuneSet.add({ nummer, navn, fulltext: kommuneText });
        }
    });

    const kommuneFilter = document.getElementById('kommuneFilter');
    if (kommuneFilter) {
        kommuneFilter.innerHTML = '<option value="">Alle kommuner</option>'; // Keep default option

        [...kommuneSet]
            .sort((a, b) => a.navn.localeCompare(b.navn))
            .forEach((kommune) => {
                const option = document.createElement('option');
                option.value = kommune.fulltext;
                option.textContent = kommune.navn;
                kommuneFilter.appendChild(option);
            });
    }
}

// Filter rows by the selected municipality
export function filterByKommune(selectedKommune) {
    const rows = document.querySelectorAll('tbody tr');
    rows.forEach((row) => {
        const kommuneCell = row.querySelector('td:nth-child(2)');
        if (kommuneCell) {
            if (!selectedKommune || kommuneCell.textContent.trim().includes(selectedKommune)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        }
    });
}

// Initialize modal functionality
export function initializeModalHandlers(modal, closeBtn, searchInput, selectElement, noResults) {
    let selectedCorrectionId = null;
    let selectedCorrectionType = null;

    // Show modal when "Assign Case" is clicked
    document.addEventListener('click', (e) => {
        if (e.target.dataset.action === 'assign') {
            selectedCorrectionId = e.target.dataset.correctionId;
            selectedCorrectionType = e.target.dataset.correctionType;
            modal.style.display = 'block';
            searchInput.value = '';
            filterOptions('');
        }
    });

    // Handle search input
    searchInput.addEventListener('input', (e) => {
        filterOptions(e.target.value.toLowerCase());
    });

    // Close modal handlers
    closeBtn.onclick = () => {
        modal.style.display = 'none';
    };

    document.getElementById('cancelAssignment').onclick = () => {
        modal.style.display = 'none';
    };

    // Close modal if clicking outside it
    window.onclick = (event) => {
        if (event.target === modal) {
            modal.style.display = 'none';
        }
    };

    // Save assignment handler
    document.getElementById('saveAssignment').addEventListener('click', async () => {
        const selectedHandlerId = selectElement.value;
        if (!selectedHandlerId) {
            alert('Vennligst velg en saksbehandler');
            return;
        }

        try {
            const response = await fetch('/CorrectionManagement/AssignCase', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    correctionId: selectedCorrectionId,
                    correctionType: selectedCorrectionType,
                    assignTo: selectedHandlerId,
                }),
            });

            if (response.ok) {
                location.reload();
            } else {
                alert('Feil ved tildeling av sak');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Feil ved tildeling av sak');
        }
    });

    // Filter options based on search input
    function filterOptions(searchTerm) {
        let hasVisibleOptions = false;
        Array.from(selectElement.options).forEach((option) => {
            const name = option.getAttribute('data-name');
            const email = option.value.toLowerCase();
            const matches = name.includes(searchTerm) || email.includes(searchTerm);
            option.style.display = matches ? '' : 'none';
            if (matches) hasVisibleOptions = true;
        });

        noResults.classList.toggle('d-none', hasVisibleOptions);
    }
}