<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('firstName','lastName','email','username','password','password-confirm') templateType="register"; section>
    <#if section = "header">
        ${msg("registerTitle")}
    <#elseif section = "form">
        <form id="kc-register-form" class="register-form" action="${url.registrationAction}" method="post">

            <div class="form-group ${messagesPerField.printIfExists('firstName','has-error')}">
                <label for="firstName" class="control-label">${msg("firstName")}</label>
                <input type="text" id="firstName" class="form-control" name="firstName"
                       value="${(register.formData.firstName!'')}"
                       aria-invalid="<#if messagesPerField.existsError('firstName')>true</#if>"
                       autocomplete="given-name"
                />
                <#if messagesPerField.existsError('firstName')>
                    <span id="input-error-firstname" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('firstName'))?no_esc}
                    </span>
                </#if>
            </div>

            <div class="form-group ${messagesPerField.printIfExists('lastName','has-error')}">
                <label for="lastName" class="control-label">${msg("lastName")}</label>
                <input type="text" id="lastName" class="form-control" name="lastName"
                       value="${(register.formData.lastName!'')}"
                       aria-invalid="<#if messagesPerField.existsError('lastName')>true</#if>"
                       autocomplete="family-name"
                />
                <#if messagesPerField.existsError('lastName')>
                    <span id="input-error-lastname" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('lastName'))?no_esc}
                    </span>
                </#if>
            </div>

            <div class="form-group ${messagesPerField.printIfExists('email','has-error')}">
                <label for="email" class="control-label">${msg("email")}</label>
                <input type="email" id="email" class="form-control" name="email"
                       value="${(register.formData.email!'')}"
                       aria-invalid="<#if messagesPerField.existsError('email')>true</#if>"
                       autocomplete="email"
                />
                <#if messagesPerField.existsError('email')>
                    <span id="input-error-email" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('email'))?no_esc}
                    </span>
                </#if>
            </div>

            <#if !realm.registrationEmailAsUsername>
                <div class="form-group ${messagesPerField.printIfExists('username','has-error')}">
                    <label for="username" class="control-label">${msg("username")}</label>
                    <input type="text" id="username" class="form-control" name="username"
                           value="${(register.formData.username!'')}"
                           aria-invalid="<#if messagesPerField.existsError('username')>true</#if>"
                           autocomplete="username"
                    />
                    <#if messagesPerField.existsError('username')>
                        <span id="input-error-username" class="error-message" aria-live="polite">
                            ${kcSanitize(messagesPerField.get('username'))?no_esc}
                        </span>
                    </#if>
                </div>
            </#if>

            <div class="form-group ${messagesPerField.printIfExists('password','has-error')}">
                <label for="password" class="control-label">${msg("password")}</label>
                <div class="password-input-wrapper">
                    <input type="password" id="password" class="form-control" name="password"
                           aria-invalid="<#if messagesPerField.existsError('password')>true</#if>"
                           autocomplete="new-password"
                    />
                    <div class="password-toggle-icon" onclick="togglePassword('password', 'togglePasswordIcon')">
                        <svg id="togglePasswordIcon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="eye-icon">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                            <circle cx="12" cy="12" r="3"></circle>
                        </svg>
                    </div>
                </div>
                <div class="password-strength-container" id="password-strength-container">
                    <div class="password-strength-meter">
                        <div class="password-strength-meter-fill" id="password-strength-meter-fill"></div>
                    </div>
                    <div class="password-strength-text" id="password-strength-text">Password strength</div>
                </div>
                <#if messagesPerField.existsError('password')>
                    <span id="input-error-password" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('password'))?no_esc}
                    </span>
                </#if>
            </div>

            <div class="form-group ${messagesPerField.printIfExists('password-confirm','has-error')}">
                <label for="password-confirm" class="control-label">${msg("passwordConfirm")}</label>
                <div class="password-input-wrapper">
                    <input type="password" id="password-confirm" class="form-control"
                           name="password-confirm"
                           aria-invalid="<#if messagesPerField.existsError('password-confirm')>true</#if>"
                           autocomplete="new-password"
                    />
                    <div class="password-toggle-icon" onclick="togglePassword('password-confirm', 'toggleConfirmPasswordIcon')">
                        <svg id="toggleConfirmPasswordIcon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="eye-icon">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                            <circle cx="12" cy="12" r="3"></circle>
                        </svg>
                    </div>
                </div>
                <#if messagesPerField.existsError('password-confirm')>
                    <span id="input-error-password-confirm" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('password-confirm'))?no_esc}
                    </span>
                </#if>
            </div>

            <#if recaptchaRequired??>
                <div class="form-group">
                    <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}"></div>
                </div>
            </#if>

            <div id="kc-form-buttons" class="form-group">
                <input class="btn btn-primary btn-md" type="submit" value="${msg("doRegister")}"/>
            </div>

            <div class="login-pf-signup">
                <span>Already have an account? <a href="${url.loginUrl}">${msg("doLogIn")}</a></span>
            </div>
        </form>
    </#if>

    <script>
    function togglePassword(passwordId, toggleIconId) {
        var passwordField = document.getElementById(passwordId);
        var toggleIcon = document.getElementById(toggleIconId);

        if (passwordField && toggleIcon) {
            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                toggleIcon.innerHTML = '<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle><line x1="1" y1="1" x2="23" y2="23"></line>';
                toggleIcon.parentNode.title = "Hide password";
            } else {
                passwordField.type = 'password';
                toggleIcon.innerHTML = '<path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle>';
                toggleIcon.parentNode.title = "Show password";
            }
        }
    }

    // Password Strength Indicator
    document.addEventListener("DOMContentLoaded", function() {
        const passwordField = document.getElementById('password');
        const strengthContainer = document.getElementById('password-strength-container');
        const strengthMeter = document.getElementById('password-strength-meter-fill');
        const strengthText = document.getElementById('password-strength-text');

        if (passwordField && strengthContainer && strengthMeter && strengthText) {
            passwordField.addEventListener('input', function() {
                const password = passwordField.value;

                if (password.length === 0) {
                    strengthContainer.classList.remove('active');
                    return;
                }

                strengthContainer.classList.add('active');
                const strength = calculatePasswordStrength(password);
                updateStrengthMeter(strength);
            });
        }

        function calculatePasswordStrength(password) {
            // Basic criteria
            let score = 0;

            // Length check (max 4 points)
            if (password.length >= 6) score += 1;
            if (password.length >= 8) score += 1;
            if (password.length >= 10) score += 1;
            if (password.length >= 12) score += 1;

            // Complexity checks
            const patterns = [
                /[a-z]/, // lowercase
                /[A-Z]/, // uppercase
                /[0-9]/, // numbers
                /[^a-zA-Z0-9]/ // special chars
            ];

            // Award 1 point for each pattern matched
            patterns.forEach(pattern => {
                if (pattern.test(password)) score += 1;
            });

            // Bonus for mixed characters
            if (password.match(/[a-z]/) && password.match(/[A-Z]/)) score += 1;
            if (password.match(/[a-zA-Z]/) && password.match(/[0-9]/)) score += 1;
            if (password.match(/[a-zA-Z0-9]/) && password.match(/[^a-zA-Z0-9]/)) score += 1;

            // Return score out of 10
            return Math.min(score, 10);
        }

        function updateStrengthMeter(score) {
            // Define strength levels and corresponding UI updates
            let strengthClass, strengthMessage;

            if (score <= 2) {
                strengthClass = 'very-weak';
                strengthMessage = 'Very Weak';
            } else if (score <= 4) {
                strengthClass = 'weak';
                strengthMessage = 'Weak';
            } else if (score <= 6) {
                strengthClass = 'medium';
                strengthMessage = 'Medium';
            } else if (score <= 8) {
                strengthClass = 'strong';
                strengthMessage = 'Strong';
            } else {
                strengthClass = 'very-strong';
                strengthMessage = 'Very Strong';
            }

            // Remove any existing classes
            strengthMeter.className = 'password-strength-meter-fill';

            // Add new class
            strengthMeter.classList.add(strengthClass);

            // Update text
            strengthText.textContent = strengthMessage;
        }
    });
    </script>
</@layout.registrationLayout>
