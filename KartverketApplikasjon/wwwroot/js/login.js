document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById('loginModal');
    var btn = document.getElementById('loginButton');
    var navBtn = document.getElementById('navLoginButton');
    var span = document.getElementsByClassName('close')[0];

    function openModal() {
        modal.style.display = 'block';
    }

    function closeModal() {
        modal.style.display = 'none';
    }

    btn.onclick = openModal;
    navBtn.onclick = openModal;
    span.onclick = closeModal;

    window.onclick = function (event) {
        if (event.target == modal) {
            closeModal();
        }
    }

    var form = document.getElementById('loginForm');
    form.onsubmit = function (e) {
        e.preventDefault();
        
        // Log inn data is sent here, temporarily logs to console
        console.log('Login attempted');
        closeModal();
    }
});