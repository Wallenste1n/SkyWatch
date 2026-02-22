const unitButtons = document.querySelectorAll(".unit-btn");
const unitsInput = document.getElementById("unitsInput");
const form = document.getElementById('weatherForm');

// Shows active state of the button of the chosen temp unit
unitButtons.forEach(btn => {
    btn.addEventListener("click", () => {
        unitButtons.forEach(b => b.classList.remove("active"));
        btn.classList.add("active");
        
        unitsInput.value = btn.dataset.unit;
        
        form.submit();
    });
});