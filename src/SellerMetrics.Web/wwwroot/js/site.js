// SellerMetrics Site JavaScript

(function () {
    'use strict';

    // ==========================================================================
    // Sidebar Toggle
    // ==========================================================================

    const sidebarToggle = document.getElementById('sidebarToggle');
    const wrapper = document.getElementById('wrapper');

    if (sidebarToggle && wrapper) {
        // Check for saved sidebar state
        const sidebarState = localStorage.getItem('sidebarToggled');
        if (sidebarState === 'true') {
            wrapper.classList.add('toggled');
        }

        // Initialize tooltips function
        function initializeTooltips() {
            const tooltipTriggerList = document.querySelectorAll('[title]');
            tooltipTriggerList.forEach(function (tooltipTriggerEl) {
                // Only enable tooltips when sidebar is collapsed
                if (wrapper.classList.contains('toggled') && tooltipTriggerEl.closest('.sidebar')) {
                    new bootstrap.Tooltip(tooltipTriggerEl, {
                        placement: 'right',
                        trigger: 'hover'
                    });
                }
            });
        }

        // Initialize tooltips on page load
        initializeTooltips();

        sidebarToggle.addEventListener('click', function (e) {
            e.preventDefault();
            wrapper.classList.toggle('toggled');
            // Save state to localStorage
            localStorage.setItem('sidebarToggled', wrapper.classList.contains('toggled'));

            // Dispose all tooltips
            const tooltips = document.querySelectorAll('.tooltip');
            tooltips.forEach(function (tooltip) {
                tooltip.remove();
            });

            // Reinitialize tooltips after toggle
            setTimeout(initializeTooltips, 100);
        });

        // Close sidebar on mobile when clicking outside
        document.addEventListener('click', function (e) {
            if (window.innerWidth <= 768) {
                const sidebar = document.getElementById('sidebar-wrapper');
                if (wrapper.classList.contains('toggled') &&
                    !sidebar.contains(e.target) &&
                    !sidebarToggle.contains(e.target)) {
                    wrapper.classList.remove('toggled');
                    localStorage.setItem('sidebarToggled', 'false');
                }
            }
        });
    }

    // ==========================================================================
    // Delete Confirmation Modal
    // ==========================================================================

    // Generic delete confirmation handler
    document.querySelectorAll('[data-bs-toggle="modal"][data-delete-url]').forEach(function (button) {
        button.addEventListener('click', function () {
            const deleteUrl = this.getAttribute('data-delete-url');
            const itemName = this.getAttribute('data-item-name') || 'this item';
            const modal = document.getElementById('deleteConfirmModal');

            if (modal) {
                const form = modal.querySelector('form');
                const itemNameSpan = modal.querySelector('.delete-item-name');

                if (form) {
                    form.action = deleteUrl;
                }
                if (itemNameSpan) {
                    itemNameSpan.textContent = itemName;
                }
            }
        });
    });

    // ==========================================================================
    // Form Validation Enhancement
    // ==========================================================================

    // Add Bootstrap validation styling
    const forms = document.querySelectorAll('.needs-validation');
    forms.forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });

    // ==========================================================================
    // Money Input Formatting
    // ==========================================================================

    document.querySelectorAll('.money-input').forEach(function (input) {
        input.addEventListener('blur', function () {
            const value = parseFloat(this.value);
            if (!isNaN(value)) {
                this.value = value.toFixed(2);
            }
        });
    });

    // ==========================================================================
    // Auto-dismiss Alerts
    // ==========================================================================

    document.querySelectorAll('.alert-dismissible').forEach(function (alert) {
        // Auto-dismiss success alerts after 5 seconds
        if (alert.classList.contains('alert-success')) {
            setTimeout(function () {
                const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
                bsAlert.close();
            }, 5000);
        }
    });

    // ==========================================================================
    // Table Row Click Navigation
    // ==========================================================================

    document.querySelectorAll('table[data-clickable-rows] tbody tr').forEach(function (row) {
        row.addEventListener('click', function (e) {
            // Don't navigate if clicking on a button, link, or checkbox
            if (e.target.closest('a, button, input, .btn')) {
                return;
            }

            const href = this.getAttribute('data-href');
            if (href) {
                window.location.href = href;
            }
        });
    });

    // ==========================================================================
    // Filter Form Auto-submit
    // ==========================================================================

    document.querySelectorAll('.filter-auto-submit select').forEach(function (select) {
        select.addEventListener('change', function () {
            this.closest('form').submit();
        });
    });

    // ==========================================================================
    // Storage Location Tree Toggle
    // ==========================================================================

    document.querySelectorAll('.location-toggle').forEach(function (toggle) {
        toggle.addEventListener('click', function () {
            const listItem = this.closest('li');
            const childList = listItem.querySelector('ul');

            if (childList) {
                childList.classList.toggle('d-none');
                const icon = this.querySelector('i');
                if (icon) {
                    icon.classList.toggle('bi-chevron-right');
                    icon.classList.toggle('bi-chevron-down');
                }
            }
        });
    });

    // ==========================================================================
    // Quantity Adjustment Buttons
    // ==========================================================================

    document.querySelectorAll('.qty-adjust').forEach(function (button) {
        button.addEventListener('click', function () {
            const input = this.closest('.input-group').querySelector('input[type="number"]');
            const adjustment = parseInt(this.getAttribute('data-adjust'));

            if (input && !isNaN(adjustment)) {
                const currentValue = parseInt(input.value) || 0;
                const newValue = currentValue + adjustment;
                const min = parseInt(input.getAttribute('min')) || 0;
                const max = parseInt(input.getAttribute('max')) || Infinity;

                input.value = Math.max(min, Math.min(max, newValue));
                input.dispatchEvent(new Event('change'));
            }
        });
    });

})();

