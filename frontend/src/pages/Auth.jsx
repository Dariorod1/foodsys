import React, { useState, useRef } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import '../styles/Auth.css';

function Auth() {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({
    correo: '',
    contraseña: '',
    nombreCompleto: '',
    rol: 'Mozo'
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const loginCardRef = useRef(null);
  const registroCardRef = useRef(null);

  const { login, registro } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      if (isLogin) {
        await login({
          correo: formData.correo,
          contraseña: formData.contraseña
        });
      } else {
        await registro(formData);
      }
      navigate('/dashboard');
    } catch (err) {
      setError(err.message || (isLogin ? 'Error en el login' : 'Error en el registro'));
    } finally {
      setLoading(false);
    }
  };

  const toggleForm = () => {
    setIsLogin(!isLogin);
    setError('');
    setFormData({
      correo: '',
      contraseña: '',
      nombreCompleto: '',
      rol: 'Mozo'
    });
  };

  return (
    <div className="auth-container">
      <div className="auth-wrapper">
        {/* Tarjeta de Login */}
        <div className={`auth-card login-card ${!isLogin ? 'hidden' : ''}`} ref={loginCardRef}>
          <h1>Iniciar Sesión</h1>
          <form onSubmit={handleSubmit}>
            {error && isLogin && <div className="error-message">{error}</div>}
            
            <div className="form-group">
              <label htmlFor="login-correo">Correo</label>
              <input
                type="email"
                id="login-correo"
                name="correo"
                value={formData.correo}
                onChange={handleChange}
                required
                disabled={loading}
                placeholder="tu@email.com"
              />
            </div>

            <div className="form-group">
              <label htmlFor="login-contraseña">Contraseña</label>
              <input
                type="password"
                id="login-contraseña"
                name="contraseña"
                value={formData.contraseña}
                onChange={handleChange}
                required
                disabled={loading}
                placeholder="••••••••"
              />
            </div>

            <button type="submit" disabled={loading} className="btn-submit">
              {loading ? 'Cargando...' : 'Iniciar Sesión'}
            </button>
          </form>

          <div className="auth-toggle">
            <p>¿No tienes cuenta?</p>
            <button type="button" onClick={toggleForm} className="btn-toggle">
              Regístrate
            </button>
          </div>
        </div>

        {/* Tarjeta de Registro */}
        <div className={`auth-card registro-card ${isLogin ? 'hidden' : ''}`} ref={registroCardRef}>
          <h1>Crear Cuenta</h1>
          <form onSubmit={handleSubmit}>
            {error && !isLogin && <div className="error-message">{error}</div>}
            
            <div className="form-group">
              <label htmlFor="registro-nombre">Nombre Completo</label>
              <input
                type="text"
                id="registro-nombre"
                name="nombreCompleto"
                value={formData.nombreCompleto}
                onChange={handleChange}
                required
                disabled={loading}
                placeholder="Tu nombre"
              />
            </div>

            <div className="form-group">
              <label htmlFor="registro-correo">Correo</label>
              <input
                type="email"
                id="registro-correo"
                name="correo"
                value={formData.correo}
                onChange={handleChange}
                required
                disabled={loading}
                placeholder="tu@email.com"
              />
            </div>

            <div className="form-group">
              <label htmlFor="registro-contraseña">Contraseña</label>
              <input
                type="password"
                id="registro-contraseña"
                name="contraseña"
                value={formData.contraseña}
                onChange={handleChange}
                required
                disabled={loading}
                placeholder="••••••••"
              />
            </div>

            <div className="form-group">
              <label htmlFor="registro-rol">Rol</label>
              <select 
                id="registro-rol" 
                name="rol" 
                value={formData.rol} 
                onChange={handleChange}
                disabled={loading}
              >
                <option value="Mozo">Mozo</option>
                <option value="Cajero">Cajero</option>
                <option value="Encargado">Encargado</option>
                <option value="Administrador">Administrador</option>
              </select>
            </div>

            <button type="submit" disabled={loading} className="btn-submit">
              {loading ? 'Cargando...' : 'Registrarse'}
            </button>
          </form>

          <div className="auth-toggle">
            <p>¿Ya tienes cuenta?</p>
            <button type="button" onClick={toggleForm} className="btn-toggle">
              Inicia sesión
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Auth;
