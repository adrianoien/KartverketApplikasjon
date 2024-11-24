// Global variables
let selectedCorrectionId = null;
let selectedCorrectionType = null;

// DOM Elements
const initializeElements = () => {
    return {
        modal: document.getElementById('assignModal'),
        closeBtn: document.querySelector('.close-button'),
        searchInput: document.getElementById('saksbehandlerSearch'),
        selectElement: document.getElementById('saksbehandlerSelect'),
        noResults: document.getElementById('noResults'),
        kommuneFilter: document.getElementById('kommuneFilter'),
        saveAssignmentBtn: document.getElementById('saveAssignment'),
        cancelAssignmentBtn: document.getElementById('cancelAssignment')
    };
};

// Municipality Functions
async function fetchMunicipality(lat, lng, elementId) {
    const params = new URLSearchParams({
        nord: lat.toString(),
        ost: lng.toString(),
        koordsys: '4326'
    });

    try {
        const response = await fetch(`https://api.kartverket.no/kommuneinfo/v1/punkt?${params.toString()}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        const kommuneText = data && data.kommunenavn ? `${data.kommunenummer} ${data.kommunenavn}` : 'Ikke funnet';
        document.getElementById(elementId).textContent = kommuneText;
        updateKommuneFilter();
    } catch (error) {
        console.error('Error fetching municipality:', error);
        document.getElementById(elementId).textContent = 'Kunne ikke bestemme kommune';
    }
}

// Filter Functions
function updateKommuneFilter() {
    const kommuneCells = document.querySelectorAll('tbody tr td:nth-child(2)');
    const kommuneSet = new Set();
    const elements = initializeElements();

    kommuneCells.forEach(cell => {
        const kommuneText = cell.textContent.trim();
        if (kommuneText && !['Laster...', 'Error', 'Ikke funnet', 'Kunne ikke bestemme kommune'].includes(kommuneText)) {
            const [nummer, ...navnDeler] = kommuneText.split(' ');
            const navn = navnDeler.join(' ');
            kommuneSet.add({ nummer, navn, fulltext: kommuneText });
        }
    });

    if (elements.kommuneFilter) {
        elements.kommuneFilter.innerHTML = '<option value="">Alle kommuner</option>';
        [...kommuneSet]
            .sort((a, b) => a.navn.localeCompare(b.navn))
            .forEach(kommune => {
                const option = document.createElement('option');
                option.value = kommune.fulltext;
                option.textContent = kommune.navn;
                elements.kommuneFilter.appendChild(option);
            });
    }
}

function filterByKommune(selectedKommune) {
    const rows = document.querySelectorAll('tbody tr');
    rows.forEach(row => {
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

// Modal Functions
function filterOptions(searchTerm) {
    const elements = initializeElements();
    let hasVisibleOptions = false;

    Array.from(elements.selectElement.options).forEach(option => {
        const name = option.getAttribute('data-name');
        const email = option.value.toLowerCase();
        const matches = name.includes(searchTerm) || email.includes(searchTerm);
        option.style.display = matches ? '' : 'none';
        if (matches) hasVisibleOptions = true;
    });

    elements.noResults.classList.toggle('d-none', hasVisibleOptions);
}

async function handleAssignmentSave() {
    const elements = initializeElements();
    const selectedHandlerId = elements.selectElement.value;

    if (!selectedHandlerId) {
        alert('Vennligst velg en saksbehandler');
        return;
    }

    try {
        const response = await fetch('/CorrectionManagement/AssignCase', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                correctionId: selectedCorrectionId,
                correctionType: selectedCorrectionType,
                assignTo: selectedHandlerId
            })
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
}

// Event Listeners
function initializeEventListeners() {
    const elements = initializeElements();

    // Municipality loading check
    const checkInterval = setInterval(() => {
        const loadingElements = document.querySelectorAll('td:nth-child(2)');
        const allLoaded = Array.from(loadingElements).every(el =>
            el.textContent.trim() !== 'Laster...' && el.textContent.trim() !== '');

        if (allLoaded) {
            updateKommuneFilter();
            clearInterval(checkInterval);
        }
    }, 500);

    setTimeout(() => clearInterval(checkInterval), 10000);

    // Modal event listeners
    document.addEventListener('click', function (e) {
        if (e.target.dataset.action === 'assign') {
            selectedCorrectionId = e.target.dataset.correctionId;
            selectedCorrectionType = e.target.dataset.correctionType;
            elements.modal.style.display = 'block';
            elements.searchInput.value = '';
            filterOptions('');
        }
    });

    elements.closeBtn.onclick = () => elements.modal.style.display = 'none';
    elements.cancelAssignmentBtn.onclick = () => elements.modal.style.display = 'none';

    window.onclick = (event) => {
        if (event.target == elements.modal) {
            elements.modal.style.display = 'none';
        }
    };

    // Search input handler
    let searchTimeout;
    elements.searchInput.addEventListener('input', function (e) {
        filterOptions(e.target.value.toLowerCase());
    });

    // Save assignment handler
    elements.saveAssignmentBtn.addEventListener('click', handleAssignmentSave);

    // Table observer
    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.type === 'childList' || mutation.type === 'characterData') {
                setTimeout(updateKommuneFilter, 500);
            }
        });
    });

    const tbody = document.querySelector('tbody');
    if (tbody) {
        observer.observe(tbody, {
            childList: true,
            subtree: true,
            characterData: true
        });
    }
}

// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', initializeEventListeners);

// Export functions that need to be called from Razor view
window.fetchMunicipality = fetchMunicipality;
window.filterByKommune = filterByKommune;