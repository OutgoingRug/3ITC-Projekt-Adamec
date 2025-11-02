// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });
});



const input = document.querySelector(".input");

input.addEventListener("keydown", (e) => {
    if (e.key === "Enter") {
        input.classList.add("flash");
        setTimeout(() => {
            input.classList.remove("flash");
        }, 200); //200 milisekund
    }
});