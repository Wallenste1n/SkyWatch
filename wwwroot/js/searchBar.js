// Gets units and form from document to submit it after pressing each select or submit 
document.addEventListener("DOMContentLoaded", () =>{
    const units = document.getElementById("unitsSelect");
    const form = document.getElementById("weatherForm");
    if(units && form){
        units.addEventListener("change", () => form.submit());
    }
});

// Getting elements 
const cityInput = document.getElementById("cityInput");
const suggestions = document.getElementById("suggestions");

// Timer to lower API requests
let debounceTimer;

// Variables for selection items
let selectedIndex = -1;
let currentItems = [];

// Shows or hide list of cities when user starts to input in search bar 
cityInput.addEventListener("input", e => {
    const query = e.target.value.trim();

    clearTimeout(debounceTimer);

    if(query.length < 2){
        suggestions.innerHTML = "";
        suggestions.classList.remove("show");
        selectedIndex = -1;
        return;
    }

    debounceTimer = setTimeout(() => {
        fetch(`Home/CitySuggestions?query=${encodeURIComponent(query)}`)
            .then(response => response.text())
            .then(html => {
                suggestions.innerHTML = html;
                suggestions.classList.add("show");

                currentItems = [...suggestions.querySelectorAll(".suggestion-item")]
                selectedIndex = -1;
            });

    }, 300);

});

// Prevents of "Enter" key to do something, if it's not chosen in the list of city's
cityInput.addEventListener("keydown", e =>{
    if(e.key === "Enter"){
        if(selectedIndex === -1){
            e.preventDefault();
        }
    }
});

// When click by the mouse and choose item (city) submits form 
suggestions.addEventListener("click", e => {
    const item = e.target.closest(".suggestion-item");
    if(!item) return;

    selectCity(item);
});

// Controls in city list (to go up or down)
cityInput.addEventListener("keydown", e => {
    if(!currentItems.length) return;

    if(e.key === "ArrowDown"){
        e.preventDefault();
        selectedIndex = (selectedIndex + 1) % currentItems.length;
        updateSelection();
    }

    if(e.key === "ArrowUp"){
        e.preventDefault();
        selectedIndex = (selectedIndex - 1 + currentItems.length) % currentItems.length;
        updateSelection();
    }

    if(e.key === "Enter"){
        if(selectedIndex >= 0){
            e.preventDefault();
            selectCity(currentItems[selectedIndex]);
        } else{
            e.preventDefault(); //blocks enter if nothing chosen
        }
    }

    if(e.key === "Escape"){
        closeSuggestions();
    }
});

// Updates selection position when choosing city in the list by keyboard
function updateSelection(){
    currentItems.forEach(item => item.classList.remove("active"));

    if(selectedIndex >= 0 && currentItems[selectedIndex]){
        const selected = currentItems[selectedIndex];
        selected.classList.add("active");
        selected.scrollIntoView({block: "nearest"});
    }
}

// Function for submit form when city is chosen in the list
function selectCity(item){
    cityInput.value = item.dataset.name;

    suggestions.innerHTML = "";
    suggestions.classList.remove("show");

    document.getElementById("weatherForm").submit();
}

// Closes list if user clicks somewhere (but not in the list)
document.addEventListener("click", e => {
    const autocomplete = document.querySelector(".city-search");

    if(!autocomplete.contains(e.target)){
        closeSuggestions();
    }
});

// If clicking on search bar, and there are some text - opens list 
cityInput.addEventListener("focus", () =>{
    if(cityInput.value.trim() !== "" && currentItems.length){
        openSuggestions();
    }
});

// If something typing in search bar - opens list
cityInput.addEventListener("input", () =>{
    selectedIndex = -1;
    openSuggestions();
});

// Function for opening list of suggestions
function openSuggestions(){
    suggestions.style.display = "block"
}

// Function for closing list of suggestions
function closeSuggestions(){
    suggestions.style.display = "none";
    selectedIndex = -1;
}