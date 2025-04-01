<#import "template.ftl" as layout>
<@layout.registrationLayout displayInfo=social.displayInfo displayWide=(realm.password && social.providers??) templateType="login"; section>
    <#if section = "header">
        ${msg("doLogIn")}
    <#elseif section = "form">
        <div id="kc-form">
            <div id="kc-form-wrapper">
                <#if realm.password>
                    <form id="kc-form-login" onsubmit="login.disabled = true; return true;" action="${url.loginAction}" method="post">
                        <div class="form-group">
                            <label for="username" class="control-label">${msg("username")}</label>
                            <input tabindex="1" id="username" class="form-control" name="username" value="${(login.username!'')}" type="text" autofocus autocomplete="username" />
                        </div>

                        <div class="form-group">
                            <label for="password" class="control-label">${msg("password")}</label>
                            <div class="password-input-wrapper">
                                <input tabindex="2" id="password" class="form-control" name="password" type="password" autocomplete="current-password" />
                                <div class="password-toggle-icon" onclick="togglePassword('password', 'togglePasswordIcon')">
                                    <svg id="togglePasswordIcon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="eye-icon">
                                        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                                        <circle cx="12" cy="12" r="3"></circle>
                                    </svg>
                                </div>
                            </div>
                        </div>

                        <div class="form-group login-options">
                            <#if realm.rememberMe && !usernameEditDisabled??>
                                <div class="checkbox">
                                    <label>
                                        <#if login.rememberMe??>
                                            <input tabindex="3" id="rememberMe" name="rememberMe" type="checkbox" checked>
                                        <#else>
                                            <input tabindex="3" id="rememberMe" name="rememberMe" type="checkbox">
                                        </#if>
                                        <span>${msg("rememberMe")}</span>
                                    </label>
                                </div>
                            </#if>
                            <div class="login-forgot-password">
                                <#if realm.resetPasswordAllowed>
                                    <a tabindex="5" href="${url.loginResetCredentialsUrl}">${msg("doForgotPassword")}</a>
                                </#if>
                            </div>
                        </div>

                        <div id="kc-form-buttons" class="form-group">
                            <input type="hidden" id="id-hidden-input" name="credentialId" <#if auth.selectedCredential?has_content>value="${auth.selectedCredential}"</#if>/>
                            <input tabindex="4" class="btn btn-primary btn-md" name="login" id="kc-login" type="submit" value="${msg("doLogIn")}"/>
                        </div>

                        <#if realm.password && realm.registrationAllowed && !registrationDisabled??>
                            <div class="registration-link">
                                <span>New user? <a tabindex="6" href="${url.registrationUrl}">${msg("doRegister")}</a></span>
                            </div>
                        </#if>
                    </form>
                </#if>
            </div>
        </div>

        <#if realm.password && social.providers??>
            <div id="kc-social-providers" class="kc-social-providers">
                <div class="social-divider">
                    <span>Or sign in with</span>
                </div>

                <ul class="kc-social-links">
                    <#list social.providers as p>
                        <li class="kc-social-link">
                            <a id="social-${p.alias}" class="zocial ${p.providerId} btn-outline" href="${p.loginUrl}">
                                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                                    <path d="M12.545,10.239v3.821h5.445c-0.712,2.315-2.647,3.972-5.445,3.972c-3.332,0-6.033-2.701-6.033-6.032s2.701-6.032,6.033-6.032c1.498,0,2.866,0.549,3.921,1.453l2.814-2.814C17.503,2.988,15.139,2,12.545,2C7.021,2,2.543,6.477,2.543,12s4.478,10,10.002,10c8.396,0,10.249-7.85,9.426-11.748L12.545,10.239z"/>
                                </svg>
                                <span>${p.displayName}</span>
                            </a>
                        </li>
                    </#list>
                </ul>
            </div>
        </#if>
    <#elseif section = "info" >
        <#-- This section intentionally left empty, as we've moved registration link to the form section -->
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
