import { useState, useEffect } from 'react';

interface AuthInfo {
  id: string | null;
  role: string | null;
}

function parseJwt(token: string) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
}

export const useAuth = (): AuthInfo => {
  const [authInfo, setAuthInfo] = useState<AuthInfo>({ id: null, role: null });

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      const decoded = parseJwt(token);
      if (decoded) {
        setAuthInfo({
          id: decoded.nameid, // Standard claim for user ID in ASP.NET Core
          role: decoded.role,   // Standard claim for role
        });
      }
    }
  }, []);

  return authInfo;
};
