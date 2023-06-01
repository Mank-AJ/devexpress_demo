// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var sidbare = true;
function openNav() {
    if (sidbare == false) {
        document.getElementById("layoutSidenav_nav").style.width = "250px";
        document.getElementById("layoutSidenav_content").style.marginLeft = "2%";
        sidbare = true;
    }
    else {
        document.getElementById("layoutSidenav_nav").style.width = "0%";
        document.getElementById("layoutSidenav_content").style.marginLeft = "-17%";
        sidbare = false;
    }
}


function openPopup(rowcount) {

    var url = '/Invoice/Details?rowcount=' + encodeURIComponent(rowcount);
    var newWindow = window.open(url, '_blank');
    newWindow.focus();
}
function generatePDF() {
    // Create the loader element
    var loader = document.createElement('div');
    loader.classList.add('loader');
    document.body.appendChild(loader);

    // Create the filename
    var today = new Date();
    var date = today.toISOString().slice(0, 10);
    var time = today.toTimeString().slice(0, 8).replace(/:/g, '');
    var filename = 'Invoice_' + date + '_' + time + '.pdf';

    // Options for the PDF style
    var options = {
        margin: 10,
        filename: filename,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'pt', format: 'letter', orientation: 'portrait' }
    };

    // Select the HTML element that you want to convert to PDF
    var element = document.documentElement;

    // Exclude the header and footer elements from PDF conversion
    var headerElements = document.getElementsByClassName('fixed-header');
    var footerElements = document.getElementsByClassName('fixed-footer');

    // Hide the PDF generation button during conversion
    var pdfButton = document.getElementById('pdfgenerate');
    pdfButton.style.display = 'none';

    for (var i = 0; i < headerElements.length; i++) {
        headerElements[i].style.display = 'none';
    }

    for (var j = 0; j < footerElements.length; j++) {
        footerElements[j].style.display = 'none';
    }

    // Call the html2pdf library to generate the PDF asynchronously
    html2pdf()
        .set(options)
        .from(element)
        .save()
        .then(function () {
            // Remove the loader element
            document.body.removeChild(loader);

            // Show the PDF generation button and restore header and footer display after conversion
            pdfButton.style.display = 'block';

            for (var i = 0; i < headerElements.length; i++) {
                headerElements[i].style.display = 'block';
            }

            for (var j = 0; j < footerElements.length; j++) {
                footerElements[j].style.display = 'block';
            }
        });
}


function initializeDataTable() {
    $('#InvoiceTable').DataTable({
        pagingType: 'full_numbers',
        lengthChange: false,
    });
}

document.addEventListener('DOMContentLoaded', function () {
    var trigger = document.querySelector('.hamburger');
    var overlay = document.querySelector('.overlay');
    var sidebar = document.querySelector('#sidebar-wrapper');
    var isClosed = false;

    trigger.addEventListener('click', function () {
        hamburgerCross();
    });

    function hamburgerCross() {
        if (isClosed) {
            overlay.style.display = 'none';
            sidebar.style.display='none'
            trigger.classList.remove('is-open');
            trigger.classList.add('is-closed');
            isClosed = false;
        } else {
            //overlay.style.display = 'block';
            sidebar.style.display = "block";
            trigger.classList.remove('is-closed');
            trigger.classList.add('is-open');
            isClosed = true;
        }
    }

    var offCanvasToggle = document.querySelectorAll('[data-toggle="offcanvas"]');
    offCanvasToggle.forEach(function (element) {
        element.addEventListener('click', function () {
            document.getElementById('wrapper').classList.toggle('toggled');
            document.getElementById('page-content-wrapper').classList.toggle('offcanvas-open');
        });
    });
});

window.addEventListener('scroll', function () {
    var scrollPosition = window.scrollY || document.documentElement.scrollTop || document.body.scrollTop;
    var windowHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    var documentHeight = Math.max(
        document.body.scrollHeight, document.documentElement.scrollHeight,
        document.body.offsetHeight, document.documentElement.offsetHeight,
        document.body.clientHeight, document.documentElement.clientHeight
    );

    if (scrollPosition + windowHeight >= documentHeight-5) {
        document.querySelector('.site-footer').style.display = 'block';
    } else {
        document.querySelector('.site-footer').style.display = 'none';
    }
});
$(document).ready(function () {
    initializeDataTable();
});