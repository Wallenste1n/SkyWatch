// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", () =>{
    //Gets units and form from document to submit it after pressing each select or submit 
    const units = document.getElementById("unitsSelect");
    const form = document.getElementById("weatherForm");
    if(units && form){
        units.addEventListener("change", () => form.submit());
    }
});

document.addEventListener("DOMContentLoaded", () => {
    //If city is in cookie - doing nothing
    if(document.cookie.includes("weather_city")) return;
    
    //Looking for location through user's IP
    fetch("https://ipapi.co/json/")
        .then(response => response.json())
        .then(data => {
            if(!data.city) return;
            
            //writes cookie
            document.cookie = `weather_city=${data.city}; path=/; max-age=2592000`;
            
            //sends form immediately
            const form = document.getElementById("weatherForm");
            if(form){
                form.querySelector("input[name='cityName']").value = data.city;
                form.submit();
            }
        })
});

//Changes states for loader of the site (sets or remove "hidden" tag)
const loader = document.getElementById("loader");

document.addEventListener("submit", () => {
    loader.classList.remove("hidden");
});

window.addEventListener("load", () => {
    loader.classList.add("hidden");
});

//can be used later
function showLoader(){
    document.getElementById("loader").classList.remove("hidden");
}

//when submits hides weather data and shows skeleton (idk if it's working, probably not)
document.addEventListener("submit", () =>{
    weather.classList().add("hidden");
    skeleton.classList().remove("hidden");
});

//getting elements 
const cityInput = document.getElementById("cityInput");
const suggestions = document.getElementById("suggestions");

//timer to lower API requests
let debounceTimer;

//variables for selection items
let selectedIndex = -1;
let currentItems = [];

//shows or hide list of cities when user starts to input in search bar 
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

//prevents of "Enter" key to do something, if it's not chosen in the list of city's
cityInput.addEventListener("keydown", e =>{
    if(e.key === "Enter"){
        if(selectedIndex === -1){
            e.preventDefault();
        }
    }
});

//when click by the mouse and choose item (city) submits form 
suggestions.addEventListener("click", e => {
    const item = e.target.closest(".suggestion-item");
    if(!item) return;
    
    selectCity(item);
});

//controls in city list (to go up or down)
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

//Updates selection position when choosing city in the list by keyboard
function updateSelection(){
    currentItems.forEach(item => item.classList.remove("active"));
    
    if(selectedIndex >= 0 && currentItems[selectedIndex]){
        const selected = currentItems[selectedIndex];
        selected.classList.add("active");
        selected.scrollIntoView({block: "nearest"});
    }
}

//function for submit form when city is chosen in the list
function selectCity(item){
    cityInput.value = item.dataset.name;
    
    suggestions.innerHTML = "";
    suggestions.classList.remove("show");
    
    document.getElementById("weatherForm").submit();
}

//closes list if user clicks somewhere (but not in the list)
document.addEventListener("click", e => {
    const autocomplete = document.querySelector(".city-search");
    
    if(!autocomplete.contains(e.target)){
        closeSuggestions();
    }
});

//if clicking on search bar, and there are some text - opens list 
cityInput.addEventListener("focus", () =>{
    if(cityInput.value.trim() !== "" && currentItems.length){
        openSuggestions();
    }
});

//if something typing in search bar - opens list
cityInput.addEventListener("input", () =>{
   selectedIndex = -1;
   openSuggestions();
});

//function for opening list of suggestions
function openSuggestions(){
    suggestions.style.display = "block"
}

//function for closing list of suggestions
function closeSuggestions(){
    suggestions.style.display = "none";
    selectedIndex = -1;
}