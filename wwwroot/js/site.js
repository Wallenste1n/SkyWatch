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
        .then(r => r.json())
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
})