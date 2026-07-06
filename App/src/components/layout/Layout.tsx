import React from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { Sidebar } from './Sidebar';
import { Button } from '../ui/Button';
import { authApi } from '../../api/client';
import { useToast } from '../ui/Toast';
import { useAuth } from '../../hooks/useAuth';

export const Layout: React.FC = () => {
  const navigate = useNavigate();
  const { toast } = useToast();
  const { id, role } = useAuth();

  const handleLogout = async () => {
    try {
      await authApi.logout();
    } catch (err: any) {
      toast('error', err.message || 'Logout failed');
    } finally {
      localStorage.removeItem('token');
      navigate('/login');
    }
  };

  return (
    <div className="flex min-h-screen bg-gray-50">
      <Sidebar />
      <main className="flex-1 overflow-auto">
        <div className="flex justify-between items-center p-4">
          <div>
            {id && role && (
              <span className="text-sm text-gray-500">
                User ID: {id} | Role: {role}
              </span>
            )}
          </div>
          <Button onClick={handleLogout} variant="outline" size="sm">
            Logout
          </Button>
        </div>
        <Outlet />
      </main>
    </div>
  );
};
