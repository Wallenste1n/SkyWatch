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