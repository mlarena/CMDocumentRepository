// Переключение темы
function initTheme() {
    const saved = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-bs-theme', saved);
}

function toggleTheme() {
    const current = document.documentElement.getAttribute('data-bs-theme');
    const next = current === 'dark' ? 'light' : 'dark';
    document.documentElement.setAttribute('data-bs-theme', next);
    localStorage.setItem('theme', next);
}

// Горячие клавиши
document.addEventListener('keydown', function(e) {
    // Ctrl+S - сохранить форму
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        const form = document.querySelector('form[method="post"]');
        if (form) form.submit();
    }
    // Ctrl+F - фокус на поиск
    if (e.ctrlKey && e.key === 'f') {
        e.preventDefault();
        const search = document.querySelector('input[name="keyword"]');
        if (search) search.focus();
    }
});

// Toast уведомления
function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container') || createToastContainer();
    const toast = document.createElement('div');
    toast.className = `alert alert-${type} alert-dismissible fade show`;
    toast.innerHTML = `${message}<button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 5000);
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toast-container';
    container.style.cssText = 'position:fixed;top:20px;right:20px;z-index:9999;max-width:350px;';
    document.body.appendChild(container);
    return container;
}

// Массовый выбор
function toggleAll(source) {
    const checkboxes = document.querySelectorAll('.select-item');
    checkboxes.forEach(cb => cb.checked = source.checked);
}

function getSelectedIds() {
    return Array.from(document.querySelectorAll('.select-item:checked')).map(cb => cb.value);
}

// Инициализация
document.addEventListener('DOMContentLoaded', function() {
    initTheme();
    // Автоскрытие ошибок из query string
    var errContainer = document.getElementById('error-toast-container');
    if (errContainer) {
        // Удалить параметр error из URL без перезагрузки
        var url = new URL(window.location);
        url.searchParams.delete('error');
        window.history.replaceState({}, '', url);
        // Автоскрытие через 5 секунд
        setTimeout(function() { errContainer.remove(); }, 5000);
    }
});
