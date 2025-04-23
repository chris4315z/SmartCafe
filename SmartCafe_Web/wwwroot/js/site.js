// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    document.body.classList.remove("fade-in");

    document.querySelectorAll("a").forEach(function (link) {
        if (link.hostname === window.location.hostname && link.getAttribute("href") && !link.getAttribute("href").startsWith("#")) {
            link.addEventListener("click", function (e) {
                e.preventDefault();
                document.body.classList.add("fade-out");
                setTimeout(() => {
                    window.location = link.href;
                }, 600); // match CSS transition time
            });
        }
    });
});

