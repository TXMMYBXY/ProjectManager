import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastProvider } from './components/ui/Toast';
import { Layout } from './components/layout/Layout';
import { EmployeeList } from './pages/employees/EmployeeList';
import { EmployeeDetail } from './pages/employees/EmployeeDetail';
import { ProjectList } from './pages/projects/ProjectList';
import { ProjectDetail } from './pages/projects/ProjectDetail';
import { IssueList } from './pages/issues/IssueList';
import { IssueDetail } from './pages/issues/IssueDetail';
import { Login } from './pages/auth/Login';
import { Register } from './pages/auth/Register';
import { PrivateRoute } from './components/auth/PrivateRoute';

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <ToastProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route element={<PrivateRoute />}>
            <Route element={<Layout />}>
              <Route path="/" element={<Navigate to="/projects" replace />} />
              <Route path="/employees" element={<EmployeeList />} />
              <Route path="/employees/:id" element={<EmployeeDetail />} />
              <Route path="/projects" element={<ProjectList />} />
              <Route path="/projects/:id" element={<ProjectDetail />} />
              <Route path="/issues" element={<IssueList />} />
              <Route path="/issues/:id" element={<IssueDetail />} />
            </Route>
          </Route>
        </Routes>
      </ToastProvider>
    </BrowserRouter>
  );
};

export default App;
