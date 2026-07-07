import {
  BulkDeleteRequest,
  BulkInsertRequest,
  CreateEmployeeRequest,
  CreateIssueRequest,
  CreateProjectRequest,
  EmployeeInfoResponse,
  EmployeeQueryParams,
  IssueInfoResponse,
  IssueQueryParams,
  PagedEmployeeResponse,
  PagedIssueResponse,
  PagedProjectResponse,
  ProjectInfoResponse,
  ProjectManagerResponse,
  ProjectQueryParams,
  UpdateEmployeeRequest,
  UpdateIssueRequest,
  UpdateProjectRequest,
  LoginRequest,
  RegisterRequest,
  TokenResponse,
} from './types';

// Vite exposes env vars as import.meta.env.VITE_*
// При сборке/запуске в Docker задаётся VITE_API_URL, иначе по умолчанию используем адрес внутри Docker-сети
const BASE_URL = (import.meta as any).env?.VITE_API_URL ?? 'http://projectmanager-api:8080';

function buildQuery(params: Record<string, unknown>): string {
  const q = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      q.append(key, String(value));
    }
  });
  const str = q.toString();
  return str ? `?${str}` : '';
}

async function request<T>(path: string, options?: RequestInit, isPublic = false): Promise<T> {
  const headers = new Headers({ 'Content-Type': 'application/json', Accept: 'application/json' });
  const token = localStorage.getItem('token');
  if (token && !isPublic) {
    headers.append('Authorization', `Bearer ${token}`);
  }

  const res = await fetch(`${BASE_URL}${path}`, { headers, ...options });

  if (res.status === 401 && !isPublic) {
    localStorage.removeItem('token');
    window.location.href = '/login';
    throw new Error('Unauthorized');
  }

  if (!res.ok) {
    const text = await res.text().catch(() => '');
    throw new Error(`HTTP ${res.status}: ${text || res.statusText}`);
  }
  const text = await res.text();
  if (!text) return undefined as T;
  return JSON.parse(text) as T;
}

// ─── Auth ───────────────────────────────────────────────────────────────────

export const authApi = {
  login: (body: LoginRequest) =>
    request<TokenResponse>('/api/auth/login', { method: 'POST', body: JSON.stringify(body) }, true),
  register: (body: RegisterRequest) =>
    request<void>('/api/auth/register', { method: 'POST', body: JSON.stringify(body) }, true),
  logout: () =>
    request<void>('/api/auth/logout', { method: 'POST' }),
};


// ─── Employee ───────────────────────────────────────────────────────────────

export const employeeApi = {
  list: (params: EmployeeQueryParams = {}) => {
    console.log('employeeApi.list called with:', params);
    return request<PagedEmployeeResponse>(`/api/employee${buildQuery(params as Record<string, unknown>)}`);
  },

  get: (id: number) => request<EmployeeInfoResponse>(`/api/employee/${id}`),

  getProjectManagers: () => {
    console.log('employeeApi.getProjectManagers called');
    return request<ProjectManagerResponse>('/api/employee/project-managers');
  },

  create: (body: CreateEmployeeRequest) =>
    request<void>('/api/employee', { method: 'POST', body: JSON.stringify(body) }),

  update: (id: number, body: UpdateEmployeeRequest) =>
    request<void>(`/api/employee/${id}`, { method: 'PATCH', body: JSON.stringify(body) }),

  delete: (id: number) => request<void>(`/api/employee/${id}`, { method: 'DELETE' }),

  bulkDelete: (body: BulkDeleteRequest) =>
    request<number>('/api/employee/bulk-delete', { method: 'POST', body: JSON.stringify(body) }),

  insertToProject: (employeeId: number, projectId: number) =>
    request<EmployeeInfoResponse>(`/api/employee/insert/${employeeId}/${projectId}`, { method: 'POST' }),

  removeFromProject: (employeeId: number, projectId: number) =>
    request<boolean>(`/api/employee/delete/${employeeId}/${projectId}`, { method: 'POST' }),

  bulkInsertToProject: (employeeId: number, body: BulkInsertRequest) =>
    request<number>(`/api/employee/bulk-insert/${employeeId}`, { method: 'POST', body: JSON.stringify(body) }),

  bulkRemoveFromProject: (employeeId: number, body: BulkDeleteRequest) =>
    request<number>(`/api/employee/bulk-delete/${employeeId}`, { method: 'POST', body: JSON.stringify(body) }),
};

// ─── Project ────────────────────────────────────────────────────────────────

export const projectApi = {
  list: (params: ProjectQueryParams = {}) =>
    request<PagedProjectResponse>(`/api/project${buildQuery(params as Record<string, unknown>)}`),

  get: (id: number) => request<ProjectInfoResponse>(`/api/project/${id}`),

  create: (body: CreateProjectRequest) =>
    request<void>('/api/project', { method: 'POST', body: JSON.stringify(body) }),

  update: (id: number, body: UpdateProjectRequest) =>
    request<void>(`/api/project/${id}`, { method: 'PATCH', body: JSON.stringify(body) }),

  delete: (id: number) => request<void>(`/api/project/${id}`, { method: 'DELETE' }),

  bulkDelete: (body: BulkDeleteRequest) =>
    request<number>('/api/project/bulk-delete', { method: 'POST', body: JSON.stringify(body) }),

  insertEmployee: (projectId: number, employeeId: number) =>
    request<ProjectInfoResponse>(`/api/project/insert/${projectId}/${employeeId}`, { method: 'POST' }),

  removeEmployee: (projectId: number, employeeId: number) =>
    request<boolean>(`/api/project/delete/${projectId}/${employeeId}`, { method: 'POST' }),

  bulkInsertEmployees: (projectId: number, body: BulkInsertRequest) =>
    request<number>(`/api/project/bulk-insert/${projectId}`, { method: 'POST', body: JSON.stringify(body) }),

  bulkRemoveEmployees: (projectId: number, body: BulkDeleteRequest) =>
    request<number>(`/api/project/bulk-delete/${projectId}`, { method: 'POST', body: JSON.stringify(body) }),
};

// ─── Issue ──────────────────────────────────────────────────────────────────

export const issueApi = {
  list: (params: IssueQueryParams = {}) =>
    request<PagedIssueResponse>(`/api/issue${buildQuery(params as Record<string, unknown>)}`),

  get: (id: number) => request<IssueInfoResponse>(`/api/issue/${id}`),

  create: (body: CreateIssueRequest) =>
    request<void>('/api/issue', { method: 'POST', body: JSON.stringify(body) }),

  update: (id: number, body: UpdateIssueRequest) =>
    request<void>(`/api/issue/${id}`, { method: 'PATCH', body: JSON.stringify(body) }),

  delete: (id: number) => request<void>(`/api/issue/${id}`, { method: 'DELETE' }),

  bulkDelete: (body: BulkDeleteRequest) =>
    request<number>('/api/issue/bulk-delete', { method: 'POST', body: JSON.stringify(body) }),
};
