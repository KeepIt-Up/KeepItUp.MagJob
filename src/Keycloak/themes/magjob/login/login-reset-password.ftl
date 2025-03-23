<#import "template.ftl" as layout>
<@layout.registrationLayout displayInfo=true displayMessage=!messagesPerField.existsError('username') templateType="reset" bodyClass="reset-password-page"; section>
    <#if section = "header">
        ${msg("emailForgotTitle")}
    <#elseif section = "form">
        <form id="kc-reset-password-form" class="${properties.kcFormClass!}" action="${url.loginAction}" method="post">
            <div class="form-group">
                <label for="username" class="control-label">${msg("email")}</label>
                <input type="text" id="username" name="username" class="form-control" autofocus
                       autocomplete="email"
                       aria-invalid="<#if messagesPerField.existsError('username')>true</#if>"
                />
                <#if messagesPerField.existsError('username')>
                    <span id="input-error-username" class="error-message" aria-live="polite">
                        ${kcSanitize(messagesPerField.get('username'))?no_esc}
                    </span>
                </#if>
            </div>

            <div class="form-group">
                <a class="back-to-login" href="${url.loginUrl}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                        <path d="M19 12H5"></path>
                        <path d="M12 19l-7-7 7-7"></path>
                    </svg>
                    Back to Login
                </a>
            </div>

            <div id="kc-form-buttons" class="form-group">
                <input class="btn btn-primary btn-md" type="submit" value="${msg("doSubmit")}"/>
            </div>
        </form>
        <div class="reset-instructions">
            ${msg("emailInstruction")}
        </div>
    </#if>
</@layout.registrationLayout>
