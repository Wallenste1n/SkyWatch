// Buttons for daily forecast
const dailyScroll = document.getElementById("dailyScroll");

document.getElementById("dailyUp").onclick = () => {
    dailyScroll.scrollBy({top: -210, behavior: "smooth"});
};

document.getElementById("dailyDown").onclick = () => {
    dailyScroll.scrollBy({top: 210, behavior: "smooth"});  
};

// When clicked on the day hourly card is displayed of clicked day in daily forecast
document.querySelectorAll(".daily-card").forEach(card => {
    card.addEventListener("click", () => {
        const date = card.dataset.date;
        
        const target = document.querySelector(
            `.hourly-card[data-date="${date}"]`
        );
        
        if(target){
            target.scrollIntoView({
                behavior: "smooth",
                inline: "center",
                block: "nearest"
            });
        }
    });
});

const hourly = document.getElementById("hourlyScroll");

hourly.addEventListener("scroll", () => {
    const cards = [...document.querySelector(".hourly-card")];
    const center = hourly.scrollLeft + hourly.clientWidth / 2;
    
    const current = cards.find(card => {
        const rect = card.getBoundingClientRect();
        return rect.left <= center && rect.right >= center;
    });
    
    if(!current) return;
    
    const date = current.dataset.date;
    
    document.querySelectorAll(".daily-card").forEach(d =>
        d.classList.toggle("active", d.dataset.date === date)
    );
});

document.querySelector(".daily-card.today")?.click();

/* ====== For swipe on phones ====== */
(() => {
    const container = document.getElementById('dailyScroll');
    
    if(!container) return;
    
    let startY = 0;
    let currentY = 0;
    let isTouching = false;
    
    const SWIPE_THRESHOLD = 40;
    const SCROLL_STEP = 120;
    
    container.addEventListener("touchstart", e => {
        startY = e.touches[0].clientY;
        isTouching = true;
    }, {passive: true});
    
    container.addEventListener("touchmove", e => {
        if(!isTouching) return;
        currentY = e.touches[0].clientY;
    }, {passive: true});
    
    container.addEventListener("touchend", () => {
        if(!isTouching) return;
        
        const deltaY = startY - currentY;
        
        if(Math.abs(deltaY) > SWIPE_THRESHOLD) {
            container.scrollBy({
                top: deltaY > 0 ? SCROLL_STEP : -SCROLL_STEP,
                behavior: "smooth"
            });
        }
        
        isTouching = false;
    });
})();