const Theme = (() => {
    const root = document.documentElement;
    const buttons = document.querySelectorAll(".theme-btn");
   
    //getting system theme
    const getSystemTheme = () => 
        window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    
    //applying theme
    const applyTheme = (theme) => {
        const resolvedTheme = theme === 'auto' ? getSystemTheme() : theme;
        root.setAttribute('data-theme', resolvedTheme);
        
        buttons.forEach(b => {
            b.classList.toggle("active", b.dataset.theme === theme)
        });
        
        localStorage.setItem('theme', theme);
    };
    
    const init = () => {
        const saved = localStorage.getItem('theme') || 'auto';
        applyTheme(saved);
        
        buttons.forEach(btn => 
            btn.addEventListener('click', () => 
                applyTheme(btn.dataset.theme)
            )
        );
        
        //Live update if system theme changes
        window.matchMedia('(prefers-color-scheme: dark)')
            .addEventListener('change', () => {
                if(localStorage.getItem('theme') === 'auto') {
                    applyTheme('auto')
                }
            })
    }
    
    return { init };
})();

document.addEventListener('DOMContentLoaded', Theme.init);