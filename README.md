# ProjectManager

Mono-repo: серверная часть на .NET 9 и клиентская часть на React (Vite).

Коротко
- Backend: ASP.NET Core Web API (.NET 9), EF Core, Identity (int keys)
- Frontend: React + Vite (папка App/), собирается npm -> статические файлы в dist, раздача через nginx в контейнере
- База: MS SQL Server (Docker)

Структура репозитория

- ProjectManager.slnx — решение
- ProjectManager.Api/ — ASP.NET Core API (Dockerfile присутствует)
- ProjectManager.Application/ — доменная логика, DTO
- ProjectManager.Infrastructure/ — репозитории, конфигурация, Identity, DI
- ProjectManager.Entities.Models/ — сущности данных
- App/ — клиентское приложение (React + Vite)
- compose.yaml — файл сборки docker-compose
- .env — параметры окружения (используется compose)

Технологии и зависимости

- .NET 9, ASP.NET Core
- Entity Framework Core (SQL Server)
- ASP.NET Identity
- Serilog
- React, Vite, npm
- nginx (для раздачи собранного клиента в контейнере)

Быстрый старт (через Docker Compose)

1) В корне репозитория настроены compose.yaml и .env. По умолчанию .env содержит пример значений (пароли/секреты тестовые).

2) Собрать и запустить все сервисы в фоне:

   docker compose -f compose.yaml up -d --build

3) Остановить и удалить контейнеры:

   docker compose -f compose.yaml down

Что поднимает docker-compose
- db — MS SQL Server (контейнер mcr.microsoft.com/mssql/server)
- projectmanager.api — backend, строится из ProjectManager.Api/Dockerfile, слушает URL, указанный в ASPNETCORE_URLS
- projectmanager.client — frontend, строится из App/Dockerfile, раздаётся через nginx, проброшен внешний порт 3000

Переменные окружения (.env)

Файл .env в корне используется docker-compose и содержит значения для приложения. В проекте применён стиль, где двойное подчеркивание используется для доступа к вложенным настройкам .NET (пример):

- DataBaseConnectionSettings__Host — хост БД (в контейнере db)
- DataBaseConnectionSettings__Port — порт БД
- DataBaseConnectionSettings__Database — имя БД
- DataBaseConnectionSettings__Username — имя пользователя БД
- DataBaseConnectionSettings__Password — пароль SA
- JwtSettings__SecretKey, JwtSettings__Issuer, JwtSettings__Audience — настройки JWT
- ASPNETCORE_ENVIRONMENT, ASPNETCORE_URLS — окружение и URL для API
- VITE_API_URL — адрес API, используемый клиентом (в режиме сборки/запросов)

Важно: значения секретов (.env) в репозитории — только для локальной разработки. В продакшене замените их на безопасные значения.

Клиент (App/)

- Для локальной разработки клиента можно использовать стандартные npm-скрипты (в App/package.json). Примеры:
  - npm install (или npm ci)
  - npm run dev — запуск Vite для разработки
  - npm run build — сборка в папку dist

- Dockerfile клиента (App/Dockerfile) делает сборку через node и копирует /dist в nginx для статической раздачи.

API и базы данных

- При старте контейнера API выполняются миграции EF Core (db.Database.Migrate()).
- После миграций API создаёт роли и сидирует тестовых пользователей: по 3 пользователя для каждой роли (Director, Manager, Employee). Пароль тестовых пользователей: `1234`.

Локальная разработка без Docker

- Backend: открыть ProjectManager.Api в Visual Studio / VS Code и запустить (dotnet run или через IDE).
- Frontend: зайти в App/, выполнить npm ci и npm run dev. Убедиться, что переменная VITE_API_URL указывает на адрес запущенного API.

Проблемы и отладка

- Логи API пишутся в папку Logs (конфигурация Serilog в ProjectManager.Api/appsettings.json). В compose API монтирует volume для логов.
- Проблемы с подключением к БД в Docker: проверьте переменные в .env и что контейнер db поднялся и слушает.

Советы по продакшену

- Не храните секреты в репозитории. Используйте секреты окружения, vault или CI/CD секреты.
- В продакшене включите TLS/HTTPS (nginx/ingress или прокси), обновите настройки шифрования и параметры Identity.

Контакт и вклад

PR и issue приветствуются. Для contribution следуйте стандартному workflow: fork -> branch -> PR.

Лицензия

По умолчанию без лицензии. Добавьте LICENSE файл при необходимости.

