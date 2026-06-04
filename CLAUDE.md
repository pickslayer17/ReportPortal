# CLAUDE.md — ReportPortal

> Этот файл авто-загружается каждой сессией. Здесь: обзор проекта, команды, и активный бэклог рефакторинга.
> Локальные секреты/состояние сессии (токен Telegram, заметки по пермишенам) — в `.claude/SESSION_HANDOFF.md` (gitignored).

## Что это
Система отчётности по тестам. Функционал намеренно простой:
1. Хранить тесты, группировать по прогонам (runs) и проектам.
2. Отдавать данные фронту для отображения (дерево папок → тесты → результаты).
3. Давать пользователям помечать отсмотренные тесты (TestReview: ревьюер, исход, комментарий).
Реалтайм-обновления — через SignalR.

## Стек и структура
- **Backend**: ASP.NET Core 8 (`net8.0`), EF Core 8, слои `ReportPortal` (API) / `ReportPortal.BL` (сервисы) / `ReportPortal.DAL` (EF, репозитории).
- **Frontend**: Vite + React + TypeScript, `ReactFE/reactfe.client`. SignalR-клиент.
- **DB**: MSSQL. Миграции EF Core.
- **Docker**: `docker-compose.yml` (sql, git-clone, builder, migrator, backend, frontend).

## Окружение (важно)
- ОС: Windows 11, оболочка **PowerShell**. **Bash-инструмент сломан** (msys fatal error) — использовать PowerShell или прямые dotnet-команды.
- Установлен **.NET SDK 10.0.300**, проект таргетит **net8.0** (собирается, но при желании запинить SDK через `global.json`).
- Каталог: `C:\Workspace\ReportPortal`. Ветка: `masterGitHub`.

## Команды
```
dotnet build ReportPortal.sln                 # сборка решения
dotnet run --project ReportPortal             # запуск API
dotnet ef database update --project ReportPortal.DAL --startup-project ReportPortal
# фронт:
#   cd ReactFE/reactfe.client; npm install; npm run dev
```
`Bash(dotnet *)` уже в allowlist — билд/тесты идут без апрува.

## Конвенции
- Conventional commits (`feat(scope):`, `fix(scope):`, …), английский, императив, один коммит = одно изменение.
- Не коммитить секреты. Править существующее предпочтительнее создания новых файлов.
- Все правки/комментарии/доки — на английском.

---

# АКТИВНЫЙ БЭКЛОГ РЕФАКТОРИНГА
Цель: тот же функционал, но чистый код и правильные подходы. Полный разбор (63 пункта) сделан в сессии 2026-06-04. Ниже — приоритизировано.

## P0 — ломает функционал / данные (делать первым)
1. **TRX-аплоад пишет всё в run #2.** `TrxParserService.cs:33` — захардкожен `runId = 2;`, затирает параметр. Плюс `RunManagementController.UploadTrxFile:103` не передаёт `runId`/`projectId` (маршрут `Project/{projectId}/upload-trx` игнорирует projectId). Пробросить реальные id, убрать хардкод.
2. **Удаление прогона не работает.** `RunService.DeleteByIdAsync` (`RunService.cs:62`) = `NotImplementedException`, а контроллер его зовёт → всегда BadRequest. Реализовать (каскадное удаление folders/tests/results через `ExecuteDelete` в транзакции).
3. **Защита от дублей тестов не срабатывает.** `TestService.CreateAsync:39` берёт папку через `FolderRepository.GetByAsync` = `AsNoTracking` без `Include(Tests)` + lazy → `folder.Tests == null` → проверка пропускается. Грузить тесты явно.
4. **Парсер падает на нестандартных исходах.** `TrxParserService.GetOutcome:75` кидает `NotImplementedException` на всё кроме Passed/Failed/NotExecuted (реальны Inconclusive/Timeout/Aborted/Error). `TrxHelper.cs:30` `.First()` кидает при отсутствии результата. Обработать.
5. **Схема vs модель: `TestResult.ScreenShot` NOT NULL в снапшоте** (`ApplicationContextModelSnapshot.cs:154`), но парсер его не заполняет → вставка падает. Сделать nullable + миграция.
6. **Path traversal в аплоаде.** `RunManagementController.cs:96` — `file.FileName` в `Path.Combine`. Использовать `Path.GetFileName`. Плюс temp-файлы не удаляются (`:93-101`) — чистить в `finally`.

## P1 — архитектура, перформанс, корректность
7. **Индекс на `Test.RunId`** — главный фильтр (`TestRepository.cs:15`), индекса нет (есть только FolderId). Добавить. Заодно решить судьбу «висячей» колонки `Test.RunId` без FK к `Run`.
8. **N+1 в рекурсии папок.** `FolderService` (`GetIdOrAddFolderInRunAsync`/`GetIdOrAddFolderAsync`, доступ к `run.Folders`, `parentFolder.Children`) тянет БД лениво по узлу. Выключить глобальный `LazyLoadingProxies` (`Program.cs:32`), грузить дерево одним запросом.
9. **`GetAllByAsync` грузит всю таблицу и фильтрует в памяти.** `RunService.cs:73` (`r => true` + `predicate.Compile()`); `GetAllRuns` тянет все прогоны всех проектов. Предикат должен уходить в SQL.
10. **Нет Unit of Work / транзакций.** Каждый репозиторий сам зовёт `SaveChanges`; TRX-цикл делает тысячи round-trip'ов без атомарности. Ввести UoW/транзакцию + батч-вставку.
11. **Блокирующий `.Result` поверх async.** `RunService.cs:55`, `ProjectManagementController.cs:49` → `await`.
12. **Создание run+rootFolder без транзакции** (`RunService.CreateAsync:46,55`) → возможен осиротевший run. Обернуть в транзакцию.
13. **`ReviewerId` берётся из тела запроса**, а не из аутентификации (`TestReviewManagementController.cs:59`). Брать текущего пользователя из claims.
14. **Две перегрузки `UpdateTestReviewAsync` с разной семантикой** (`TestReviewService.cs:27` делает `SetValues` поверх всего → затирает поля). Свести к точечному апдейту.
15. **Нет пагинации** ни на одном списочном эндпоинте (`GetAllRunTests` отдаёт все 10K). Ввести server-side paging/keyset + фильтры по исходу.
16. **Фронт: на каждое SignalR-событие — повторный фетч всего** (`RunPage.tsx:119-137`); водопад из 4 запросов на загрузке (`:70-95`); нет виртуализации списка (`:312-448`); агрегаты/`getAllTestsForFolder` без мемоизации (`:212-217,256-282`). Точечные мутации стейта + `Promise.all` + `react-window` + `useMemo`/`Map`.
17. **Фронт: жизненный цикл SignalR** разнесён на два `useEffect` с гонкой соединений (`RunPage.tsx:97-181`). Вынести в `useSignalR` с корректным cleanup.

