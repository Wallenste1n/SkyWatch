// To deal with buttons on the PC
const container = document.getElementById("hourlyScroll");
const btnLeft = document.getElementById("scrollLeft");
const btnRight = document.getElementById("scrollRight");

const cardWidth = 176; // card + gap

btnRight.onclick = () => scrollToCard(1);
btnLeft.onclick = () => scrollToCard(-1);

//scrolls to next card on chosen direction
function scrollToCard(direction) {
    const index = Math.round(container.scrollLeft / cardWidth);
    const target = (index + direction) * cardWidth;

    container.scrollTo({
        left: target,
        behavior: "smooth"
    });
}

// Hides buttons, when there are no available items
function updateButtons() {
    btnLeft.classList.toggle("hidden", container.scrollLeft <= 0);
    btnRight.classList.toggle(
        "hidden", 
        container.scrollLeft + container.clientWidth >= container.scrollWidth - 5
    );
}

container.addEventListener("scroll", updateButtons);
window.addEventListener("load", updateButtons);

/* ====== For swipe on phones ====== */
let startX = 0;
let currentX = 0;
let isDragging = false;

let snapTimeout;

container.addEventListener("scroll", () => {
    clearTimeout(snapTimeout);

    snapTimeout = setTimeout(() => {
        const index = Math.round(container.scrollLeft  / cardWidth);
        
        container.scrollTo({
            left: index * cardWidth,
            behavior: "smooth"
        })
    }, 80);
});

container.addEventListener("touchstart", e =>{
    startX = e.touches[0].clientX;
    isDragging = true;
});

container.addEventListener("touchmove", e => {
    if(!isDragging) return;
    currentX = e.touches[0].clientX;
});

container.addEventListener("touchend", () =>{
    if(!isDragging) return;
    
    const delta = startX - currentX;
    
    if(Math.abs(delta) > 50){
        container.scrollLeft += delta;
    }
    isDragging = false;
});

//for 12/24 hours modes
document.addEventListener("DOMContentLoaded", () => {
    const buttons = document.querySelectorAll(".time-btn");
    const timeElements = document.querySelectorAll("[data-unix]")

    // locale auto-detect
    const locale = navigator.language || "en-US";
    let timeFormat = locale.startsWith("en-US") ? "12" : "24";

    // cookie override
    const cookieMatch = document.cookie.match(/time_format=(12|24)/);
    if(cookieMatch) timeFormat = cookieMatch[1];

    // apply initial state
    setActiveButton(timeFormat);
    renderTimes(timeFormat);

    // button click
    buttons.forEach(btn => {
        btn.addEventListener("click", () => {
            timeFormat = btn.dataset.format;

            document.cookie = `time_format=${timeFormat}; path=/; max-age=2592000`;

            setActiveButton(timeFormat);
            renderTimes(timeFormat);
        });
    });

    // Sets active button
    function setActiveButton (format) {
        buttons.forEach(b =>
            b.classList.toggle("active", b.dataset.format === format)
        );
    }

    // Renders unix time to the date 
    function renderTimes (format) {
        timeElements.forEach(el => {
            const unix = parseInt(el.dataset.unix);
            if(!unix) return;

            const date = new Date(unix * 1000);

            el.textContent = formatTime(date, format);
        });
    }

    // Sets time Format 
    function formatTime(date, format) {
        return new Intl.DateTimeFormat(
            locale,
            {
                hour: "numeric",
                minute: "2-digit",
                hour12: format === "12"
            }
        ).format(date);
    }
});