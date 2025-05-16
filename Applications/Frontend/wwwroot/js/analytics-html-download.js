// Функционал для скачивания HTML-версии страницы
(function() {
    // Функция для получения всех стилей страницы
    function getPageStyles() {
        const styles = [];
        const styleSheets = document.styleSheets;

        for (let i = 0; i < styleSheets.length; i++) {
            try {
                const rules = styleSheets[i].cssRules || styleSheets[i].rules;
                if (rules) {
                    for (let j = 0; j < rules.length; j++) {
                        styles.push(rules[j].cssText);
                    }
                }
            } catch (e) {
                console.warn('Не удалось получить стили из таблицы стилей:', e);
            }
        }

        return styles.join('\n');
    }

    // Функция для получения всех скриптов страницы
    function getPageScripts() {
        const scripts = [];
        const scriptElements = document.getElementsByTagName('script');

        for (let i = 0; i < scriptElements.length; i++) {
            const script = scriptElements[i];
            if (script.src) {
                scripts.push(`<script src="${script.src}"><\/script>`);
            } else if (script.textContent) {
                scripts.push(`<script>${script.textContent}<\/script>`);
            }
        }

        return scripts.join('\n');
    }

    // Функция для получения всех внешних ресурсов (шрифты, изображения и т.д.)
    function getExternalResources() {
        const resources = [];
        const links = document.getElementsByTagName('link');

        for (let i = 0; i < links.length; i++) {
            const link = links[i];
            if (link.rel === 'stylesheet' || link.rel === 'icon' || link.rel === 'shortcut icon') {
                resources.push(`<link rel="${link.rel}" href="${link.href}"${link.type ? ` type="${link.type}"` : ''}>`);
            }
        }

        return resources.join('\n');
    }

    // Функция для создания полного HTML-документа
    function createFullHtmlDocument() {
        const title = document.title;
        // Получаем содержимое блока с информацией об отчете
        const reportInfo = document.querySelector('.card.mb-4').outerHTML;
        // Получаем содержимое блока с графиками
        const content = document.getElementById('reportContent').innerHTML;
        const styles = getPageStyles();
        const scripts = getPageScripts();
        const resources = getExternalResources();

        return `<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>${title}</title>
    ${resources}
    <style>
        ${styles}
        /* Дополнительные стили для печати */
        @media print {
            .no-print {
                display: none !important;
            }
            .container-fluid {
                width: 100%;
                padding: 0;
                margin: 0;
            }
            body {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <div class="container-fluid print-page">
        <div class="mt-1">
            ${reportInfo}
        </div>
        ${content}
    </div>
    ${scripts}
</body>
</html>`;
    }

    // Функция для скачивания HTML-файла
    function downloadHtml() {
        const html = createFullHtmlDocument();
        const blob = new Blob([html], { type: 'text/html;charset=utf-8' });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        
        // Получаем название файла так же, как при печати
        const section = $(".card.mb-4 .bi-graph-up").closest('.col-md-3').find('p.mb-0').text().trim();
        const period = $(".card.mb-4 .bi-calendar-range").closest('.col-md-3').find('p.mb-0').text().trim();
        const filename = `${section} за ${period}.html`;
        
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }

    // Добавляем обработчик для кнопки скачивания HTML
    document.addEventListener('DOMContentLoaded', function() {
        const downloadHtmlButton = document.getElementById('downloadHtml');
        if (downloadHtmlButton) {
            downloadHtmlButton.addEventListener('click', function() {
                // Устанавливаем светлую тему для графиков перед скачиванием
                if (window.setAllChartsLightTheme) {
                    window.setAllChartsLightTheme();
                }
                
                // Ждем обновления графиков
                requestAnimationFrame(() => {
                    requestAnimationFrame(() => {
                        downloadHtml();
                    });
                });
            });
        }
    });
})(); 