// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
  // Add focus and blur events to form inputs to enhance UX
  const formInputs = document.querySelectorAll('.form-control');

  if (formInputs) {
    formInputs.forEach(input => {
      // Add a class when the input is focused
      input.addEventListener('focus', function() {
        this.parentNode.classList.add('is-focused');
      });

      // Remove the class when focus is lost, but keep it if there's a value
      input.addEventListener('blur', function() {
        this.parentNode.classList.remove('is-focused');
        if (this.value) {
          this.parentNode.classList.add('has-value');
        } else {
          this.parentNode.classList.remove('has-value');
        }
      });

      // Check initial state (useful if form is pre-filled)
      if (input.value) {
        input.parentNode.classList.add('has-value');
      }
    });
  }

  // Handle togglePassword click events - moved to inline scripts in templates for better reliability
});