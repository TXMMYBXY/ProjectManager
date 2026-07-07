// Enums
export enum IssueStatus {
  ToDo = 0,
  InProgress = 1,
  Done = 2,
}

export interface ProjectManagerResponse {
  projectManagers: EmployeeItemDto[] | null;
}

export interface DocumentDto {
  id: number;
  title: string;
}

export enum UserRole {
  Director = 1,
  Manager = 2,
  Employee = 3,
}

export const IssueStatusLabel: Record<IssueStatus, string> = {
  [IssueStatus.ToDo]: 'To Do',
  [IssueStatus.InProgress]: 'In Progress',
  [IssueStatus.Done]: 'Done',
};

export const UserRoleLabel: Record<UserRole, string> = {
  [UserRole.Director]: 'Director',
  [UserRole.Manager]: 'Manager',
  [UserRole.Employee]: 'Employee',
};

// DTOs
export interface EmployeeSummaryDto {
  id: number;
  firstName: string;
  lastName: string;
  patronymic?: string | null;
  email: string;
}

export interface EmployeeItemDto {
  id: number;
  firstName: string;
  lastName: string;
  patronymic?: string | null;
  email: string;
}

export interface ProjectSummaryDto {
  id: number;
  title: string;
  companyCustomer: string;
  companyExecuter: string;
  startDate: string;
  finishDate?: string | null;
  priority: number;
}

export interface ProjectItemDto {
  id: number;
  title: string;
  companyCustomer: string;
  companyExecuter: string;
  startDate: string;
  finishDate?: string | null;
  priority: number;
  projectManager?: EmployeeSummaryDto;
}

export interface IssueItemDto {
  id: number;
  title: string;
  status: IssueStatus;
  comments?: string | null;
  priority: number;
  project?: ProjectSummaryDto;
  author?: EmployeeSummaryDto;
  executor?: EmployeeSummaryDto;
}

// Paged responses
export interface PagedEmployeeResponse {
  employees: EmployeeItemDto[] | null;
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export interface PagedProjectResponse {
  projects: ProjectItemDto[] | null;
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

export interface PagedIssueResponse {
  issues: IssueItemDto[] | null;
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
}

// Detail responses
export interface EmployeeInfoResponse {
  id: number;
  firstName: string;
  lastName: string;
  patronymic?: string | null;
  email: string;
  projects?: ProjectItemDto[];
  authoredIssues?: IssueItemDto[];
  executedIssues?: IssueItemDto[];
}

export interface ProjectInfoResponse {
  id: number;
  title: string;
  companyCustomer: string;
  companyExecuter: string;
  startDate: string;
  finishDate?: string | null;
  priority: number;
  projectManager?: EmployeeSummaryDto;
  employees?: EmployeeItemDto[];
  issues?: IssueItemDto[];
  documents?: DocumentDto[];
}

export interface DocumentsResponse {
  documents?: DocumentDto[] | null;
}

export interface IssueInfoResponse {
  id: number;
  title: string;
  status: IssueStatus;
  comments?: string | null;
  priority: number;
  project?: ProjectSummaryDto;
  author?: EmployeeSummaryDto;
  executor?: EmployeeSummaryDto;
}

// Request bodies
export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  patronymic?: string | null;
  email: string;
  password?: string;
  role?: string;
}

export interface UpdateEmployeeRequest {
  firstName?: string | null;
  lastName?: string | null;
  patronymic?: { hasValue: boolean; value?: string | null };
  email?: string | null;
  role?: string | null;
}

export interface CreateProjectRequest {
  title: string;
  companyCustomer: string;
  companyExecutor: string;
  startDate: string;
  finishDate?: string | null;
  priority?: number;
  projectManagerId?: number | null;
}

export interface UpdateProjectRequest {
  title?: string | null;
  companyCustomer?: string | null;
  companyExecuter?: string | null;
  finishDate?: { hasValue: boolean; value?: string | null };
  priority?: number | null;
  projectManagerId?: number | null;
}

export interface CreateIssueRequest {
  title: string;
  status?: IssueStatus;
  comments?: string | null;
  priority?: number;
  projectId: number;
  executorId: number;
}

export interface UpdateIssueRequest {
  title?: string | null;
  status?: IssueStatus | null;
  comments?: { hasValue: boolean; value?: string | null };
  priority?: number | null;
  executorId?: number | null;
}

export interface BulkDeleteRequest {
  ids: number[];
}

export interface BulkInsertRequest {
  ids: number[];
}

// Query params
export interface EmployeeQueryParams {
  SortField?: string;
  Descending?: boolean;
  FirstName?: string;
  LastName?: string;
  Patronymic?: string;
  Email?: string;
  role?: UserRole;
  PageSize?: number;
  PageNumber?: number;
}

export interface ProjectQueryParams {
  SortBy?: string;
  Descending?: boolean;
  Title?: string;
  CompanyCustomer?: string;
  CompanyExecuter?: string;
  StartDateFrom?: string;
  StartDateTo?: string;
  FinishDateFrom?: string;
  FinishDateTo?: string;
  Priority?: number;
  ProjectManagerId?: number;
  PageSize?: number;
  PageNumber?: number;
}

export interface IssueQueryParams {
  SortField?: string;
  Descending?: boolean;
  Title?: string;
  Status?: string;
  Priority?: number;
  ProjectTitle?: string;
  AuthorFullName?: string;
  ExecutorFullName?: string;
  PageSize?: number;
  PageNumber?: number;
}

// Auth
export interface LoginRequest {
  email?: string;
  password?: string;
}

export interface RegisterRequest {
  email?: string;
  password?: string;
  firstName?: string;
  lastName?: string;
}

export interface TokenResponse {
  token: string;
}
