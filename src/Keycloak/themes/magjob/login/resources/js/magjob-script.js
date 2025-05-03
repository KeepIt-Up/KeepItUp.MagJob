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

  // Sprawdź, czy na stronie istnieje element z id "kc-error-message"
  if (document.getElementById('kc-error-message')) {
    // Dodaj klasę 'error-page' do body dokumentu
    document.body.classList.add('error-page');

    // Opcjonalnie: Sprawdź, czy istnieje element .card-image i ukryj go
    const cardImage = document.querySelector('.card-image');
    if (cardImage) {
      cardImage.style.display = 'none';
    }

    // Opcjonalnie: Znajdź .split-card i zastosuj stylowanie
    const splitCard = document.querySelector('.split-card');
    if (splitCard) {
      splitCard.style.flexDirection = 'column';
      splitCard.style.maxWidth = '500px';
      splitCard.style.margin = '0 auto';
    }

    // Opcjonalnie: Znajdź .card-content i zastosuj stylowanie
    const cardContent = document.querySelector('.card-content');
    if (cardContent) {
      cardContent.style.width = '100%';
      cardContent.style.display = 'flex';
      cardContent.style.flexDirection = 'column';
      cardContent.style.alignItems = 'center';
      cardContent.style.justifyContent = 'center';
      cardContent.style.textAlign = 'center';
    }
  }
});
