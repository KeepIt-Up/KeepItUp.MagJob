<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('firstName','lastName','email','username','password','password-confirm'); section>
    <#if section = "header">
        ${msg("registerTitle")}
    <#elseif section = "form">
        <form id="kc-register-form" class="register-form" action="${url.registrationAction}" method="post">

            <div class="form-group">
                <label for="firstName" class="control-label">${msg("firstName")}</label>
                <input type="text" id="firstName" class="form-control" name="firstName" value="${(register.formData.firstName!'')}"
                       aria-invalid="<#if messagesPerField.existsError('firstName')>true</#if>"
                />

                <#if messagesPerField.existsError('firstName')>
                    <span id="input-error-firstname" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('firstName'))?no_esc}
                    </span>
                </#if>
            </div>

            <div class="form-group">
                <label for="lastName" class="control-label">${msg("lastName")}</label>
                <input type="text" id="lastName" class="form-control" name="lastName" value="${(register.formData.lastName!'')}"
                       aria-invalid="<#if messagesPerField.existsError('lastName')>true</#if>"
                />

                <#if messagesPerField.existsError('lastName')>
                    <span id="input-error-lastname" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('lastName'))?no_esc}
                    </span>
                </#if>
            </div>

            <div class="form-group">
                <label for="email" class="control-label">${msg("email")}</label>
                <input type="email" id="email" class="form-control" name="email" value="${(register.formData.email!'')}" autocomplete="email"
                       aria-invalid="<#if messagesPerField.existsError('email')>true</#if>"
                />

                <#if messagesPerField.existsError('email')>
                    <span id="input-error-email" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('email'))?no_esc}
                    </span>
                </#if>
            </div>

            <#if !realm.registrationEmailAsUsername>
                <div class="form-group">
                    <label for="username" class="control-label">${msg("username")}</label>
                    <input type="text" id="username" class="form-control" name="username" value="${(register.formData.username!'')}" autocomplete="username"
                           aria-invalid="<#if messagesPerField.existsError('username')>true</#if>"
                    />

                    <#if messagesPerField.existsError('username')>
                        <span id="input-error-username" class="error-message" aria-live="polite">
                            ${kcSanitize(messagesPerField.get('username'))?no_esc}
                        </span>
                    </#if>
                </div>
            </#if>

            <div class="form-group">
                <label for="password" class="control-label">${msg("password")}</label>
                <div class="password-input-wrapper">
                    <input type="password" id="password" class="form-control" name="password" autocomplete="new-password"
                           aria-invalid="<#if messagesPerField.existsError('password','password-confirm')>true</#if>"
                    />
                    <div class="password-toggle-icon" onclick="togglePassword('password', 'togglePasswordIcon')">
                        <svg id="togglePasswordIcon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="eye-icon">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                            <circle cx="12" cy="12" r="3"></circle>
                        </svg>
                    </div>
                </div>

                <#if messagesPerField.existsError('password')>
                    <span id="input-error-password" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('password'))?no_esc}
                    </span>
                </#if>
            </div>

            <div class="form-group">
                <label for="password-confirm" class="control-label">${msg("passwordConfirm")}</label>
                <div class="password-input-wrapper">
                    <input type="password" id="password-confirm" class="form-control" name="password-confirm" autocomplete="new-password"
                           aria-invalid="<#if messagesPerField.existsError('password-confirm')>true</#if>"
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
    </script>
</@layout.registrationLayout>
