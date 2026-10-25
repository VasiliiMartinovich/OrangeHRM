# OrangeHRM Test Automation Framework

Automated UI, API, and BDD tests for the [OrangeHRM demo application](https://opensource-demo.orangehrmlive.com/). The solution is built with C# and .NET 10 and uses Selenium WebDriver, NUnit, Reqnroll, RestSharp, and Allure reporting.

## Contents

- [Project overview](#project-overview)
- [Technology stack](#technology-stack)
- [Architecture](#architecture)
- [Test coverage](#test-coverage)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Run tests](#run-tests)
- [Allure reports](#allure-reports)
- [Cross-browser execution](#cross-browser-execution)

## Project overview

The solution separates reusable automation components from test scenarios:

- **OrangeHRM.Framework.Core** — shared UI automation components, browser management, page objects, element wrappers, and API clients/models.
- **OrangeHRM.Tests** — UI and API tests, Reqnroll feature files and step definitions, test configuration, hooks, logging, and test-data helpers.

Solution file: `OrangeHRM.sln`.

## Technology stack

- C# / .NET 10
- Selenium WebDriver and Selenium.Support
- NUnit
- Reqnroll for BDD scenarios
- RestSharp for API requests
- Allure.NUnit and Allure.Reqnroll for test reporting
- log4net for logging
- Microsoft.Extensions.Configuration for JSON and environment-variable configuration
- PowerShell script for running UI tests in multiple browsers

## Architecture

### Business / test layer

Located mainly in `OrangeHRM.Tests`.

- **UI tests** describe and verify user-facing workflows.
- **API tests** verify endpoints, response status codes, response data, validation, and candidate operations.
- **BDD features** describe behavior in Gherkin; step definitions connect the scenarios to page objects and test-data setup.
- **Assertions** check expected application behavior and API responses.

### Technical layer

Located mainly in `OrangeHRM.Framework.Core`, with test infrastructure in `OrangeHRM.Tests`.

- **Drivers** — `DriverFactory`, `DriverManager`, and `BrowserType` create and manage browser sessions.
- **Pages and components** — Page Object classes encapsulate page locators and actions. `BasePage` and `LoadablePage` provide shared page behavior; `SidePanel` is a reusable navigation component.
- **Element wrappers** — `BaseElement`, `Button`, `TextBox`, `Label`, and `Drop-down` provide reusable element interactions.
- **API clients and models** — API clients are grouped by resource (employees, job titles, vacancies, and candidates); request and response models represent payloads.
- **Test infrastructure** — settings, Reqnroll hooks, UI test base, logging, Allure attachments, and API test-data helpers support test execution and diagnostics.

## Test coverage

### UI tests

Current UI test areas include:

- Authentication: successful and unsuccessful login, logout.
- Employees / PIM: employee-related workflows and employee list search.
- Recruitment: candidates, vacancies, and hiring workflow.

### API tests

Current API test areas include:

- Employees
- Job titles
- Vacancies
- Candidates, including candidate lifecycle and validation scenarios

### BDD scenarios

Gherkin feature files currently cover:

- **Authentication** — valid and invalid login.
- **Employee list** — opening the list and searching for a non-existing employee.
- **Recruitment** — creating a candidate with a CV and vacancy, deleting a candidate, and creating a vacancy with a hiring manager.

Test coverage can change as the project evolves; see the test classes and `.feature` files for the current scenarios.

## Prerequisites

Install the following:

- .NET 10 SDK
- Git
- Chrome and/or Firefox for UI tests
- Allure command-line tool if you want to generate and open HTML reports

The tests use the public OrangeHRM demo environment, so availability and test data may change outside this project.

## Configuration

Test settings are loaded from `OrangeHRM.Tests/appsettings.json` and environment variables.

The current demo configuration contains:

- Base URL: `https://opensource-demo.orangehrmlive.com/`
- Browser: `Chrome`
- Headless mode: `true`
- Explicit wait timeout: `10` seconds
- Demo login: `Admin` / `admin123`

You can change `Browser`, `Headless`, and `TimeoutSeconds` in `appsettings.json`, or override configuration values with environment variables.

**API tests require an access token.** Set the `ORANGEHRM_ACCESS_TOKEN` environment variable before running them. Do not commit personal tokens or other secrets to the repository.

PowerShell example for the current terminal session:

```powershell
$env:ORANGEHRM_ACCESS_TOKEN = "YOUR_ACCESS_TOKEN"
```

## Run tests

Run commands from the repository root.

Restore dependencies and build the solution:

```powershell
dotnet restore OrangeHRM.sln
dotnet build OrangeHRM.sln
```

Run all tests:

```powershell
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj
```

Run UI tests only:

```powershell
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj --filter "FullyQualifiedName!~OrangeHRM.Tests.API"
```

Run API tests only (requires `ORANGEHRM_ACCESS_TOKEN`):

```powershell
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj --filter "FullyQualifiedName~OrangeHRM.Tests.API"
```

Run BDD scenarios by feature name:

```powershell
dotnet test OrangeHRM.Tests/OrangeHRM.Tests.csproj --filter "FullyQualifiedName~AuthenticationFeature"
```

The exact generated test names can depend on Reqnroll code generation; use `dotnet test --list-tests` to inspect available test names.

## Allure reports

The project configures Allure integration through `Allure.NUnit`, `Allure.Reqnroll`, and `OrangeHRM.Tests/allureConfig.json`. Test results are written to the test output's `allure-results` directory.

After running tests, generate and open a report with the Allure CLI:

```powershell
allure generate OrangeHRM.Tests/bin/Debug/net10.0/allure-results --clean -o allure-report-local
allure open allure-report-local
```

The repository also contains previously generated Allure report folders. These are snapshots from earlier executions and may not represent the latest local test run.

## Cross-browser execution

Run the supplied PowerShell script from the repository root:

```powershell
.\run-ui-cross-browser.ps1
```

The script runs UI tests in **Chrome** and **Firefox**, keeps the results for each browser under `allure-results/Chrome` and `allure-results/Firefox`, and returns a non-zero exit code if a browser run fails.

The repository includes `Test.runsettings` and `Test.Parallel.runsettings` with different NUnit worker settings. Select the runsettings file deliberately when running tests; parallel execution should be verified against the current test setup and shared test data.

## Repository structure

```text
OrangeHRM/
├── OrangeHRM.sln
├── OrangeHRM.Framework.Core/
│   ├── API/
│   │   ├── Clients/
│   │   └── Models/
│   │       ├── Requests/
│   │       └── Responses/
│   ├── Drivers/
│   ├── Pages/
│   │   ├── Authentication/
│   │   ├── Components/
│   │   ├── Employees/
│   │   └── Recruitment/
│   └── Wrappers/
├── OrangeHRM.Tests/
│   ├── API/
│   ├── Configuration/
│   ├── Features/
│   ├── Infrastructure/
│   ├── StepDefinitions/
│   ├── TestData/
│   └── UI/
├── Test.runsettings
├── Test.Parallel.runsettings
└── run-ui-cross-browser.ps1
```