// ==========================================================================
// Chart.js Helper Functions
// ==========================================================================

const ChartHelper = {
    // Default chart colors
    colors: {
        primary: '#0d6efd',
        success: '#198754',
        danger: '#dc3545',
        warning: '#ffc107',
        info: '#0dcaf0',
        secondary: '#6c757d',
        ebay: '#0064d2',
        services: '#28a745'
    },

    // Format currency for chart labels
    formatCurrency: function (value) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).format(value);
    },

    // Create a revenue trend line chart
    createRevenueChart: function (canvasId, labels, ebayData, servicesData) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        return new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'eBay Revenue',
                        data: ebayData,
                        borderColor: this.colors.ebay,
                        backgroundColor: this.colors.ebay + '20',
                        tension: 0.3,
                        fill: true
                    },
                    {
                        label: 'Service Revenue',
                        data: servicesData,
                        borderColor: this.colors.services,
                        backgroundColor: this.colors.services + '20',
                        tension: 0.3,
                        fill: true
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return context.dataset.label + ': ' + ChartHelper.formatCurrency(context.parsed.y);
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function (value) {
                                return ChartHelper.formatCurrency(value);
                            }
                        }
                    }
                }
            }
        });
    },

    // Create an expense breakdown doughnut chart
    createExpenseChart: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        const backgroundColors = [
            this.colors.primary,
            this.colors.success,
            this.colors.warning,
            this.colors.danger,
            this.colors.info,
            this.colors.secondary
        ];

        return new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: backgroundColors.slice(0, data.length),
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right'
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((context.parsed / total) * 100).toFixed(1);
                                return context.label + ': ' + ChartHelper.formatCurrency(context.parsed) + ' (' + percentage + '%)';
                            }
                        }
                    }
                }
            }
        });
    },

    // Create a profit comparison bar chart
    createProfitChart: function (canvasId, labels, revenueData, expenseData, profitData) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        return new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Revenue',
                        data: revenueData,
                        backgroundColor: this.colors.primary
                    },
                    {
                        label: 'Expenses',
                        data: expenseData,
                        backgroundColor: this.colors.danger
                    },
                    {
                        label: 'Profit',
                        data: profitData,
                        backgroundColor: this.colors.success
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                return context.dataset.label + ': ' + ChartHelper.formatCurrency(context.parsed.y);
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function (value) {
                                return ChartHelper.formatCurrency(value);
                            }
                        }
                    }
                }
            }
        });
    }
};
