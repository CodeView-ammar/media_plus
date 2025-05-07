// Main JavaScript file for MediaPlus application

$(document).ready(function() {
    // Handle sidebar toggle for mobile devices
    $('.navbar-toggler').on('click', function() {
        $('.sidebar').toggleClass('show');
    });

    // Close sidebar when clicking outside on mobile
    $(document).on('click', function(e) {
        if ($(window).width() < 992) {
            if (!$(e.target).closest('.sidebar').length && 
                !$(e.target).closest('.navbar-toggler').length) {
                $('.sidebar').removeClass('show');
            }
        }
    });

    // Active link highlighting based on current URL
    const currentPage = window.location.pathname;
    $('.nav-link').each(function() {
        const linkHref = $(this).attr('href');
        if (linkHref && currentPage.includes(linkHref) && linkHref !== '/') {
            $(this).addClass('active');
            
            // If in a submenu, expand the parent menu
            const parentCollapse = $(this).closest('.collapse');
            if (parentCollapse.length) {
                parentCollapse.addClass('show');
                $(`[data-bs-target="#${parentCollapse.attr('id')}"]`).removeClass('collapsed');
            }
        }
    });

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize any DataTables
    if ($.fn.DataTable) {
        $('.table').each(function() {
            if ($.fn.DataTable.isDataTable(this)) {
                return;
            }
            
            // Only initialize if it has the datatable class or id
            if ($(this).hasClass('dataTable') || 
                $(this).attr('id') && $(this).attr('id').includes('dataTable')) {
                $(this).DataTable({
                    responsive: true,
                    language: {
                        url: $('.html').attr('lang') === 'ar' 
                            ? '//cdn.datatables.net/plug-ins/1.13.4/i18n/ar.json'
                            : '//cdn.datatables.net/plug-ins/1.13.4/i18n/en-US.json'
                    }
                });
            }
        });
    }

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert-dismissible').alert('close');
    }, 5000);

    // Form validation styles
    $('form').on('submit', function() {
        $(this).addClass('was-validated');
    });
});