import React, { createContext, useState, useCallback, useEffect } from 'react';
import { autenticacionService } from '../services/api';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token') || null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Verificar si hay sesión activa
  useEffect(() => {
    if (token) {
      verificarSesion();
    }
  }, [token]);

  const verificarSesion = async () => {
    try {
      const response = await autenticacionService.me();
      setUser(response.data);
      setError(null);
    } catch (err) {
      logout();
    }
  };

  const registro = useCallback(async (datos) => {
    setLoading(true);
    setError(null);
    try {
      const response = await autenticacionService.registro(datos);
      const { token: newToken, usuario } = response.data;
      
      localStorage.setItem('token', newToken);
      setToken(newToken);
      setUser(usuario);
      
      return response.data;
    } catch (err) {
      const mensaje = err.response?.data?.mensaje || 'Error en el registro';
      setError(mensaje);
      throw new Error(mensaje);
    } finally {
      setLoading(false);
    }
  }, []);

  const login = useCallback(async (datos) => {
    setLoading(true);
    setError(null);
    try {
      const response = await autenticacionService.login(datos);
      const { token: newToken, usuario } = response.data;
      
      localStorage.setItem('token', newToken);
      setToken(newToken);
      setUser(usuario);
      
      return response.data;
    } catch (err) {
      const mensaje = err.response?.data?.mensaje || 'Error en el login';
      setError(mensaje);
      throw new Error(mensaje);
    } finally {
      setLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem('token');
    setToken(null);
    setUser(null);
    setError(null);
  }, []);

  const value = {
    user,
    token,
    loading,
    error,
    isAuthenticated: !!token,
    registro,
    login,
    logout,
    verificarSesion
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = React.useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth debe usarse dentro de AuthProvider');
  }
  return context;
};
