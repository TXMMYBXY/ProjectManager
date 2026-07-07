# ProjectsControl
Сервис для управления проектами и сотрудниками с поддержкой статусов, документов и контейнеризацией через Docker.

## Содержание
- [ProjectsControl](#projectscontrol)
  - [Содержание](#содержание)
  - [Стек технологий](#стек-технологий)
    - [Backend](#backend)
    - [Frontend](#frontend)
  - [Архитектура проекта](#архитектура-проекта)
  - [Архитектурный подход](#архитектурный-подход)
  - [Docker архитектура](#docker-архитектура)
  - [Установка и запуск](#установка-и-запуск)
  - [Основные возможности](#основные-возможности)
  - [Бизнес-правила](#бизнес-правила)
  - [API Документация](#api-документация)
  - [Автор](#автор)


## Стек технологий
### Backend
- ASP.NET Core 9
- Entity Framework Core
- MS SQL Server 2022
- AutoMapper
- Scalar
- Docker
### Frontend
- React (Vite)
- TailwindCSS
- Nginx (production proxy)

## Архитектура проекта

```
ProjectsControl
│
├── ProjectsControl.Api              → Web API (Controllers, Middleware)
├── ProjectsControl.Application      → Бизнес-логика, DTO, сервисы
├── ProjectsControl.Entities         → Модели и DbContext
├── ProjectsControl.Infrastructure   → Репозитории, реализация зависимостей
├── ProjectsControl.Utils            → Вспомогательные классы и методы
├── project-control-reactapp         → React клиент
├── docker-compose.yml               → Оркестрация контейнеров
└── .env.example                     → Пример конфигурации
```

## Архитектурный подход
- Разделение слоёв (API / Application / Infrastructure / Entities / Utils / reactapp)
- Repository Pattern
- Service Layer
- DTO separation
- Частичное обновление (PATCH)
- Production-ready Docker setup
- Один входной порт через Nginx

## Docker архитектура
```
Browser
   ↓
Nginx (Client)
   ↓
ASP.NET API
   ↓
SQL Server
```
## Установка и запуск
Клонировать репозиторий:
```bash
git clone https://github.com/TXMMYBXY/ProjectsControl.git
```

Созать файл конфигурации из примера:
```bash
cp .env.example .env
```
Настроить файл конфигурации `.env` и запустить контейнеры

```bash
docker compose up --build
```
Удалить контейнеры
```bash
docker-compose down
```
Чтобы удалить контейнеры и том где хранятся данные
```bash
docker-compose down -v
```

## Основные возможности
- CRUD проектов
- CRUD сотрудников
- Назначение руководителя проекта
- Управление составом команды
- Статусы проекта (`Backlog` / `Планирование` / `Активный` / `Приостановлен` / `Завершен` / `Архив`)
- Валидация бизнес-правил
- Загрузка и скачивание документов
- Автоматические миграции при старте
  
## Бизнес-правила
- Проект вне Backlog должен иметь руководителя
- При удалении сотрудника управляемые проекты архивируются

## API Документация
<details>
<summary>Документация</summary>

```json
{
  "openapi": "3.0.4",
  "info": {
    "title": "Сервис контроля проектов",
    "version": "v1"
  },
  "paths": {
    "/api/document/{projectId}/upload": {
      "post": {
        "tags": [
          "Document"
        ],
        "parameters": [
          {
            "name": "projectId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "requestBody": {
          "content": {
            "multipart/form-data": {
              "schema": {
                "required": [
                  "File"
                ],
                "type": "object",
                "properties": {
                  "File": {
                    "type": "string",
                    "format": "binary"
                  }
                }
              },
              "encoding": {
                "File": {
                  "style": "form"
                }
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/document/{documentId}/download": {
      "get": {
        "tags": [
          "Document"
        ],
        "parameters": [
          {
            "name": "documentId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/employee": {
      "get": {
        "tags": [
          "Employee"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/GetEmployeeViewModel"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/GetEmployeeViewModel"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/GetEmployeeViewModel"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Employee"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateEmployeeViewModel"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateEmployeeViewModel"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateEmployeeViewModel"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CreateEmployeeViewModel"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CreateEmployeeViewModel"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CreateEmployeeViewModel"
                }
              }
            }
          }
        }
      }
    },
    "/api/employee/{employeeId}": {
      "get": {
        "tags": [
          "Employee"
        ],
        "parameters": [
          {
            "name": "employeeId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/GetEmployeeViewModel"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/GetEmployeeViewModel"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/GetEmployeeViewModel"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "Employee"
        ],
        "parameters": [
          {
            "name": "employeeId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/employee/{employeeId}/info": {
      "patch": {
        "tags": [
          "Employee"
        ],
        "parameters": [
          {
            "name": "employeeId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateEmployeeViewModel"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateEmployeeViewModel"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateEmployeeViewModel"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/employee/{employeeId}/projects": {
      "patch": {
        "tags": [
          "Employee"
        ],
        "parameters": [
          {
            "name": "employeeId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeProjectEmployeeViewModel"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeProjectEmployeeViewModel"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeProjectEmployeeViewModel"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/project": {
      "get": {
        "tags": [
          "Project"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/GetProjectViewModel"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/GetProjectViewModel"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/GetProjectViewModel"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Project"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateProjectViewModel"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateProjectViewModel"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateProjectViewModel"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/project/{projectId}": {
      "get": {
        "tags": [
          "Project"
        ],
        "parameters": [
          {
            "name": "projectId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/GetProjectViewModel"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/GetProjectViewModel"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/GetProjectViewModel"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "Project"
        ],
        "parameters": [
          {
            "name": "projectId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/project/{projectId}/info": {
      "patch": {
        "tags": [
          "Project"
        ],
        "parameters": [
          {
            "name": "projectId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProjectViewModel"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProjectViewModel"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProjectViewModel"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/project/{projectId}/status-and-employees": {
      "patch": {
        "tags": [
          "Project"
        ],
        "parameters": [
          {
            "name": "projectId",
            "in": "path",
            "required": true,
            "schema": {
              "type": "integer",
              "format": "int32"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeEmployeesOnProjectViewModel"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeEmployeesOnProjectViewModel"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeEmployeesOnProjectViewModel"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "ChangeEmployeesOnProjectViewModel": {
        "type": "object",
        "properties": {
          "projectManagerId": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "employeesIds": {
            "type": "array",
            "items": {
              "type": "integer",
              "format": "int32"
            },
            "nullable": true
          },
          "status": {
            "$ref": "#/components/schemas/ProjectStatus"
          }
        },
        "additionalProperties": false
      },
      "ChangeProjectEmployeeViewModel": {
        "type": "object",
        "properties": {
          "projectsIds": {
            "type": "array",
            "items": {
              "type": "integer",
              "format": "int32"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateEmployeeViewModel": {
        "required": [
          "firstName",
          "lastName"
        ],
        "type": "object",
        "properties": {
          "firstName": {
            "maxLength": 63,
            "minLength": 1,
            "type": "string"
          },
          "lastName": {
            "maxLength": 63,
            "minLength": 1,
            "type": "string"
          },
          "patronymic": {
            "maxLength": 63,
            "type": "string",
            "nullable": true
          },
          "email": {
            "maxLength": 63,
            "type": "string",
            "nullable": true
          },
          "phoneNumber": {
            "maxLength": 31,
            "type": "string",
            "nullable": true
          },
          "projectsIds": {
            "type": "array",
            "items": {
              "type": "integer",
              "format": "int32"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateProjectViewModel": {
        "required": [
          "customerCompany",
          "performingCompany",
          "startDate",
          "title"
        ],
        "type": "object",
        "properties": {
          "title": {
            "minLength": 1,
            "type": "string"
          },
          "customerCompany": {
            "minLength": 1,
            "type": "string"
          },
          "performingCompany": {
            "minLength": 1,
            "type": "string"
          },
          "employeesIds": {
            "type": "array",
            "items": {
              "type": "integer",
              "format": "int32"
            },
            "nullable": true
          },
          "projectManagerId": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "startDate": {
            "type": "string",
            "format": "date-time"
          },
          "finishDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "priority": {
            "maximum": 10,
            "minimum": 0,
            "type": "integer",
            "format": "int32"
          },
          "status": {
            "$ref": "#/components/schemas/ProjectStatus"
          }
        },
        "additionalProperties": false
      },
      "EmployeeInProjectDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "format": "int32"
          },
          "firstName": {
            "type": "string",
            "nullable": true
          },
          "lastName": {
            "type": "string",
            "nullable": true
          },
          "patronymic": {
            "type": "string",
            "nullable": true
          },
          "email": {
            "type": "string",
            "nullable": true
          },
          "phoneNumber": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "GetDocumentDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "format": "int32"
          },
          "fileName": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "GetEmployeeViewModel": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "format": "int32"
          },
          "firstName": {
            "type": "string",
            "nullable": true
          },
          "lastName": {
            "type": "string",
            "nullable": true
          },
          "patronymic": {
            "type": "string",
            "nullable": true
          },
          "email": {
            "type": "string",
            "nullable": true
          },
          "phoneNumber": {
            "type": "string",
            "nullable": true
          },
          "projects": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/ProjectByEmployeeDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "GetProjectViewModel": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "format": "int32"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "customerCompany": {
            "type": "string",
            "nullable": true
          },
          "performingCompany": {
            "type": "string",
            "nullable": true
          },
          "employees": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/EmployeeInProjectDto"
            },
            "nullable": true
          },
          "projectManager": {
            "$ref": "#/components/schemas/EmployeeInProjectDto"
          },
          "startDate": {
            "type": "string",
            "format": "date-time"
          },
          "endDate": {
            "type": "string",
            "format": "date-time"
          },
          "priority": {
            "type": "integer",
            "format": "int32"
          },
          "status": {
            "$ref": "#/components/schemas/ProjectStatus"
          },
          "documents": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/GetDocumentDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "ProjectByEmployeeDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "format": "int32"
          },
          "title": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "ProjectStatus": {
        "enum": [
          0,
          1,
          2,
          3,
          4,
          5
        ],
        "type": "integer",
        "format": "int32"
      },
      "UpdateEmployeeViewModel": {
        "type": "object",
        "properties": {
          "firstName": {
            "maxLength": 63,
            "type": "string",
            "nullable": true
          },
          "lastName": {
            "maxLength": 63,
            "type": "string",
            "nullable": true
          },
          "patronymic": {
            "maxLength": 63,
            "type": "string",
            "nullable": true
          },
          "email": {
            "maxLength": 63,
            "type": "string",
            "nullable": true
          },
          "phoneNumber": {
            "maxLength": 31,
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateProjectViewModel": {
        "type": "object",
        "properties": {
          "title": {
            "type": "string",
            "nullable": true
          },
          "customerCompany": {
            "type": "string",
            "nullable": true
          },
          "performingCompany": {
            "type": "string",
            "nullable": true
          },
          "startDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "endDate": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "priority": {
            "maximum": 10,
            "minimum": 0,
            "type": "integer",
            "format": "int32",
            "nullable": true
          }
        },
        "additionalProperties": false
      }
    }
  }
}
```
</details>


## Автор
[Михаил](https://github.com/TXMMYBXY)