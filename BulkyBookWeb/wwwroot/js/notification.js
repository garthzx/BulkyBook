const successDiv = document.querySelector(".success-container");
successDiv.classList.toggle("active");

const sub = setTimeout(() => {
    successDiv.classList.toggle("active");
}, 2500);