function initializeCookieConsent(message, linkText, url) {
    window.cookieconsent.initialise({
        cookie: {
            name: '@cookieString'
        },
        palette: {
            popup: {
                background: "#3c404d",
                text: "#d6d6d6"
            },
            button: {
                background: "#8bed4f"
            }
        },
        theme: "classic",
        position: "bottom-left",
        content: {
            message: message,
            link: linkText,
            href: url
        }
    });
}