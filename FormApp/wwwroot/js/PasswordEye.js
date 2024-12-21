document.querySelectorAll('.input-group').forEach(group => {
    const togglePasswordButton = group.querySelector('#togglePassword');
    const passwordInput = group.querySelector('input[type="password"]');
    const toggleIcon = group.querySelector('#toggleIcon');

    togglePasswordButton.addEventListener('click', function () {
        const type = passwordInput.type === 'password' ? 'text' : 'password';
        passwordInput.type = type;
        toggleIcon.classList.toggle('bi-eye');
        toggleIcon.classList.toggle('bi-eye-slash');
    });
});