## P2 — качество, SOLID/DRY/KISS, мелочи
18. **`IServiceBase`/`IRepository<T>` навязывают CRUD всем** → лес `NotImplementedException` (ISP/LSP). Разбить на мелкие интерфейсы.
19. **DRY репозиториев**: одинаковые `Insert/Remove/GetBy` скопированы в 7 классов — вынести в дженерик-базу `Repository<T>`.
20. **Непредсказуемый `GetByAsync`**: где-то throw, где-то null. Унифицировать (`GetByIdAsync`→null + `GetRequired`→throw).
21. **`nvarchar(MAX)` на всех строках** (Name/Email) — ограничить длину, дать индексируемость.
22. **`User.Email` без unique index**; **`User.Id` как `int?`** — починить.
23. **Нет concurrency-токена (rowversion)** на TestReview → потерянные обновления.
24. **Унести проекции-маппинг из репозиториев** (`TestRepository.cs:19-43`, `FolderRepository.cs:19-27`) в сервис/запрос.
25. **`ExceptionHandlingMiddleware` отдаёт `ex.Message` клиенту** (`:32`) и контроллеры тоже — убрать в проде; доменные «уже существует» → 409, не 500.
26. **Лишние DI-зависимости** (`RunService` использует 3 из 7), мёртвый код: `GenerateSalt()` нулевой GUID (`AuthenticationService.cs:68`), `break` после `return` (`TrxParserService.cs:67-73`), `Console.WriteLine` (`FolderRepository.cs:12`), `catch{throw ex;}` (`AuthenticationService.cs:41`), пустой `MainPage.tsx`, кнопка Edit без onClick (`SettingsPage.tsx:147`).
27. **Каша в namespace'ах**: `UserRepository` в `ReportPortal.Services`, `TrxParserService` (класс) в `...Services.Interfaces`, DTO в разных неймспейсах. Опечатка папки `Constatnts`.
28. **CancellationToken теряется** по цепочке (аплоад, RunService, рекурсия удаления папок).
29. **Работа с юзерами отсутствует**: нет смены пароля, обновления профиля, `/me`, валидации при создании; `GetUsers` открыт любому. Нет связи user↔project (мультиарендности нет).
30. **TS-качество фронта**: `any` (`auth.tsx:7`, `SettingsPage.tsx:16-17`), небезопасный каст enum (`EditTestReviewModal.tsx:72`), дублирование извлечения токена и fetch-обёрток (`api.ts`), хардкод URL, рассинхрон карты исходов (`TestPage.tsx:22-27`), нет обработки 401/тостов.

## P3 — инфраструктура / docker
31. Сборка из `git clone` публичного master вместо локального кода (`docker-compose.yml:20-26`).
32. Оркестрация на `sleep`+флаг-файлах вместо healthcheck'ов.
33. Рассинхрон env строки подключения (`ConnectionStrings__DefaultConnection` vs `DefaultConnection`).
34. «Прод»-компоуз поднимает `Development` (Swagger/DevExceptionPage наружу).
35. `VITE_API_URL` зашит в билд — нет рантайм-конфига фронта.
36. Локально: `FrontEndUrl=http://localhost:100` (опечатка, должно 3000) → CORS режет фронт; `Urls=https://...:5002` + `UseHttpsRedirection` при http-only Kestrel.
37. Секреты в открытом виде (docker-compose/appsettings/init.sql/seeder) — вынести в User Secrets/env/Key Vault, ротировать. db-init даёт приложению `db_owner` — урезать права.

---
## Прогресс
- [x] Полный анализ (сессия 2026-06-04).
- [x] **P0 (пункты 1–6) реализованы (2026-06-04).** Билд зелёный.
  - #1 аплоад создаёт Run и пробрасывает id (хардкод `runId=2` убран); #4 парсер исходов не падает + `.First()`→`FirstOrDefault`; #6 path traversal (`Path.GetFileName`) + чистка temp в `finally`; #5 `TestResult.ScreenShot`→nullable + миграция `ScreenShotNullable`; #3 дубли тестов через SQL `EXISTS`; #2 каскадное удаление прогона в транзакции (UoW).
  - Доп.: passed-тесты больше не выкидываются (фильтр убран); `RunService.CreateAsync` в транзакции, блокирующий `.Result` убран.
  - ⚠️ Миграция `ScreenShotNullable` создана, но к БД НЕ применена (`dotnet ef database update`).
- [ ] Дальше: P1 (пункты 7–17).
