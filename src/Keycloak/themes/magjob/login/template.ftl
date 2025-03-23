<#macro registrationLayout bodyClass="" displayInfo=false displayMessage=true displayRequiredFields=false displayWide=false showAnotherWayIfPresent=true>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"  "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" class="${properties.kcHtmlClass!}">

<head>
    <meta charset="utf-8">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="robots" content="noindex, nofollow">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />

    <title>${msg("loginTitle",(realm.displayName!''))}</title>
    <#if properties.meta?has_content>
        <#list properties.meta?split(' ') as meta>
            <meta name="${meta?split('==')[0]}" content="${meta?split('==')[1]}"/>
        </#list>
    </#if>
    <link rel="icon" href="${url.resourcesPath}/img/favicon.ico" />
    <#if properties.stylesCommon?has_content>
        <#list properties.stylesCommon?split(' ') as style>
            <link href="${url.resourcesCommonPath}/${style}" rel="stylesheet" />
        </#list>
    </#if>
    <#if properties.styles?has_content>
        <#list properties.styles?split(' ') as style>
            <link href="${url.resourcesPath}/${style}" rel="stylesheet" />
        </#list>
    </#if>
    <#if scripts??>
        <#list scripts as script>
            <script src="${script}" type="text/javascript"></script>
        </#list>
    </#if>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">
</head>

<body class="${properties.kcBodyClass!}">
    <button type="button" class="theme-toggle" aria-label="Toggle dark mode" title="Toggle dark mode">
        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="theme-toggle-icon">
            <circle cx="12" cy="12" r="5"></circle>
            <line x1="12" y1="1" x2="12" y2="3"></line>
            <line x1="12" y1="21" x2="12" y2="23"></line>
            <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line>
            <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
            <line x1="1" y1="12" x2="3" y2="12"></line>
            <line x1="21" y1="12" x2="23" y2="12"></line>
            <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line>
            <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
        </svg>
    </button>

    <div class="login-pf-page">
        <div class="login-pf-container">
            <div class="login-pf-header">
                <div id="kc-header" class="${properties.kcHeaderClass!}">
                    <div class="logo-wrapper">
                        <div class="kc-logo-text"></div>
                    </div>
                </div>
            </div>

            <div class="card-pf">
                <header class="login-pf-header">
                    <#if realm.internationalizationEnabled && locale.supported?size gt 1>
                        <div class="language-selector">
                            <div class="kc-dropdown" id="kc-locale-dropdown">
                                <a href="#" id="kc-current-locale-link">${locale.current}</a>
                                <ul>
                                    <#list locale.supported as l>
                                        <li class="kc-dropdown-item"><a href="${l.url}">${l.label}</a></li>
                                    </#list>
                                </ul>
                            </div>
                        </div>
                    </#if>
                    <h1 id="kc-page-title"><#nested "header"></h1>
                </header>

                <div id="kc-content">
                    <div id="kc-content-wrapper">
                        <#-- App-initiated actions should not see warning messages about the need to complete the action -->
                        <#-- during login.                                                                               -->
                        <#if displayMessage && message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
                            <div class="alert alert-${message.type}">
                                <span class="message-text">${kcSanitize(message.summary)?no_esc}</span>
                            </div>
                        </#if>

                        <#nested "form">

                        <#if displayInfo>
                            <div id="kc-info" class="${properties.kcInfoAreaClass!}">
                                <div id="kc-info-wrapper" class="${properties.kcInfoAreaWrapperClass!}">
                                    <#nested "info">
                                </div>
                            </div>
                        </#if>
                    </div>
                </div>
            </div>

            <div class="footer">
                <p>&copy; ${.now?string('yyyy')} MagJob 2.0. All rights reserved.</p>
            </div>
        </div>
    </div>

    <script type="text/javascript">
    // Funkcja inicjująca tryb ciemny
    function initDarkMode() {
        const themeToggleBtn = document.querySelector('.theme-toggle');
        const themeToggleIcon = document.querySelector('.theme-toggle-icon');

        // Sprawdź zapisane preferencje
        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'dark') {
            document.body.classList.add('dark-mode');
            updateThemeIcon(true);
        } else if (savedTheme === 'light') {
            document.body.classList.remove('dark-mode');
            updateThemeIcon(false);
        } else {
            // Tryb automatyczny (preferencje systemu)
            document.body.classList.add('auto-mode');
            const prefersDarkMode = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
            if (prefersDarkMode) {
                document.body.classList.add('dark-mode');
                updateThemeIcon(true);
            } else {
                updateThemeIcon(false);
            }
        }

        // Dodaj nasłuchiwanie na kliknięcie przycisku
        themeToggleBtn.addEventListener('click', toggleTheme);

        // Nasłuchiwanie na zmianę preferencji systemowych
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
            if (document.body.classList.contains('auto-mode')) {
                if (e.matches) {
                    document.body.classList.add('dark-mode');
                    updateThemeIcon(true);
                } else {
                    document.body.classList.remove('dark-mode');
                    updateThemeIcon(false);
                }
            }
        });
    }

    // Funkcja przełączająca tryb ciemny
    function toggleTheme() {
        const isDarkMode = document.body.classList.contains('dark-mode');

        document.body.classList.remove('auto-mode');

        if (isDarkMode) {
            document.body.classList.remove('dark-mode');
            localStorage.setItem('theme', 'light');
            updateThemeIcon(false);
        } else {
            document.body.classList.add('dark-mode');
            localStorage.setItem('theme', 'dark');
            updateThemeIcon(true);
        }
    }

    // Aktualizacja ikony zależnie od trybu
    function updateThemeIcon(isDarkMode) {
        const themeToggleIcon = document.querySelector('.theme-toggle-icon');

        if (isDarkMode) {
            // Ikona księżyca dla trybu ciemnego
            themeToggleIcon.innerHTML = `
                <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"></path>
            `;
        } else {
            // Ikona słońca dla trybu jasnego
            themeToggleIcon.innerHTML = `
                <circle cx="12" cy="12" r="5"></circle>
                <line x1="12" y1="1" x2="12" y2="3"></line>
                <line x1="12" y1="21" x2="12" y2="23"></line>
                <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line>
                <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
                <line x1="1" y1="12" x2="3" y2="12"></line>
                <line x1="21" y1="12" x2="23" y2="12"></line>
                <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line>
                <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
            `;
        }
    }

    // Inicjalizacja po załadowaniu strony
    document.addEventListener('DOMContentLoaded', initDarkMode);
    </script>

    <#if properties.scripts?has_content>
        <#list properties.scripts?split(' ') as script>
            <script src="${url.resourcesPath}/${script}" type="text/javascript"></script>
        </#list>
    </#if>
    <#if scripts??>
        <#list scripts as script>
            <script src="${script}" type="text/javascript"></script>
        </#list>
    </#if>
</body>
</html>
</#macro>