// Dashboard JavaScript functionality for MediaPlus

$(document).ready(function() {
    // Toggle submenu items on click
    $('.nav-link[data-bs-toggle="collapse"]').on('click', function() {
        const target = $(this).attr('data-bs-target');
        $(target).on('shown.bs.collapse', function() {
            $(this).parent().find('.fa-angle-down').addClass('rotate-icon');
        });
        
        $(target).on('hidden.bs.collapse', function() {
            $(this).parent().find('.fa-angle-down').removeClass('rotate-icon');
        });
    });

    // Initialize charts if available
    if (typeof Chart !== 'undefined') {
        initializeCharts();
    }

    // Handle any quick filters on dashboard
    $('.dashboard-filter-btn').on('click', function() {
        $('.dashboard-filter-btn').removeClass('active');
        $(this).addClass('active');
        
        // You can add filtering logic here based on the clicked button
        const filterValue = $(this).data('filter');
        
        // Simulate loading state
        $('.dashboard-content').addClass('loading');
        
        // Remove loading state after a short delay (replace with actual data loading)
        setTimeout(function() {
            $('.dashboard-content').removeClass('loading');
        }, 600);
    });
});

// Chart initialization functions
function initializeCharts() {
    // Example chart initialization code
    const ctx = document.getElementById('usageChart');
    if (ctx) {
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
                datasets: [{
                    label: 'Materials',
                    data: [65, 59, 80, 81, 56, 55, 40],
                    borderColor: '#2563eb',
                    backgroundColor: 'rgba(37, 99, 235, 0.1)',
                    tension: 0.4,
                    fill: true
                }, {
                    label: 'Shows',
                    data: [28, 48, 40, 19, 86, 27, 90],
                    borderColor: '#16a34a',
                    backgroundColor: 'rgba(22, 163, 74, 0.1)',
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    tooltip: {
                        mode: 'index',
                        intersect: false,
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    // Example device status chart (doughnut)
    const deviceCtx = document.getElementById('deviceStatusChart');
    if (deviceCtx) {
        new Chart(deviceCtx, {
            type: 'doughnut',
            data: {
                labels: ['Online', 'Offline', 'Maintenance'],
                datasets: [{
                    data: [70, 15, 15],
                    backgroundColor: [
                        '#16a34a', // success - online
                        '#dc2626', // danger - offline
                        '#f59e0b'  // warning - maintenance
                    ],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'bottom',
                    }
                },
                cutout: '70%'
            }
        });
    }
}