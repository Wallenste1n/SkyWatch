(function () {
    try {
        const savedTheme = localStorage.getItem("theme");

        if (savedTheme === "dark") {
            document.documentElement.setAttribute("data-theme", "dark");
        }
        else if (savedTheme === "light") {
            document.documentElement.setAttribute("data-theme", "light");
        }
        else {
            // Auto-detect from OS
            const prefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
            document.documentElement.setAttribute(
                "data-theme",
                prefersDark ? "dark" : "light"
            );
        }
    } catch (e) {}
})();