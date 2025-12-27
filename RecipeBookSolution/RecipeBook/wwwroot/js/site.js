// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });
});



const input = document.querySelector(".vyhledavaniReceptu");

input.addEventListener("keydown", (e) => {
    if (e.key === "Enter") {
        input.classList.add("flash");
        setTimeout(() => {
            input.classList.remove("flash");
        }, 200); //v milisekundach
    }
});



//const button = document.querySelector(".button-confirm");

//button.addEventListener("click", () => {
//    button.classList.add("flash");

//    setTimeout(() => {
//        button.classList.remove("flash");
//    }, 200);
//});



const button = document.querySelector(".button-confirm");

button.addEventListener("click", (e) => {
    e.preventDefault(); // zastavim aby se to hned submitnulo

    button.classList.add("flash");

    setTimeout(() => {
        button.classList.remove("flash");
        button.closest("form").submit(); // submitnu formular az po animaci
    }, 200);
});