<#import "template.ftl" as layout>
<@layout.registrationLayout displayInfo=true displayMessage=!messagesPerField.existsError('username'); section>
    <#if section = "header">
        ${msg("emailForgotTitle")}
    <#elseif section = "form">
        <form id="kc-reset-password-form" class="${properties.kcFormClass!}" action="${url.loginAction}" method="post">
            <div class="form-group">
                <label for="username" class="control-label">${msg("email")}</label>
                <input type="text" id="username" name="username" class="form-control" autofocus
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
                    ← Back to Login
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
